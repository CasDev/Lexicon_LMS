using LMS.Models;
using LMS.Models.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS.Controllers
{ n
    public class StudentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public User FindUser()
        {
//            return db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault(); //Redundant, överflödig kod. Where ej nödvändigt. 
            return db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
        }

        //John Hellman tyckte detta var jätteroligt! 
        public Course FindCourse(User user)
        {
            return db.Courses.FirstOrDefault(c => c.Id == user.CoursesId);
            //return db.Courses.Where(c => c.Users.Where(u => u.Id == user.Id).Count() > 0).FirstOrDefault();
            //Where ska man bara ha om man vill ha en lista, d v s ej då man bara vill plocka ut 1 item. 
        }

        public Course FindCourse()
        {
            return FindCourse(FindUser());
        }

        public Course FindCourse(int id)
        {
            return db.Courses.FirstOrDefault(c => c.Id == id); //Behåll denna. Bättre kod än att använda Where, vilket blir överflödigt (redundant). (John Hellman har hjälpt oss.) 
        }                                           

        public ICollection<User> FindParticipants(int id)
        {
            Course course = null;
            if (id > 0)
            {
                course = FindCourse(id);
            }
            course = (course != null ? course : new Course());
            return (course.Users != null ? course.Users : new List<User>());
        }
        
        public List<Activity> FindAllDeadlines(IEnumerable<Activity> list)
        {
             // hittar alla med en deadline, ur en fördefinierad lista
            return list.Where(x => x.Deadline != null).ToList(); //Här är det bra med Where, eftersom man vill ha en lista. 
        }

        public Activity FindActivity(int id)
        {
            return db.Activities.FirstOrDefault(a => a.Id == id);
        }

        public List<Activity> FindAllOldAndCurrentActivities(Course course)
        {
            // actuella aktivitetet
            List<Activity> Activities = new List<Models.Activity>();
            foreach (Module m in db.Modules.Where(m => m.CourseId == course.Id && DateTime.Now >= m.StartDate).ToList())
            {
                Activities.AddRange(m.Activities.Where(a => a.ModuleId == m.Id && (a.StartDate == null || DateTime.Now >= a.StartDate)).ToList());
                // .Where(a => a.StartDate == null || a.StartDate > DateTime.Now) kanske kan användas
            }
            return Activities;
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult Download(int? id)
        {
            if (id == null)
            {
                ViewBag.Error = "Inget Id angett for denna download";
                return View("~/Views/Error/Index.cshtml");
//                return Redirect("~/Error/?error=Inget Id angett för denna download");
            }

            Course course = FindCourse();
            if (course == null)
            {
                return View("~/Views/Student/NoKnown.cshtml");
            }

            Document doc = db.Documents.FirstOrDefault(d => d.Id == id);
            if (doc == null)
            {
                ViewBag.Error = "Inget document funnet";
                return View("~/Views/Error/Index.cshtml");
//                return Redirect("~/Error/?error=Inget document funnet");
            }
            if (doc.ActivityId == null && doc.CourseId == null && doc.ModuleId == null)
            {
                ViewBag.Error = "Du har ej tillgång hit";
                return View("~/Views/Error/Index.cshtml");
                //                return Redirect("~/Error/?error=Du har ej tillgång hit");
            }

            if (doc.CourseId != null)
            {
                Course _course = db.Courses.FirstOrDefault(m => m.Id == doc.CourseId);
                if (_course.Id != course.Id)
                {
                    ViewBag.Error = "Du har ej tillgång hit";
                    return View("~/Views/Error/Index.cshtml");
//                    return Redirect("~/Error/?error=Du har ej tillgång hit");
                }
            }
            else if (doc.ModuleId != null)
            {
                Module _module = db.Modules.FirstOrDefault(m => m.Id == doc.ModuleId);
                Course _course = _module.Course;
                if (_course.Id != course.Id)
                {
                    ViewBag.Error = "Du har ej tillgång hit";
                    return View("~/Views/Error/Index.cshtml");
                    //                    return Redirect("~/Error/?error=Du har ej tillgång hit");
                }
            }
            else if (doc.ActivityId != null)
            {
                Activity _activity = db.Activities.FirstOrDefault(m => m.Id == doc.ActivityId);
                Module _module = _activity.Module;
                Course _course = _module.Course;
                if (_course.Id != course.Id)
                {
                    ViewBag.Error = "Du har ej tillgång hit";
                    return View("~/Views/Error/Index.cshtml");
                    //                    return Redirect("~/Error/?error=Du har ej tillgång hit");
                }
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
        [Authorize(Roles = "Student")]
        public ActionResult Index()
        {
            Course course = FindCourse();
            if (course == null)
            {
                return View("~/Views/Student/NoKnown.cshtml");
            }

            createMenu();

            course.Modules = (course.Modules != null ? course.Modules : new List<Module>());
            course.Modules = course.Modules.Where(m => m.StartDate >= DateTime.Now || m.EndDate >= DateTime.Now).OrderBy(m => m.StartDate).ToList();

            ViewBag.Documents = DocumentCRUD.FindAllDocumentsBelongingToCourse(course.Id, db);

            MenyItems items = new MenyItems {
                new MenyItem { Text = course.Name, Link = "~/Student/" }
            };
            ViewBag.BreadCrumbs = items;

            return View(course);
        }
        
        private void createMenu(bool Home = false, MenyItem Back = null)
        {
            MenyItems items = new MenyItems();
            if (Home) {
                items.Add(new MenyItem { Text = "Hem", Link = "~/Student/" });
            }
            items.AddRange(new List<MenyItem> {
                new MenyItem { Text = "Inlämningsuppgifter", Link = "~/Student/Assignments/" },
                new MenyItem { Text = "Studenter", Link = "~/Student/Participants/" },
                new MenyItem { Text = "Äldre moduler", Link = "~/Student/OldModules/" }
            });
            if (Back != null)
            {
                items.Add(Back);
            }
            ViewBag.Menu = items;
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult Assignments(string sort)
        {
            if (FindCourse() == null)
            {
                return View("~/Views/Student/NoKnown.cshtml");
            }

            User user = FindUser();
            List<AssignmentStatus> assignments = new List<AssignmentStatus>();
            foreach (Activity activity in FindAllDeadlines(FindAllOldAndCurrentActivities(FindCourse())))
            {
                Document doc = DocumentCRUD.FindAssignment(user, activity, db, Server);
                bool done = (doc != null);
                bool isLeft = (done == false);
                bool delayed = ((!done && DateTime.Now > activity.Deadline) || (done && doc != null && doc.UploadTime > activity.Deadline));

                assignments.Add(new AssignmentStatus { Activity = activity, Delayed = delayed, Done = done, IsLeft = isLeft });
            }
            if (sort != null)
            {
                switch (sort.ToLower())
                {
                    case "delayed":
                        assignments = assignments.OrderBy(c => c.Delayed).Reverse().ToList();
                        break;
                    case "isleft":
                        assignments = assignments.OrderBy(c => c.IsLeft).Reverse().ToList();
                        break;
                    case "deadline":
                        assignments = assignments.OrderBy(c => c.Activity.Deadline).Reverse().ToList();
                        break;
                    default:
                        assignments = assignments.OrderBy(c => c.Done).Reverse().ToList();
                        break;
                }
            }

            createMenu(Home: true);

            MenyItems items = new MenyItems {
                new MenyItem { Text = FindCourse().Name, Link = "~/Student/" },
                new MenyItem { Text = user.FirstName +" "+ user.LastName +" övningsuppgifter", Link = "~/Student/Assignments" },
            };
            ViewBag.BreadCrumbs = items;

            return View(assignments);
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult OldModules()
        {
            Course course = FindCourse();
            if (course == null)
            {
                return View("~/Views/Student/NoKnown.cshtml");
            }

            createMenu(Home: true);

            course.Modules = (course.Modules != null ? course.Modules : new List<Module>());
            course.Modules = course.Modules.Where(m => m.EndDate < DateTime.Now).OrderBy(m => m.EndDate).Reverse().ToList();

            MenyItems items = new MenyItems {
                new MenyItem { Text = course.Name, Link = "~/Student/" },
                new MenyItem { Text = course.Name +"'s äldre moduler", Link = "~/Student/OldModules" },
            };
            ViewBag.BreadCrumbs = items;

            return View(course);
        }
        
        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult Participants(string sort)
        {
            Course course = FindCourse();
            if (course == null)
            {
                return View("~/Views/Student/NoKnown.cshtml");
            }

            createMenu(Home: true);

            bool _sort = (sort != null && sort == "FirstName" ? false : true);

            course.Users = (course.Users != null ? course.Users : new List<User>());
            if (_sort)
            {
                course.Users = course.Users.OrderBy(u => u.LastName).ToList();
            }
            else {
                course.Users = course.Users.OrderBy(u => u.FirstName).ToList();
            }
            int id = (course != null ? course.Id : 0);

            if (id <= 0)
            {
                ViewBag.Warning = "The ID of the Course was not able to be found";
            }

            MenyItems items = new MenyItems {
                new MenyItem { Text = course.Name, Link = "~/Student/" },
                new MenyItem { Text = course.Name +"'s deltagare", Link = "~/Student/Participants" },
            };
            ViewBag.BreadCrumbs = items;

            return View(course);
        }

        [Authorize(Roles = "Student")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Activity(HttpPostedFileBase file, int? id)
        {
            if (id == null)
            {
                ViewBag.Error = "Inget Id anget för aktivitet, ingen aktivitet kan åtkommas.";
                return View("~/Views/Error/Index.cshtml");
//                return Redirect("~/Error/?error=Inget Id angett för Activity");
            }

            if (FindCourse() == null)
            {
                return View("~/Views/Student/NoKnown.cshtml");
            }

            Activity activity = FindActivity((int)id);
            if (activity == null)
            {
                ViewBag.Error = "Aktiviteten har ej hittats";
                return View("~/Views/Error/Index.cshtml");
            }
            User user = FindUser();
            if (file != null && file.ContentLength > 0)
            {
                string extention = System.IO.Path.GetExtension(file.FileName);
                Document Document = DocumentCRUD.SaveDocument(Server.MapPath("~/documents/ovning/" + activity.Id + "/" + user.Id + "/"), "ovning", extention, file);
                if (Document == null)
                {
                    ModelState.AddModelError("", "Din inlämningsuppgift har ej sparats");
                }
                else
                {
                    Document.Name = "Inlämning för " + user.FirstName + " " + user.LastName;
                    Document.Description = "Inlämning för " + user.FirstName + " " + user.LastName;
                    Document.UploadTime = DateTime.Now;
                    Document.ActivityId = null;
                    Document.CourseId = null;
                    Document.ModifyUserId = null;
                    Document.ModuleId = null;
                    Document.UserId = user.Id;

                    db.Documents.Add(Document);
                    db.SaveChanges();
                }
            }
            else
            {
                ModelState.AddModelError("", "En fil med innehåll måste erhållas");
            }

            return Redirect("~/Student/Activity/" + id);
//            return View("~/Views/Student/Activity.cshtml", activity);
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult Activity(int? id)
        {
            if (FindCourse() == null)
            {
                return View("~/Views/Student/NoKnown.cshtml");
            }

            if (id == null)
            {
                ViewBag.Error = "Inget Id har angets";
                return View("~/Views/Error/Index.cshtml");
                //return Redirect("~/Error/?error=Inget Id angett för Activity");
            }
            Activity activity = FindActivity((int)id);

            if (activity == null)
            {
                return Redirect("~/Error/?error=Ingen Aktivitet funnen");
            }

            Module module = activity.Module;
            Course course = module.Course;

            if (course.Id != FindCourse().Id)
            {
                ViewBag.Error = "Du har ej tillgång hit";
                return View("~/Views/Error/Index.cshtml");
                //return Redirect("~/Error/?error=Du har ej tillgång hit.");
            }

            //Adrian föreslog en annan lösning på nedanstående kod. Se nedanför detta stycke, för hans ändring. 
            //MenyItems items = new MenyItems();
            //items.Items.Add(new MenyItem { Text = "Inlämningsuppgifter", Link = "~/Student/Assignments/" });
            //items.Items.Add(new MenyItem { Text = "Studenter", Link = "~/Student/Participants/" });
            //items.Items.Add(new MenyItem { Text = "Äldre moduler", Link = "~/Student/OldModules/" });
            //items.Items.Add(new MenyItem { Text = "Tillbaka till " + module.Name, Link = "~/Student/Module/" + module.Id });
            //ViewBag.Menu = items;
            createMenu(Home: true, Back: new MenyItem { Text = "Tillbaka till " + module.Name, Link = "~/Student/Module/" + module.Id });
            ViewBag.Documents = DocumentCRUD.FindAllDocumentsBelongingToActivity((int)id, db);

            MenyItems items = new MenyItems {
                new MenyItem { Text = course.Name, Link = "~/Student/" },
                new MenyItem { Text = module.Name, Link = "~/Student/Module/"+ module.Id },
                new MenyItem { Text = activity.Name, Link = "~/Student/Activity/"+ activity.Id },
            };
            ViewBag.BreadCrumbs = items;

            if (activity.Deadline != null)
            {
                ViewBag.HasFile = (DocumentCRUD.FindAssignment(FindUser(), activity, db, Server) != null ? true : false);
            }

            return View(activity);
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult Module(int? id)
        {
            if (FindCourse() == null)
            {
                return View("~/Views/Student/NoKnown.cshtml");
            }

            if (id == null)
            {
                ViewBag.Error = "Inget Id har angets för modulen";
                return View("~/Views/Error/Index.cshtml");
                //return Redirect("~/Error/?error=Inget Id angett för Modulen");
            }

            Module module = db.Modules.FirstOrDefault(m => m.Id == id);

            if (module == null)
            {
                return Redirect("~/Error/?error=Ingen Modul funnen");
            }

            //Adrian föreslog en annan lösning även på denna kod. Se nedanför detta stycke. 
            //MenyItems items = new MenyItems();
            //items.Items.Add(new MenyItem { Text = "Hem", Link = "~/Student/" });
            //items.Items.Add(new MenyItem { Text = "Inlämningsuppgifter", Link = "~/Student/Assignments/" });
            //items.Items.Add(new MenyItem { Text = "Studenter", Link = "~/Student/Participants/" });
            //items.Items.Add(new MenyItem { Text = "Äldre moduler", Link = "~/Student/OldModules/" });
            //items.Items.Add(new MenyItem { Text = "Äldre aktiviteter", Link = "~/Student/OldActivities/" + module.Id });
            //ViewBag.Menu = items;

            createMenu(Home: true, Back: new MenyItem { Text = "Äldre aktiviteter", Link = "~/Student/OldActivities/"+ module.Id });
            ViewBag.Documents = DocumentCRUD.FindAllDocumentsBelongingToModule((int)id, db);

            Course course = module.Course;

            if (course.Id != FindCourse().Id)
            {
                ViewBag.Error = "Du har ej tillgång hit";
                return View("~/Views/Error/Index.cshtml");
//                return Redirect("~/Error/?error=Du har ej tillgång hit.");
            }

            MenyItems items = new MenyItems {
                new MenyItem { Text = course.Name, Link = "~/Student/" },
                new MenyItem { Text = module.Name, Link = "~/Student/Module/"+ module.Id }
            };
            ViewBag.BreadCrumbs = items;

            return View(module);
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult OldActivities(int? id)
        {
            if (FindCourse() == null)
            {
                return View("~/Views/Student/NoKnown.cshtml");
            }

            if (id == null)
            {
                ViewBag.Error = "Inget Id har angets för modulen";
                return View("~/Views/Error/Index.cshtml");
//                return Redirect("~/Error/?error=Inget Id angett för Modulen");
            }

            Module module = db.Modules.FirstOrDefault(m => m.Id == id);
            if (module == null)
            {
                ViewBag.Error = "Ingen modul funnen";
                return View("~/Views/Error/Index.cshtml");
//                return Redirect("~/Error/?error=Ingen Modul funnen");
            }

            createMenu();

            Course course = module.Course;
            if (course.Id != FindCourse().Id)
            {
                ViewBag.Error = "Du har ej tillgång hit";
                return View("~/Views/Error/Index.cshtml");
//                return Redirect("~/Error/?error=Du har ej tillgång hit.");
            }

            MenyItems items = new MenyItems {
                new MenyItem { Text = course.Name, Link = "~/Student/" },
                new MenyItem { Text = module.Name, Link = "~/Student/Module/"+ module.Id },
                new MenyItem { Text = module.Name +"'s äldre aktiviteter", Link = "~/Student/OldActivities/"+ module.Id },
            };
            ViewBag.BreadCrumbs = items;

            return View(module);
        }
    }
}