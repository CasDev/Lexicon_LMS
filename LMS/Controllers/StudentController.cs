using LMS.Models;
using LMS.Models.DataAccess;
using System;
using System.Collections.Generic;
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
    }
}