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

        // GET: Student
        [Authorize(Roles = "Student")]
        public ActionResult Index()
        {
            Course course = FindCourse();
            course.Modules = (course.Modules != null ? course.Modules : new List<Module>());
            course.Modules = course.Modules.Where(m => m.StartDate >= DateTime.Now && m.EndDate >= DateTime.Now).OrderBy(m => m.StartDate).ToList();

            return View(course);
        }

        [Authorize(Roles = "Student")]
        public ActionResult Participants(string sort)
        {
            bool _sort = (sort != null && sort == "FirstName" ? false : true);

            Course course = FindCourse();
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