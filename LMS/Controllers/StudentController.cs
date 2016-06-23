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

        public List<ApplicationUser> FindParticipants(int id)
        {
            List<ApplicationUser> participants = new List<ApplicationUser>();
            if (id > 0)
            {
                // TODO: Load all
            }
            return participants;
        }

        // GET: Student
        [Authorize(Roles = "Student")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Student")]
        public ActionResult Participants(string id)
        {
            if (id == null)
            {
                ApplicationUser user = FindUser();
                // TODO: instead of zero, find the id of the course
                id = "0";
            }
            int _id = 0;
            Int32.TryParse(id, out _id);
            if (_id <= 0)
            {
                ViewBag.Warning = "The ID of the Course was not able to be found";
            }

            return View(FindParticipants(_id));
        }
    }
}