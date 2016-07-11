using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.Models;
using LMS.Models.DataAccess;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LMS.Controllers
{
    public class TeacherController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult ShowUser(string id)
        {
            if (id == null)
            {
               return Redirect("~/Error/?error=Inget Id angett för Användaren");
            }

            User user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return Redirect("~/Error/?error=Ingen användare funnen");
            }

            return View(user);
        }
        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public ActionResult Assignment(int? id)
        {
            if (id == null)
            {
                return Redirect("~/Error/?error=Id saknas för Inlämningsuppgiften");
            }

            Activity activity = db.Activities.FirstOrDefault(a => a.Id == (int)id);
            if (activity == null)
            {
                return Redirect("~/Error/?error=Aktivitet saknas för Inlämningsuppgiften");
            }

            if (activity.Deadline == null)
            {
                return Redirect("~/Error/?error=Aktivitet saknar Inlämningsuppgift");
            }

            ViewBag.Activity = activity;

            List<AssignmentStatus> assignment = new List<AssignmentStatus>();
            Course course = activity.Module.Course;

            foreach (User user in course.Users)
            {
                Document doc = DocumentCRUD.FindAssignment(user, activity, db, Server);
                bool done = (doc != null);
                bool isLeft = (done == false);
                bool delayed = ((!done && DateTime.Now > activity.Deadline) || (done && doc != null && doc.UploadTime > activity.Deadline));

                assignment.Add(new AssignmentStatus {User = user, Activity = activity, Delayed = delayed, Done = done, IsLeft = isLeft });
            }

            return View(assignment);
        }

        [HttpGet]
        public ActionResult OldCourses()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Course(int? id)
        {
            if (id == null)
            {
                return Redirect("~/Error/?error=Inget Id angett för kursen");
            }

            Course course = db.Courses.FirstOrDefault(m => m.Id == (int)id);
            if (course == null)
            {
                return Redirect("~/Error/?error=Ingen kurs funnen");
            }
            course.Modules = (course.Modules != null ? course.Modules : new List<Module>());
            course.Modules = course.Modules.Where(m => m.EndDate > DateTime.Now).OrderBy(m => m.StartDate).ToList();

            return View(course);
        }

        [HttpGet]
        public ActionResult Module(int? id)
        {
            if (id == null)
            {
                return Redirect("~/Error/?error=Inget Id angett för Modulen");
            }

            Module module = db.Modules.FirstOrDefault(m => m.Id == (int)id);
            if (module == null)
            {
                return Redirect("~/Error/?error=Ingen module funnen");
            }

            return View(module);
        }

        [HttpGet]
        public ActionResult Activity(int? id)
        {
            if (id == null)
            {
                return Redirect("~/Error/?error=Inget Id angett för aktiviteten");
            }

            Activity activity = db.Activities.FirstOrDefault(m => m.Id == (int)id);
            if (activity == null)
            {
                return Redirect("~/Error/?error=Ingen activitet funnen");
            }

            return View(activity);
        }

        [HttpGet]
        public ActionResult CreateCourse()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateCourse(CreateCourseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool hasError = false;
            if (model.StartDate < DateTime.Today.AddDays(1))
            {
                ModelState.AddModelError("StartDate", "Startdatum kan tyvärr ej starta innan morgondagen, pga. planeringstid");
                hasError = true;
            }
            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "Slutdatumet kan ej vara innan startdatumet");
                hasError = true;
            }
            if (hasError)
            {
                return View(model);
            }

            Course course = new Course { Name = model.Name, Description = (model.Description != null ? model.Description : ""), StartDate = model.StartDate, EndDate = model.EndDate };
            db.Courses.Add(course);
            db.SaveChanges();

            return Redirect("~/Teacher/Course/"+ course.Id);
        }

        [HttpGet]
        public ActionResult CreateModule(int? id)
        {
            if (id == null)
            {
                return Redirect("~/Error/?error=Inget Id angett för Modulen");
            }

            Course course = db.Courses.FirstOrDefault(m => m.Id == (int)id);
            if (course == null)
            {
                return Redirect("~/Error/?error=Ingen course funnen");
            }

            CreateModuleViewModel model = new CreateModuleViewModel { CourseId = (int)id };
//            FetchAllCourses();  

            return View(model);
        }

        // var i första delen när man valde kurs genom en dropdown-lista
        public void FetchAllCourses()
        {
            List<SelectListItem> courses = new List<SelectListItem>();
            foreach (Course c in db.Courses)
            {
                courses.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            }

            ViewBag.Courses = courses;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateModule(CreateModuleViewModel model, int? id)
        {
            bool hasError = false;
            if (id == null)
            {
                ModelState.AddModelError("", "Inget id funnet, var god återge vilken kurs du vill använda");
                hasError = true;
            }

            if (!ModelState.IsValid)
            {
                model.CourseId = (id != null ? (int) id : 0);
                //FetchAllCourses();

                return View(model);
            }

            if (model.StartDate < DateTime.Today.AddDays(1))
            {
                ModelState.AddModelError("StartDate", "Startdatum kan tyvärr ej starta innan morgondagen, pga. planeringstid");
                hasError = true;
            }
            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "Slutdatumet kan ej vara innan startdatumet");
                hasError = true;
            }
            Course course = db.Courses.FirstOrDefault(c => c.Id == model.CourseId);
            if (course == null)
            {
                ModelState.AddModelError("", "Kursen kan ej hittas");
                hasError = true;
            }
            
            if (hasError)
            {
                model.CourseId = (id != null ? (int)id : 0);
                //FetchAllCourses();

                return View(model);
            }


            Module module = new Module { Name = model.Name, Description = (model.Description != null ? model.Description : ""), StartDate = model.StartDate, EndDate = model.EndDate, CourseId = model.CourseId };
            db.Modules.Add(module);
            db.SaveChanges();

            return Redirect("~/Teacher/Module/" + module.Id);
        }

        [HttpGet]
        public ActionResult CreateActivity()
        {
            FetchAllModules();        //Anrop till metoden FetchAllModules.   
            return View();
        }

        public void FetchAllModules()       //Denna metod ser till att man kan välja modul, då man skapar en aktivitet. 
        {
            List<SelectListItem> courses = new List<SelectListItem>();
            foreach (Module c in db.Modules)
            {
                courses.Add(new SelectListItem { Text = c.Name +" ( "+ c.StartDate.ToString("yyyy-MM-dd") +" - "+ c.EndDate.ToString("yyyy-MM-dd") + " )", Value = c.Id.ToString() });
            }

            ViewBag.Modules = courses;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateActivity(CreateActivityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ModuleId = null;
                model.Type = null;
                FetchAllModules();        //Anrop till metoden FetchAllModules.  

                return View(model);
            }

            bool hasError = false;
            if (model.StartDate != null && model.StartDate < DateTime.Today.AddDays(1))
            {
                ModelState.AddModelError("StartDate", "Startdatum kan tyvärr ej starta innan morgondagen, pga. planeringstid");
                hasError = true;
            }
            if (model.EndDate != null && model.StartDate != null && model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "Slutdatumet kan ej vara innan startdatumet");
                hasError = true;
            }
            if (model.EndDate != null && model.EndDate < DateTime.Today.AddDays(1))
            {
                ModelState.AddModelError("EndDate", "Slutdatum kan ej vara innan morgondagen");
                hasError = true;
            }
            if (model.Deadline != null && model.StartDate != null && model.Deadline < model.StartDate)
            {
                ModelState.AddModelError("Deadline", "Deadline för övningsuppgift kan ej vara innan startdatumet");
                hasError = true;
            }
            if (model.Deadline != null && model.EndDate != null && model.Deadline > model.EndDate)
            {
                ModelState.AddModelError("Deadline", "Deadline för övningsuppgift kan ej vara efter slutdatumet");
                hasError = true;
            }
            if (model.Deadline != null && model.Deadline < DateTime.Today.AddDays(1))
            {
                ModelState.AddModelError("Deadline", "Deadline för övningsuppgift kan ej vara innan morgondagen");
                hasError = true;
            }
            Module module = db.Modules.FirstOrDefault(c => c.Id == model.ModuleId);
            if (module == null)
            {
                ModelState.AddModelError("ModuleId", "Modulen kan ej hittas");
                hasError = true;
            } else
            {
                if (model.StartDate != null && model.StartDate < module.StartDate)
                {
                    ModelState.AddModelError("StartDate", "Startdatum kan ej starta innan modulen");
                    hasError = true;
                }
                if (model.EndDate != null && model.EndDate > module.EndDate)
                {
                    ModelState.AddModelError("EndDate", "Slutdatum kan ej sluta efter modulen");
                    hasError = true;
                }
                if (model.Deadline != null && model.Deadline < module.StartDate)
                {
                    ModelState.AddModelError("Deadline", "Deadline för övningsuppgift kan ej vara innan modulens startdatum");
                    hasError = true;
                }
                if (model.Deadline != null && model.Deadline > module.EndDate)
                {
                    ModelState.AddModelError("Deadline", "Deadline för övningsuppgift kan ej vara efter modulens slutdatumet");
                    hasError = true;
                }
            }
            if (hasError)
            {
                model.ModuleId = null;
                model.Type = null;
                FetchAllModules();

                return View(model);
            }

            Activity activity = new Activity { Name = model.Name, Description = (model.Description != null ? model.Description : ""), StartDate = model.StartDate, EndDate = model.EndDate, ModuleId = model.ModuleId, Deadline = model.Deadline, Type = model.Type };
            db.Activities.Add(activity);
            db.SaveChanges();

            return Redirect("~/Teacher/Activity/" + activity.Id); //Skickar vidare till vy med information om den aktivitet som vi just har skapat. 
        }

        [HttpGet]
        public ActionResult EditUser(string id)
        {
            if (id == null)
            {
                ViewBag.Error = "Inget id har angivits";
                return View("~/Views/Error/Index.cshtml");
            }

            var user = db.Users.FirstOrDefault(c => c.Id == id);
            if (user == null)
            {
                ViewBag.Error = "Ingen användare har hittats";
                return View("~/Views/Error/Index.cshtml");
            }

            EditUserViewModel model = new EditUserViewModel();
            model.CourseId = (user.CoursesId != null ? (int)user.CoursesId : 0);
            model.Email = user.Email;

            List<SelectListItem> ListItems = new List<SelectListItem>();
            foreach (Course c in db.Courses.Where(c => c.EndDate > DateTime.Now).ToList())
            {
                ListItems.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = ((user.CoursesId != null ? user.CoursesId : 0) == c.Id) });
            }

            ViewBag.Id = user.Id;
            ViewBag.CourseIds = ListItems;

            return View(model);
        }

        [HttpPost]
        public ActionResult EditUser(string id, EditUserViewModel model)
        {
            if (id == null)
            {
                ViewBag.Error = "Inget id har angivits";
                return View("~/Views/Error/Index.cshtml");
            }

            bool hasError = false;
            var user = db.Users.FirstOrDefault(c => c.Id == id);
            if (user == null)
            {
                ModelState.AddModelError("", "Ingen användare funnen");
                hasError = true;
            }

            if (!ModelState.IsValid)
            {
                List<SelectListItem> ListItems = new List<SelectListItem>();
                foreach (Course c in db.Courses.Where(c => c.EndDate > DateTime.Now).ToList())
                {
                    ListItems.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = ((user != null && user.CoursesId != null ? user.CoursesId : 0) == c.Id) });
                }

                ViewBag.Id = id;
                ViewBag.CourseIds = ListItems;

                return View(model);
            }
            var course = db.Courses.FirstOrDefault(c => c.Id == model.CourseId);
            if (course == null)
            {
                ModelState.AddModelError("CourseId", "Kurs har ej hittats");
                hasError = true;
            }

            if (hasError)
            {
                List<SelectListItem> ListItems = new List<SelectListItem>();
                foreach (Course c in db.Courses.Where(c => c.EndDate > DateTime.Now).ToList())
                {
                    ListItems.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = ((user != null && user.CoursesId != null ? user.CoursesId : 0) == c.Id) });
                }

                ViewBag.Id = id;
                ViewBag.CourseIds = ListItems;

                return View(model);
            }
            
            return View();
        }

        [HttpGet]
        public ActionResult CreateUser()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateUser(CreateUserViewModel model)
        {
            // skillnaderna mellan en vymodel såsom LoginViewModel och en FormCollection
            //LoginViewModel Model = new LoginViewModel();
            //
            //if (!ModelState.IsValid)
            //{
            //    // här händer nåt
            //}
            //
            //var epost = Model.Email;
            //var password = Model.Password;
            //bool Remember = Model.RememberMe;
            //
            // med en vymodel så sköter MVC mycket själv.
            // t.ex. denna validering kan lätt ändras till en enkel
            // if (!ModelState.IsValid) {
            // så kontrollerar den ifall vymodel EJ är giltig och sedan får programmeraren
            // göra det som skall, t.ex. skicka tillbaka datat och felmedellandena ( som
            // även sköts av vymodelen )
            //epost = Collection["email"];
            //if (epost.Length > 9)
            //{
            //    // OK
            //}
            //password = Collection["password"];
            //if (password.Length > 9)
            //{
            //    // OK
            //}
            //if (Boolean.TryParse(Collection["RememberMe"], out Remember))
            //{
            //    // OK
            //}
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool HasError = false;
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Det konfirmerande lösenordet är ej detsamma som lösenordet du har angett");
                HasError = true;
            }
            if (model.Role != "Teacher" && model.Role != "Student")
            {
                ModelState.AddModelError("Role", "Användare kan enbart vara av roll lärare eller elev");
            }
            if (HasError)
            {
                return View(model);
            }

            var uStore = new UserStore<User>(db);
            var uManager = new UserManager<User>(uStore);

            User user = new User { FirstName = model.FirstName, LastName = model.LastName, Email = model.Email, UserName = model.Email };
            uManager.Create(user, model.Password);

            user = uManager.FindByName(model.Email);
            uManager.AddToRole(user.Id, model.Role);

            return Redirect("~/Teacher/");
        }

        public ActionResult Index()
        {
            var courses = db.Courses.Where(c => c.EndDate > DateTime.Now); 
            return View(courses.ToList());
        }

        [HttpGet]
        public ActionResult EditCourse(int? id)
        {
            if (id == null )
            {
                ViewBag.Error = "Inget id har angivits";
                return View("~/Views/Error/Index.cshtml");
            }

            var course = db.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null)
            {
                ViewBag.Error = "Ingen kurs har hittats";
                return View("~/Views/Error/Index.cshtml");
            }

            EditCourseViewModel model = new EditCourseViewModel();
            model.Description = course.Description; 
            model.Name = course.Name;
            model.StartDate = course.StartDate;
            model.EndDate = course.EndDate;

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditCourse(EditCourseViewModel model)
        {
            return View();
        }

    
        [HttpGet]
        public ActionResult EditModule(int? id)
        {
            if (id == null)
            {
                ViewBag.Error = "Inget id har angivits";
                return View("~/Views/Error/Index.cshtml");
            }

            var module = db.Modules.FirstOrDefault(c => c.Id == id);
            if (module == null)
            {
                ViewBag.Error = "Ingen module har hittats";
                return View("~/Views/Error/Index.cshtml");
            }

            EditModuleViewModel model = new EditModuleViewModel();
            model.Description = module.Description;
            model.Name = module.Name;
            model.StartDate = module.StartDate;
            model.EndDate = module.EndDate;

            ViewBag.Id = (int)id;

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditModule(EditModuleViewModel model, int? id)
        {
            if (id == null)
            {
                ViewBag.Error = "Inget id har angivits";
                return View("~/Views/Error/Index.cshtml");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Id = (int)id;

                return View(model);
            }

            bool hasError = false;
            var module = db.Modules.FirstOrDefault(c => c.Id == id);
            if (module == null)
            {
                ModelState.AddModelError("", "Ingen module funnen");
                hasError = false;
            }

            if (model.StartDate < DateTime.Today.AddDays(1))
            {
                ModelState.AddModelError("StartDate", "Startdatum kan tyvärr ej starta innan morgondagen, pga. planeringstid");
                hasError = true;
            }
            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "Slutdatumet kan ej vara innan startdatumet");
                hasError = true;
            }

            if (hasError)
            {
                ViewBag.Id = (int)id;

                return View(model);
            }

            module.Description = model.Description;
            module.EndDate = model.EndDate;
            module.StartDate = model.StartDate;
            module.Name = model.Name;

            db.Entry(module).State = EntityState.Modified;
            db.SaveChanges();

            return Redirect("~/Teacher/Module/"+ module.Id);
        }

        [HttpGet]
        public ActionResult EditActivity(int? id)
        {
            if (id == null)
            {
                ViewBag.Error = "Inget id har angivits";
                return View("~/Views/Error/Index.cshtml");
            }

            var activity = db.Activities.FirstOrDefault(c => c.Id == id);
            if (activity == null)
            {
                ViewBag.Error = "Ingen activitet har hittats";
                return View("~/Views/Error/Index.cshtml");
            }

            EditActivityViewModel model = new EditActivityViewModel();
            model.Description = activity.Description;
            model.Name = activity.Name;
            model.StartDate = (activity.StartDate != null ? (DateTime) activity.StartDate : DateTime.Today.AddDays(1));
            model.EndDate = (activity.EndDate != null ? (DateTime) activity.EndDate : DateTime.Today.AddDays(2));
            model.Deadline = activity.Deadline;

            ViewBag.Id = (int)id;

            return View(activity);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditActivity(EditActivityViewModel model, int? id)
        {
            if (id == null)
            {
                ViewBag.Error = "Inget id har angivits";
                return View("~/Views/Error/Index.cshtml");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Id = (int)id;

                return View(model);
            }

            bool hasError = false;
            var activity = db.Activities.FirstOrDefault(c => c.Id == id);
            if (activity == null)
            {
                ModelState.AddModelError("", "Ingen activitet funnen");
                hasError = false;
            }

            if (model.StartDate != null && model.StartDate < DateTime.Today.AddDays(1))
            {
                ModelState.AddModelError("StartDate", "Startdatum kan tyvärr ej starta innan morgondagen, pga. planeringstid");
                hasError = true;
            }
            if (model.EndDate != null && model.StartDate != null && model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "Slutdatumet kan ej vara innan startdatumet");
                hasError = true;
            }
            if (model.EndDate != null && model.EndDate < DateTime.Today.AddDays(1))
            {
                ModelState.AddModelError("EndDate", "Slutdatum kan ej vara innan morgondagen");
                hasError = true;
            }
            if (model.Deadline != null && model.StartDate != null && model.Deadline < model.StartDate)
            {
                ModelState.AddModelError("Deadline", "Deadline för övningsuppgift kan ej vara innan startdatumet");
                hasError = true;
            }
            if (model.Deadline != null && model.EndDate != null && model.Deadline > model.EndDate)
            {
                ModelState.AddModelError("Deadline", "Deadline för övningsuppgift kan ej vara efter slutdatumet");
                hasError = true;
            }
            if (model.Deadline != null && model.Deadline < DateTime.Today.AddDays(1))
            {
                ModelState.AddModelError("Deadline", "Deadline för övningsuppgift kan ej vara innan morgondagen");
                hasError = true;
            }
            Module module = db.Modules.FirstOrDefault(c => c.Id == activity.Id);
            if (module == null)
            {
                ModelState.AddModelError("", "Föräldrar modulen kan ej hittas");
                hasError = true;
            }
            else
            {
                if (model.StartDate != null && model.StartDate < module.StartDate)
                {
                    ModelState.AddModelError("StartDate", "Startdatum kan ej starta innan modulen");
                    hasError = true;
                }
                if (model.EndDate != null && model.EndDate > module.EndDate)
                {
                    ModelState.AddModelError("EndDate", "Slutdatum kan ej sluta efter modulen");
                    hasError = true;
                }
                if (model.Deadline != null && model.Deadline < module.StartDate)
                {
                    ModelState.AddModelError("Deadline", "Deadline för övningsuppgift kan ej vara innan modulens startdatum");
                    hasError = true;
                }
                if (model.Deadline != null && model.Deadline > module.EndDate)
                {
                    ModelState.AddModelError("Deadline", "Deadline för övningsuppgift kan ej vara efter modulens slutdatumet");
                    hasError = true;
                }
            }

            if (hasError)
            {
                ViewBag.Id = (int)id;

                return View(model);
            }

            activity.Description = model.Description;
            activity.EndDate = model.EndDate;
            activity.StartDate = model.StartDate;
            activity.Name = model.Name;
            activity.Deadline = model.Deadline;

            db.Entry(activity).State = EntityState.Modified;
            db.SaveChanges();

            return Redirect("~/Teacher/Activity/" + activity.Id);
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
