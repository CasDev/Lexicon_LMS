using LMS.Models;
using LMS.Models.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS.Controllers
{
    public class StudentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public User FindUser()
        {
            return db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
        }

        public Course FindCourse(ApplicationUser user)
        {
            return db.Courses.Where(c => c.Users.Where(u => u.Id == user.Id).Count() > 0).FirstOrDefault();
        }

        public Course FindCourse()
        {
            return FindCourse(FindUser());
        }

        public Course FindCourse(int id)
        {
            return db.Courses.Where(c => c.Id == id).FirstOrDefault();
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
        
        public Activity FindActivity(int id)
        {
            return db.Activities.Where(a => a.Id == id).FirstOrDefault();
        }

        [Authorize(Roles = "Student")]
        public ActionResult Index()
        {
            Course course = FindCourse();
            if (course == null)
            {
                return View("~/Views/Student/NoKnown.cshtml");
            }

            MenyItems items = new MenyItems();
            items.Items.Add(new MenyItem { Text = "Se studenter för "+ course.Name, Link = "~/Student/Participants/" });
            items.Items.Add(new MenyItem { Text = "Se äldre moduler för " + course.Name, Link = "~/Student/OldModules/" });
            items.Items.Add(new MenyItem { Text = "Logga ut", Link = "~/Home/LogOff/" });
            ViewBag.Menu = items;

            course.Modules = (course.Modules != null ? course.Modules : new List<Module>());
            course.Modules = course.Modules.Where(m => m.StartDate >= DateTime.Now || m.EndDate >= DateTime.Now).OrderBy(m => m.StartDate).ToList();

            return View(course);
        }

        [Authorize(Roles = "Student")]
        public ActionResult OldModules()
        {
            Course course = FindCourse();
            if (course == null)
            {
                return View("~/Views/Student/NoKnown.cshtml");
            }

            MenyItems items = new MenyItems();
            items.Items.Add(new MenyItem { Text = "Hem", Link = "~/Student/" });
            items.Items.Add(new MenyItem { Text = "Logga ut", Link = "~/Home/LogOff/" });
            ViewBag.Menu = items;

            course.Modules = (course.Modules != null ? course.Modules : new List<Module>());
            course.Modules = course.Modules.Where(m => m.EndDate < DateTime.Now).OrderBy(m => m.EndDate).Reverse().ToList();

            return View(course);
        }

        [Authorize(Roles = "Student")]
        public ActionResult Participants(string sort)
        {
            Course course = FindCourse();
            if (course == null)
            {
                return View("~/Views/Student/NoKnown.cshtml");
            }

            MenyItems items = new MenyItems();
            items.Items.Add(new MenyItem { Text = "Hem", Link = "~/Student/" });
            items.Items.Add(new MenyItem { Text = "Logga ut", Link = "~/Home/LogOff/" });
            ViewBag.Menu = items;

            bool _sort = (sort != null && sort == "FirstName" ? false : true);

            course.Users = (course.Users != null ? course.Users : new List<User>());
            if (_sort)
            {
                course.Users = course.Users.OrderBy(u => u.LastName).ToList();
            }
            else {
                course.Users =  course.Users.OrderBy(u => u.FirstName).ToList();
            }
            int id = (course != null ? course.Id : 0);

            if (id <= 0)
            {
                ViewBag.Warning = "The ID of the Course was not able to be found";
            }

            return View(course);
        }

        [HttpPost]
        public ActionResult Assignment(int? id, HttpPostedFileBase file)
        {
            Activity activity = FindActivity((int)id); //Denna rad ändrad från nedanstående av Marie, p g a kompileringsfel. 
            //Activity activity = FindActivity(id);
            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                // TODO: make this work
//                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
//                file.SaveAs(path);
            } else
            {
                ModelState.AddModelError("", "En fil med innehåll måste erhållas");
            }
            // redirect back to the index action to show the form once again
//            return RedirectToAction("Index");

            return View("~/Views/Student/Activity.cshtml", activity);
        }

        [HttpGet]
        public ActionResult Assignment(int? id)
        {
            return Redirect("~/Student/Activity" + (id != null ? "?id="+ id : ""));
        }

        [Authorize(Roles = "Student")]
        public ActionResult Activity(int? id)
        {
            //TODO: vad händer om id är null?
            // TODO: Hämta aktivitet
            Activity activity = FindActivity((int)id);

            if (activity == null)
            {
                return View("~/Views/Student/NoKnown.cshtml");
            }
            /*
            if (course == null)
            {
                return View("~/Views/Student/NoKnown.cshtml");
            }*/

            MenyItems items = new MenyItems();
            items.Items.Add(new MenyItem { Text = "Se äldre aktiviteter för " + activity.Module.Name, Link = "~/Student/OldActivities/" + activity.Module.Id });
            items.Items.Add(new MenyItem { Text = "Logga ut", Link = "~/Home/LogOff/" });
            ViewBag.Menu = items;

            return View(activity);
        }

        [Authorize(Roles = "Student")]
        public ActionResult Module(int? id)
        {
            Module module = db.Modules.Where(m => m.Id == id).FirstOrDefault();

            return View(module);
        }
    }
}