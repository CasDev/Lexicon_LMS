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

        public ApplicationUser FindUser()
        {
            return db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
        }

        public Course FindCourse(ApplicationUser user)
        {
            return db.Courses.Where(c => c.Users.Where(u => u.Id == user.Id).Count() > 0).FirstOrDefault();
        }

        public Course FindCourse(int id)
        {
            return db.Courses.Where(c => c.Id == id).FirstOrDefault();
        }

        public ICollection<ApplicationUser> FindParticipants(int id)
        {
            Course course = null;
            if (id > 0)
            {
                course = FindCourse(id);
            }
            course = (course != null ? course : new Course());
            return (course.Users != null ? course.Users : new List<ApplicationUser>());
        }

        // GET: Student
        [Authorize(Roles = "Student")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Student")]
        public ActionResult Participants()
        {
            Course course = FindCourse(FindUser());
            course.Users = (course.Users != null ? course.Users : new List<ApplicationUser>());
            int id = (course != null ? course.Id : 0);

            if (id <= 0)
            {
                ViewBag.Warning = "The ID of the Course was not able to be found";
            }

            return View(course);
        }
    }
}