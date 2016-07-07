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
            return View();
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
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateActivity(CreateActivityViewModel model)
        {
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

        // GET: Teacher
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Courses);
            return View(users.ToList());
        }

        // GET: Teacher/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Teacher/Create
        public ActionResult Create()
        {
            ViewBag.CoursesId = new SelectList(db.Courses, "Id", "Name");
            return View();
        }

        // POST: Teacher/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,CoursesId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CoursesId = new SelectList(db.Courses, "Id", "Name", user.CoursesId);
            return View(user);
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


        // GET: Teacher/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.CoursesId = new SelectList(db.Courses, "Id", "Name", user.CoursesId);
            return View(user);
        }


        // POST: Teacher/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,CoursesId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CoursesId = new SelectList(db.Courses, "Id", "Name", user.CoursesId);
            return View(user);
        }

        // GET: Teacher/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Teacher/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
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
