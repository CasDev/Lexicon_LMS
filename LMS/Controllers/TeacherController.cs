﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.Models;
using LMS.Models.DataAccess;

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
        public ActionResult CreateCourse(FormCollection collection)
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateModule()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateModule(FormCollection collection)
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateActivity()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateActivity(FormCollection Collection)
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
        public ActionResult CreateUser(FormCollection Collection)
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

            return View();
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