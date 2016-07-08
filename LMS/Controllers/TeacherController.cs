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
        public ActionResult Assignment(int? id)
        {
            return View();
        }

        [HttpGet]
        public ActionResult OldCourses()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Course(int? id)
        {
            return View();
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
            return View();
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
        public ActionResult CreateModule()
        {
            FetchAllCourses();  

            return View();
        }

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
        public ActionResult CreateModule(CreateModuleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                FetchAllCourses();

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
            Course course = db.Courses.FirstOrDefault(c => c.Id == model.CourseId);
            if (course == null)
            {
                ModelState.AddModelError("CourseId", "Kursen kan ej hittas");
                hasError = true;
            }
            
            if (hasError)
            {
                FetchAllCourses();

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
                courses.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            }

            ViewBag.Modules = courses;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateActivity(CreateActivityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                FetchAllModules();        //Anrop till metoden FetchAllModules.   
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
            Module module = db.Modules.FirstOrDefault(c => c.Id == model.ModuleId);
            if (module == null)
            {
                ModelState.AddModelError("ModuleId", "Kursen kan ej hittas");
                hasError = true;
            }
            if (hasError)
            {
                FetchAllCourses();

                return View(model);
            }


            Activity activity = new Activity { Name = model.Name, Description = (model.Description != null ? model.Description : ""), StartDate = model.StartDate, EndDate = model.EndDate, ModuleId = model.ModuleId, Deadline = model.Deadline };
            db.Activities.Add(activity);
            db.SaveChanges();

            return Redirect("~/Teacher/Activity/" + activity.Id); //Skickar vidare till vy med information om den aktivitet som vi just har skapat. 
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
            var users = db.Users.Include(u => u.Courses);
            return View(users.ToList());
        }

        [HttpGet]
        public ActionResult EditCourse(int? id)
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditCourse(FormCollection collection)
        {
            return View();
        }

    
        [HttpGet]
        public ActionResult EditModule(int? id)
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditModule(FormCollection collection)
        {
            return View();
        }

        [HttpGet]
        public ActionResult EditActivity(int? id)
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditActivity(FormCollection collection)
        {
            return View();
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
