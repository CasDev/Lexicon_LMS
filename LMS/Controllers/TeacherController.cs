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

        public void SetBreadcrumbs(MenyItem one = null, MenyItem two = null, MenyItem three = null, MenyItem four = null)
        {
            MenyItems items = new MenyItems();
            if (one != null)
            {
                items.Add(one); 
            }
            if (two != null)
            {
                items.Add(two);
            }
            if (three != null)
            {
                items.Add(three);
            }
            if (four != null)
            {
                items.Add(four);
            }
            ViewBag.BreadCrumbs = items;
        }

        public void Menu(bool Home = false, MenyItem Back = null)
        {
            MenyItems items = new MenyItems();
            if (Home)
            {
                items.Add(new MenyItem { Text = "Hem", Link = "~/Teacher/" });
            }
            items.AddRange(new List<MenyItem> {
                new MenyItem { Text = "Skapa ny kurs", Link = "~/Teacher/CreateCourse/" },
                new MenyItem { Text = "Skapa ny användare", Link = "~/Teacher/CreateUser/" },
                new MenyItem { Text = "Se äldre kurser", Link = "~/Teacher/OldCourses/" }
            });
            if (Back != null)
            {
                items.Add(Back);
            }
            ViewBag.Menu = items;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public ActionResult CreateDocument(int? id, string type, CreateDocumentViewModule model)
        {
            SetBreadcrumbs(one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" });
            Menu(Home: true);

            bool HasError = false;
            if (id == null)
            {
                ModelState.AddModelError("", "Ingen id har engets");
                HasError = true;
            }
            if (type == null)
            {
                ModelState.AddModelError("", "Ingen type har engets, som dokumentet skall tillgå till");
                HasError = true;
            }
            if (model.File != null && model.File.ContentLength <= 0)
            {
                ModelState.AddModelError("File", "Filen måste ha ett innehåll");
                HasError = true;
            }

            ViewBag.Id = id != null ? (int)id : 0;
            ViewBag.Type = type != null ? type : "none";

            if (!ModelState.IsValid)
            {
                HasError = true;
            }
            if (HasError)
            {
                return View(model);
            }

            int? courseId = null;
            int? moduleId = null;
            int? activityId = null;
            string GoTo = "";

            switch (type.ToLower())
            {
                case "course":
                    Course course = db.Courses.FirstOrDefault(c => c.Id == (int)id);
                    if (course == null)
                    {
                        ModelState.AddModelError("", "Kursen som letas efter har ej hittats");
                        HasError = true;
                    } else
                    {
                        courseId = course.Id;
                        GoTo = "Course/"+ courseId;
                    }
                    break;
                case "module":
                    Module module = db.Modules.FirstOrDefault(c => c.Id == (int)id);
                    if (module == null)
                    {
                        ModelState.AddModelError("", "Modulen som letas efter har ej hittats");
                        HasError = true;
                    }
                    else
                    {
                        moduleId = module.Id;
                        GoTo = "Module/" + moduleId;
                    }
                    break;
                case "activity":
                    Activity activity = db.Activities.FirstOrDefault(c => c.Id == (int)id);
                    if (activity == null)
                    {
                        ModelState.AddModelError("", "Activititeten som letas efter har ej hittats");
                        HasError = true;
                    }
                    else
                    {
                        activityId = activity.Id;
                        GoTo = "Activity/" + activityId;
                    }
                    break;
                default:
                    ModelState.AddModelError("", "Typen är ej igenkänd som en giltig entitet");
                    HasError = true;
                    break;
            }

            if (!HasError)
            {
                string extention = System.IO.Path.GetExtension(model.File.FileName);
                Document Document = DocumentCRUD.SaveDocument(Server.MapPath("~/documents/" + type.ToLower() + "/" + (int)id +"/"), model.Name.ToLower(), extention, model.File);
                if (Document == null) {
                    ModelState.AddModelError("File", "Dokumentet har ej sparats");
                }
                else
                {
                    Document.Name = model.Name;
                    Document.Description = model.Description != null ? model.Description : "";
                    Document.UploadTime = DateTime.Now;
                    Document.ModifyUserId = null;
                    Document.CourseId = courseId;
                    Document.ModuleId = moduleId;
                    Document.ActivityId = activityId;
                    Document.UserId = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id;

                    db.Documents.Add(Document);
                    db.SaveChanges();

                    return Redirect("~/Teacher/"+ GoTo);
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public ActionResult CreateDocument(int? id, string type)
        {
            if (id == null)
            {
                return Redirect("~/Error/?error=Inget Id angett för documentets föräldrar entitet");
            }
            if (type == null)
            {
                return Redirect("~/Error/?error=Inget typ angett för document");
            }

            ViewBag.Id = (int)id;
            ViewBag.Type = type;

            MenyItem item = null;

            switch (type.ToLower())
            {
                case "course":
                    Course course = db.Courses.FirstOrDefault(c => c.Id == (int)id);
                    if (course == null)
                    {
                        return Redirect("~/Error/?error=Ingen kurs funnen");
                    }

                    item = new MenyItem { Link = "~/Teacher/Course/" + id, Text = course.Name };
                    break;
                case "module":
                    Module module = db.Modules.FirstOrDefault(c => c.Id == (int)id);
                    if (module == null)
                    {
                        return Redirect("~/Error/?error=Ingen module funnen");
                    }

                    item = new MenyItem { Link = "~/Teacher/Module/" + id, Text = module.Name };
                    break;
                case "activity":
                    Activity activity = db.Activities.FirstOrDefault(c => c.Id == (int)id);
                    if (activity == null)
                    {
                        return Redirect("~/Error/?error=Ingen activitet funnen");
                    }

                    item = new MenyItem { Link = "~/Teacher/Activity/" + id, Text = activity.Name };
                    break;
                default:
                    return Redirect("~/Error/?error=Fel typ angett för document");
            }

            SetBreadcrumbs(one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" }, two: item);
            Menu(Home: true);

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
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

            SetBreadcrumbs(one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" }, two: new MenyItem { Link = "~/Teacher/ShowUser/"+ id, Text = user.FirstName +" "+ user.LastName });
            Menu(Home: true);

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

                assignment.Add(new AssignmentStatus {Doc = doc, User = user, Activity = activity, Delayed = delayed, Done = done, IsLeft = isLeft });
            }

            SetBreadcrumbs(
                one: new MenyItem { Link = "~/Teacher/Course/"+ course.Id, Text = course.Name },
                two: new MenyItem { Link = "~/Teacher/Module/"+ activity.ModuleId },
                three: new MenyItem { Link = "~/Teacher/Activity/"+ activity.Id, Text = activity.Name },
                four: new MenyItem { Link = "~/Teacher/Assignment/"+ id, Text = activity.Name +"'s inlämningsuppgifter" });
            Menu(Home: true);

            return View(assignment);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public ActionResult Download(int? id)
        {
            if (id == null)
            {
                ViewBag.Error = "Inget Id angett for denna download";
                return View("~/Views/Error/Index.cshtml");
                //                return Redirect("~/Error/?error=Inget Id angett för denna download");
            }

            Document doc = db.Documents.FirstOrDefault(d => d.Id == id);
            if (doc == null)
            {
                ViewBag.Error = "Inget document funnet";
                return View("~/Views/Error/Index.cshtml");
                //                return Redirect("~/Error/?error=Inget document funnet");
            }

            // from here and to return File(filedata, contentType)
            // http://stackoverflow.com/questions/5826649/returning-a-file-to-view-download-in-asp-net-mvc
            string filepath = doc.FileFolder + doc.FileName;
            byte[] filedata = System.IO.File.ReadAllBytes(filepath);
            string contentType = MimeMapping.GetMimeMapping(filepath);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = doc.FileName,
                Inline = false,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(filedata, contentType);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public ActionResult OldCourses()
        {
            Menu(Home: true);
            SetBreadcrumbs(one: new MenyItem { Link = "~/Teacher/", Text = "Se alla pågående kurser" });

            return View(db.Courses.Where(c => c.EndDate < DateTime.Now).ToList());
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public ActionResult OldModules(int? id)
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
            course.Modules = course.Modules.Where(m => m.EndDate < DateTime.Now).OrderBy(m => m.StartDate).ToList();

            Menu(Home: true, Back: new MenyItem { Link = "~/Teacher/Course/"+ id });
            SetBreadcrumbs(one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" }, two: new MenyItem { Link = "~/Teacher/Course/" + id, Text = course.Name }, three: new MenyItem { Link = "~/Teacher/OldModules/" + id, Text = course.Name +"'s äldre moduler" });
            
            return View(course);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
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

            Menu(Home: true, Back: new MenyItem { Link = "~/Teacher/OldModules/"+ course.Id, Text = course.Name + "'s äldre moduler" });
            SetBreadcrumbs(one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" }, two : new MenyItem { Link = "~/Teacher/Course/"+ id, Text = course.Name });

            ViewBag.Documents = DocumentCRUD.FindAllDocumentsBelongingToCourse(course.Id, db);

            return View(course);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public ActionResult OldActivities(int? id)
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

            Course course = module.Course;
            if (course == null)
            {
                return Redirect("~/Error/?error=Ingen kursförälder funnen");
            }

            Menu(Home: true, Back: new MenyItem { Link = "~/Teacher/Module/" + module.Id, Text = module.Name });
            SetBreadcrumbs(one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" }, two: new MenyItem { Link = "~/Teacher/Course/" + course.Id, Text = course.Name }, three: new MenyItem { Link = "~/Teacher/Module/" + module.Id, Text = module.Name }, four: new MenyItem { Link = "~/Teacher/OldActivities/" + module.Id, Text = module.Name +"'s äldre aktiviteter" });
            
            return View(module);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
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

            Course course = module.Course;
            if (course == null)
            {
                return Redirect("~/Error/?error=Ingen kursförälder funnen");
            }

            Menu(Home: true, Back: new MenyItem { Link = "~/Teacher/OldActivities/" + module.Id, Text = module.Name + "'s äldre aktiviteter" });
            SetBreadcrumbs(one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" }, two: new MenyItem { Link = "~/Teacher/Course/" + course.Id, Text = course.Name }, three: new MenyItem { Link = "~/Teacher/Module/" + module.Id, Text = module.Name });

            ViewBag.Documents = DocumentCRUD.FindAllDocumentsBelongingToModule(module.Id, db);

            return View(module);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
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

            Module module = activity.Module;
            if (module == null)
            {
                return Redirect("~/Error/?error=Ingen moduleförälder funnen");
            }
            Course course = module.Course;
            if (course == null)
            {
                return Redirect("~/Error/?error=Ingen kursförälder funnen");
            }

            Menu(Home: true, Back: (activity.Deadline != null ? new MenyItem { Link = "~/Teacher/Assignment/"+ activity.Id, Text = "Inlämningsuppgifter" } : null));
            SetBreadcrumbs(
                one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" },
                two: new MenyItem { Link = "~/Teacher/Course/" + course.Id, Text = course.Name },
                three: new MenyItem { Link = "~/Teacher/Module/" + module.Id, Text = module.Name },
                four: new MenyItem { Link = "~/Teacher/Activity/" + activity.Id, Text = activity.Name });

            ViewBag.Documents = DocumentCRUD.FindAllDocumentsBelongingToActivity(activity.Id, db);

            return View(activity);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public ActionResult CreateCourse()
        {
            Menu(Home: true);
            SetBreadcrumbs(
                one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" });

            ViewBag.AtEarliest = DateTime.Today.AddDays(1);

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public ActionResult CreateCourse(CreateCourseViewModel model)
        {
            ViewBag.AtEarliest = DateTime.Today.AddDays(1);

            Menu(Home: true);
            SetBreadcrumbs(
                one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" });

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
            if (db.Courses.FirstOrDefault(c => c.Name == model.Name) != null)
            {
                ModelState.AddModelError("Name", "Namnet för denna kurs är redan upptagen");
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
        [Authorize(Roles = "Teacher")]
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

            CreateModuleViewModel model = new CreateModuleViewModel { CourseId = (int)id, StartDate = (DateTime.Now > course.StartDate ? DateTime.Today.AddDays(1) : course.StartDate), EndDate = course.EndDate };
//            FetchAllCourses();  

            Menu(Home: true);
            SetBreadcrumbs(
                one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" },
                two: new MenyItem { Link = "~/Teacher/Course/" + course.Id, Text = course.Name });

            ViewBag.AtEarliest = (DateTime.Today.AddDays(1) > course.StartDate ? DateTime.Today.AddDays(1) : course.StartDate);
            ViewBag.AtLatest = (DateTime.Today.AddDays(2) < course.EndDate ? course.EndDate : DateTime.Today.AddDays(2));

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
        [Authorize(Roles = "Teacher")]
        public ActionResult CreateModule(CreateModuleViewModel model, int? id)
        {
            Course course = db.Courses.FirstOrDefault(c => c.Id == model.CourseId);
            ViewBag.AtEarliest = (course != null ? (DateTime.Today.AddDays(1) > course.StartDate ? DateTime.Today.AddDays(1) : course.StartDate) : DateTime.Today.AddDays(1));
            ViewBag.AtLatest = (course != null ? (DateTime.Today.AddDays(2) < course.EndDate ? course.EndDate : DateTime.Today.AddDays(2)) : DateTime.Today.AddDays(2));

            Menu(Home: true);
            SetBreadcrumbs(
                one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" },
                two: new MenyItem { Link = "~/Teacher/Course/" + id, Text = "Tillbaka till kurs" });

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
            
            if (course == null)
            {
                ModelState.AddModelError("", "Kursen kan ej hittas");
                hasError = true;
            }
            if (model.StartDate < course.StartDate)
            {
                ModelState.AddModelError("StartDate", "Kan ej starta innan kursen");
                hasError = true;
            }
            if (model.EndDate > course.EndDate)
            {
                ModelState.AddModelError("EndDate", "Kan ej sluta efter kursen");
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
        [Authorize(Roles = "Teacher")]
        public ActionResult CreateActivity(int? id)
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

            //            FetchAllModules();        //Anrop till metoden FetchAllModules.   
            CreateActivityViewModel model = new CreateActivityViewModel();
            model.ModuleId = (int)id;
            model.StartDate = (DateTime.Now > module.StartDate ? DateTime.Today.AddDays(1) : module.StartDate);
            model.EndDate = module.EndDate;

            Course course = module.Course;
            if (course == null)
            {
                return Redirect("~/Error/?error=Ingen kurs flrälder funnen");
            }

            Menu(Home: true);
            SetBreadcrumbs(
                one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" },
                two: new MenyItem { Link = "~/Teacher/Course/" + course.Id, Text = course.Name },
                three: new MenyItem { Link = "~/Teacher/Module/" + module.Id, Text = module.Name });

            ViewBag.AtEarliest = (DateTime.Today.AddDays(1) > module.StartDate ? DateTime.Today.AddDays(1) : module.StartDate);
            ViewBag.AtLatest = (DateTime.Today.AddDays(2) < module.EndDate ? module.EndDate : DateTime.Today.AddDays(1));

            return View(model);
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
        [Authorize(Roles = "Teacher")]
        public ActionResult CreateActivity(CreateActivityViewModel model, int? id)
        {
            Module module = db.Modules.FirstOrDefault(c => c.Id == model.ModuleId);
            ViewBag.AtEarliest = (module != null ? (DateTime.Today.AddDays(1) > module.StartDate ? DateTime.Today.AddDays(1) : module.StartDate) : DateTime.Today.AddDays(1));
            ViewBag.AtLatest = (module != null ? (DateTime.Today.AddDays(2) < module.EndDate ? module.EndDate : DateTime.Today.AddDays(2)) : DateTime.Today.AddDays(2));

            Menu(Home: true);
            SetBreadcrumbs(
                one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" },
                three: new MenyItem { Link = "~/Teacher/Module/" + id, Text = "Tillbaka till modul" });

            if (!ModelState.IsValid)
            {
                model.ModuleId = (id == null ? 0 : id);
                model.Type = null;
//                FetchAllModules();        //Anrop till metoden FetchAllModules.  

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
            
            if (module == null)
            {
                ModelState.AddModelError("", "Modulen kan ej hittas");
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
                model.ModuleId = (id == null ? 0 : id); ;
                model.Type = null;
//                FetchAllModules();

                return View(model);
            }

            Activity activity = new Activity { Name = model.Name, Description = (model.Description != null ? model.Description : ""), StartDate = model.StartDate, EndDate = model.EndDate, ModuleId = model.ModuleId, Deadline = model.Deadline, Type = model.Type };
            db.Activities.Add(activity);
            db.SaveChanges();

            return Redirect("~/Teacher/Activity/" + activity.Id); //Skickar vidare till vy med information om den aktivitet som vi just har skapat. 
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
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

            Menu(Home: true);
            SetBreadcrumbs(
                one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" },
                two: new MenyItem { Link = "~/Teacher/ShowUser/" + id, Text = user.FirstName +" "+ user.LastName });

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public ActionResult EditUser(string id, EditUserViewModel model)
        {
            Menu(Home: true);
            SetBreadcrumbs(
                one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" },
                two: new MenyItem { Link = "~/Teacher/ShowUser/"+ id, Text = "Tillbaka till användaren" });

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
        [Authorize(Roles = "Teacher")]
        public ActionResult CreateUser()
        {
            Menu(Home: true);
            SetBreadcrumbs(
                one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" });

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Roles = "Teacher")]
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
            Menu(Home: true);
            SetBreadcrumbs(
                one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" });

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
                HasError = true;
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

        [Authorize(Roles = "Teacher")]
        public ActionResult Index()
        {
            var courses = db.Courses.Where(c => c.EndDate > DateTime.Now); 

            Menu();
            SetBreadcrumbs(
                one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" }
            );

            return View(courses.ToList());
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
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

            EditCourseViewModel model = new EditCourseViewModel();      //Vi lägger info om kursen i en ViewModel-modell. 
            model.Description = course.Description; 
            model.Name = course.Name;
            model.StartDate = course.StartDate;
            model.EndDate = course.EndDate;
            ViewBag.id = course.Id;

            Menu(Home: true);
            SetBreadcrumbs(
                one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" },
                two: new MenyItem { Link = "~/Teacher/Course/" + course.Id, Text = course.Name });

            ViewBag.AtEarliest = DateTime.Today.AddDays(1);

            return View(model);
        }

        [ValidateAntiForgeryToken]  
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public ActionResult EditCourse(EditCourseViewModel model, int? id)      //Detta id hämtas från query-stringen.  
        {
            ViewBag.AtEarliest = DateTime.Today.AddDays(1);

            Menu(Home: true);
            SetBreadcrumbs(
                one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" },
                three: new MenyItem { Link = "~/Teacher/Course/" + id, Text = "Tillbaka till kurs" });

            bool hasError = false;
            if (id == null)
            {
                ModelState.AddModelError("", "Finns inget id");
                hasError = true;
            }

            var course = db.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null)
            {
                ModelState.AddModelError("", "Finns kurs id");
                hasError = true;
            }
            if (!ModelState.IsValid)        //MVC kollar om modellen har några fel själv, t ex det som är required; att strängen har en viss längd m m. 
            {
                hasError = true;
            }
            if (model.StartDate < DateTime.Today.AddDays(1))       //Vi kollar att den inte är mindre än morgondagen. D v s vi lägger till 1 till i dag, för att få morgondagen. Kollar att startdatumet ligger i framtiden. 
            {
                ModelState.AddModelError("StartDate", "Kursen kan inte starta före morgondagens datum.");
                hasError = true;
            }
            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "Kursen kan inte sluta före kursens startdatum. ");
                hasError = true; //D v s nu har det inträffat ett Error. 
            }
                if (hasError)
            {
                ViewBag.id = id != null ? (int)id : 0;            //Parameter castas om till en int. //Om id inte är null, så skickar vi id som parametern ViewBag = id. Annars om id är null, så sätter vi ViewBag till 0 per default. 
                return View(model);                               //Skickar tillbaka till samma vy, nu med alla felmeddelanden. 
            }

            course.Description = model.Description;                 // Coursens Description sätts lika med modellens Description, för att modellen representerar det vi ändrat i kursen. 
            course.Name = model.Name;
            course.StartDate = model.StartDate;
            course.EndDate = model.EndDate;

            db.Entry(course).State = EntityState.Modified; //Vi säger till databasen att vi uppdaterar/modifierar något. Ska göras så, enligt Entity Framework. 
            db.SaveChanges();

            return Redirect("~/Teacher/Course/" + course.Id); //Skickar vidare till kursens vy, för att se våra uppdateringar. 
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
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

            Course course = module.Course;
            if (course == null)
            {
                ViewBag.Error = "Ingen kurs förälder har hittats";
                return View("~/Views/Error/Index.cshtml");
            }

            EditModuleViewModel model = new EditModuleViewModel();
            model.Description = module.Description;
            model.Name = module.Name;
            model.StartDate = module.StartDate;
            model.EndDate = module.EndDate;

            ViewBag.Id = (int)id;

            Menu(Home: true);
            SetBreadcrumbs(
                one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" },
                two: new MenyItem { Link = "~/Teacher/Course/" + course.Id, Text = course.Name },
                three: new MenyItem { Link = "~/Teacher/Module/" + module.Id, Text = module.Name });

            ViewBag.AtEarliest = (DateTime.Today.AddDays(1) > course.StartDate ? DateTime.Today.AddDays(1) : course.StartDate);
            ViewBag.AtLatest = (DateTime.Today.AddDays(2) < course.EndDate ? course.EndDate : DateTime.Today.AddDays(2));

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public ActionResult EditModule(EditModuleViewModel model, int? id)
        {
            var module = db.Modules.FirstOrDefault(c => c.Id == id);
            ViewBag.AtEarliest = (module != null ? (DateTime.Today.AddDays(1) > module.StartDate ? DateTime.Today.AddDays(1) : module.StartDate) : DateTime.Today.AddDays(1));
            ViewBag.AtLatest = (module != null ? (DateTime.Today.AddDays(2) < module.EndDate ? module.EndDate : DateTime.Today.AddDays(2)) : DateTime.Today.AddDays(2));

            Menu(Home: true);
            SetBreadcrumbs(
                one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" },
                three: new MenyItem { Link = "~/Teacher/Module/" + id, Text = "Tillbaka till modul" });

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
            if (module == null)
            {
                ModelState.AddModelError("", "Ingen module funnen");
                hasError = true;
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

            Course course = db.Courses.FirstOrDefault(c => c.Id == module.CourseId);
            if (course == null)
            {
                ModelState.AddModelError("", "Ingen tillhörande kurs funnen");
                hasError = true;
            }

            if (model.StartDate < course.StartDate)
            {
                ModelState.AddModelError("StartDate", "Startdatum kan tyvärr ej starta innan kursen");
                hasError = true;
            }
            if (model.EndDate > course.EndDate)
            {
                ModelState.AddModelError("EndDate", "Slutdatumet kan ej överstrida kursens slutdatum");
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
        [Authorize(Roles = "Teacher")]
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

            Module module = activity.Module;
            if (module == null)
            {
                ViewBag.Error = "Ingen modul förälder har hittats";
                return View("~/Views/Error/Index.cshtml");
            }

            Course course = module.Course;
            if (course == null)
            {
                ViewBag.Error = "Ingen kurs flrälder har hittats";
                return View("~/Views/Error/Index.cshtml");
            }

            EditActivityViewModel model = new EditActivityViewModel();
            model.Description = activity.Description;
            model.Name = activity.Name;
            model.StartDate = (activity.StartDate != null ? (DateTime) activity.StartDate : DateTime.Today.AddDays(1));
            model.EndDate = (activity.EndDate != null ? (DateTime) activity.EndDate : DateTime.Today.AddDays(2));
            model.Deadline = activity.Deadline;

            ViewBag.Id = (int)id;

            Menu(Home: true);
            SetBreadcrumbs(
                one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" },
                two: new MenyItem { Link = "~/Teacher/Course/" + course.Id, Text = course.Name },
                three: new MenyItem { Link = "~/Teacher/Module/" + module.Id, Text = module.Name },
                four: new MenyItem { Link = "~/Teacher/Activity/" + activity.Id, Text = activity.Name });
            
            ViewBag.AtEarliest = (DateTime.Today.AddDays(1) > module.StartDate ? DateTime.Today.AddDays(1) : module.StartDate);
            ViewBag.AtLatest = (DateTime.Today.AddDays(2) < module.EndDate ? module.EndDate : DateTime.Today.AddDays(2));

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public ActionResult EditActivity(EditActivityViewModel model, int? id)
        {
            var activity = db.Activities.FirstOrDefault(c => c.Id == id);
            Module module = (activity != null ? db.Modules.FirstOrDefault(c => c.Id == activity.ModuleId) : null);
            ViewBag.AtEarliest = (DateTime.Today.AddDays(1) > module.StartDate ? DateTime.Today.AddDays(1) : module.StartDate);
            ViewBag.AtLatest = (DateTime.Today.AddDays(2) < module.EndDate ? module.EndDate : DateTime.Today.AddDays(2));

            Menu(Home: true);
            SetBreadcrumbs(
                one: new MenyItem { Link = "~/Teacher/", Text = "Se alla kurser" },
                three: new MenyItem { Link = "~/Teacher/Activity/" + id, Text = "Tillbaka till aktivitet" });

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
            if (activity == null)
            {
                ModelState.AddModelError("", "Ingen activitet funnen");
                hasError = true;
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
