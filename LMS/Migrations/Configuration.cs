namespace LMS.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Hosting;
    internal sealed class Configuration : DbMigrationsConfiguration<LMS.Models.DataAccess.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(LMS.Models.DataAccess.ApplicationDbContext context)
        {
            var uStore = new UserStore<User>(context);
            var uManager = new UserManager<User>(uStore);
            var rStore = new RoleStore<IdentityRole>(context);
            var rManager = new RoleManager<IdentityRole>(rStore);

            var users = new User[] {
                new User {
                    Email = "castell_john@hotmail.com", UserName = "castell_john@hotmail.com", FirstName = "John", LastName = "Castell"
                },
                new User {
                    Email = "ari.kylmanen@comhem.se", UserName = "ari.kylmanen@comhem.se", FirstName = "Ari", LastName = "Kylmänen"
                },
                new User {
                    Email = "mariehansson10@hotmail.com", UserName = "mariehansson10@hotmail.com", FirstName = "Marie", LastName = "Hansson"
                },
                new User {
                    Email = "kaffeutvecklare@gmail.com", UserName = "kaffeutvecklare@gmail.com", FirstName = "John", LastName = "Castell"
                },
                new User
                {
                    Email = "admin@mail.nu", UserName = "admin@mail.nu", FirstName = "Admin", LastName = "Adminsson"
                }
            };
            foreach (User _user in users)
            {
                uManager.Create(_user, "password");
            }

            foreach (var roleName in new[] { "Teacher", "Student" })
            {
                var role = new IdentityRole { Name = roleName };
                rManager.Create(role);
            }

            User user = uManager.FindByName("castell_john@hotmail.com");
            uManager.AddToRole(user.Id, "Student");
            uManager.Update(user);
            user = uManager.FindByName("ari.kylmanen@comhem.se");
            uManager.AddToRole(user.Id, "Student");
            uManager.Update(user);
            user = uManager.FindByName("mariehansson10@hotmail.com");
            uManager.AddToRole(user.Id, "Student");
            uManager.Update(user);

            Course Course = new Course { Name = ".NET, Våren -16", Description = "En utbildning i .NET C#, .NET MVC5, Bootstrap, AngularJS, etc. etc.", StartDate = new DateTime(2016, 2, 16), EndDate = new DateTime(2016, 8, 15), Users = new List<User>() };
            Course.Users.Add(user);
            user = uManager.FindByName("castell_john@hotmail.com");
            Course.Users.Add(user);
            if (user.CoursesId == null || user.CoursesId <= 0)
            {
                user.CoursesId = Course.Id;
                context.Users.AddOrUpdate(u => u.UserName, user);
            }
            user = uManager.FindByName("ari.kylmanen@comhem.se");
            Course.Users.Add(user);
            if (user.CoursesId == null || user.CoursesId <= 0)
            {
                user.CoursesId = Course.Id;
                context.Users.AddOrUpdate(u => u.UserName, user);
            }
            user = uManager.FindByName("mariehansson10@hotmail.com");
            Course.Users.Add(user);
            if (user.CoursesId == null || user.CoursesId <= 0)
            {
                user.CoursesId = Course.Id;
                context.Users.AddOrUpdate(u => u.UserName, user);
            }
            context.Courses.AddOrUpdate(x => x.Name,
                Course);
            context.SaveChanges();

            foreach (User _user in new User[] {
                new User { Email = "example@mail.nu", UserName = "example@mail.nu", FirstName = "förnamn", LastName = "Efternamn" },
                new User { Email = "sebcas@live.se", UserName = "sebcas@live.se", FirstName = "Exempel", LastName = "Jonsson" },
                new User { Email = "anexample@mail.nu", UserName = "anexample@mail.nu", FirstName = "Firstname", LastName = "Lastname" },
            })
            {
                uManager.Create(_user, "password");
                user = uManager.FindByName(_user.UserName);

                uManager.AddToRole(user.Id, "Student");
                Course.Users.Add(user);
                if (user.CoursesId == null || user.CoursesId <= 0)
                {
                    user.CoursesId = Course.Id;
                    context.Users.AddOrUpdate(u => u.UserName, user);
                }
            }
            
            Module Module = new Module { Name = "C#", Description = "Learning C#", StartDate = new DateTime(2016, 3, 2), EndDate = new DateTime(2016, 3, 28) };
            Module.CourseId = Course.Id;
            context.Modules.AddOrUpdate(m => m.Name,
                Module);
            Module = new Module { Name = "SQL Server", Description = "Learning how MS SQL Server works", StartDate = new DateTime(2016, 3, 1), EndDate = new DateTime(2016, 5, 13) };
            Module.CourseId = Course.Id;
            context.Modules.AddOrUpdate(m => m.Name,
                Module);
            Module = new Module { Name = "SQL", Description = "Learning SQL", StartDate = new DateTime(2016, 3, 1), EndDate = new DateTime(2016, 3, 10) };
            Module.CourseId = Course.Id;
            context.Modules.AddOrUpdate(m => m.Name,
                Module);
            Module = new Module { Name = "Entity Framework", Description = "Working skills with Entity Framework", StartDate = new DateTime(2016, 3, 16), EndDate = new DateTime(2016, 4, 18) };
            Module.CourseId = Course.Id;
            context.Modules.AddOrUpdate(m => m.Name,
                Module);
            Module = new Module { Name = "Övningstillfällen", Description = "Öva på diverse", StartDate = new DateTime(2016, 2, 16), EndDate = new DateTime(2016, 8, 15) };
            Module.CourseId = Course.Id;
            context.Modules.AddOrUpdate(m => m.Name,
                Module);
            context.SaveChanges();

            //Här börjar aktiviteterna seedas: 
            
            //Detta är Aktivitet nr 1:
            Module = context.Modules.Where(m => m.Name == "Entity Framework").FirstOrDefault();
            Activity Activity = new Activity { Name = "Pluralsight", Description = "Se videos av Anton X på ämnet Entity Framework", StartDate = null, EndDate = null, Deadline = Module.EndDate, ModuleId = Module.Id, Type = "E-learning" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);

            //Detta är Aktivitet nr 2: 
            Activity = new Activity { Name = "Adrians Entity Framework", Description = "Föreläsning av Adrian", StartDate = new DateTime(2016, 3, 16, 10, 0, 0), EndDate = new DateTime(2016, 3, 16, 17, 0, 0), Deadline = null, ModuleId = Module.Id, Type = "Lecture" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);

            //Detta är Aktivitet nr 3: 
            Module = context.Modules.Where(m => m.Name == "Övningstillfällen").FirstOrDefault();
            Activity = new Activity { Name = "Öva på HTML 4.1", Description = "HTML-övningar", StartDate = new DateTime(2016, 5, 10, 8, 30, 0), EndDate = new DateTime(2016, 8, 15, 17, 0, 0), Deadline = new DateTime(2016, 8, 16, 15, 0, 0), ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);

            context.Courses.AddOrUpdate(x => x.Name,
                Course);

            foreach (string file in Directory.GetFiles(MapPath("~/documents/modules/-1/").Replace("%20", " "))) {
                string extention = System.IO.Path.GetExtension(file);
                string name = System.IO.Path.GetFileNameWithoutExtension(file);
                Document Document = DocumentCRUD.SaveDocument(MapPath("~/documents/modules/5/").Replace("%20", " "), name, extention, File.ReadAllBytes(file));
                if (Document != null)
                {
                    Document.Name = name;
                    Document.Description = name;
                    Document.UploadTime = DateTime.Now;
                    Document.ActivityId = null;
                    Document.CourseId = null;
                    Document.ModifyUserId = null;
                    Document.ModuleId = 5;
                    Document.UserId = user.Id;
                    context.Documents.AddOrUpdate(d => d.Name, Document);
                }
                Document = DocumentCRUD.SaveDocument(MapPath("~/documents/course/1/").Replace("%20", " "), name, extention, File.ReadAllBytes(file));
                if (Document != null)
                {
                    Document.Name = name;
                    Document.Description = name;
                    Document.UploadTime = DateTime.Now;
                    Document.ActivityId = null;
                    Document.CourseId = 1;
                    Document.ModifyUserId = null;
                    Document.ModuleId = null;
                    Document.UserId = user.Id;
                    context.Documents.AddOrUpdate(d => d.Name, Document);
                }
                Document = DocumentCRUD.SaveDocument(MapPath("~/documents/activity/3/").Replace("%20", " "), name, extention, File.ReadAllBytes(file));
                if (Document != null)
                {
                    Document.Name = name;
                    Document.Description = name;
                    Document.UploadTime = DateTime.Now;
                    Document.ActivityId = 3;
                    Document.CourseId = null;
                    Document.ModifyUserId = null;
                    Document.ModuleId = null;
                    Document.UserId = user.Id;
                    context.Documents.AddOrUpdate(d => d.Name, Document);
                }
            }

            user = uManager.FindByName("admin@mail.nu");
            uManager.AddToRole(user.Id, "Teacher");
            uManager.Update(user);
        }

        private string MapPath(string seedFile)
        {
            if (HttpContext.Current != null)
                return HostingEnvironment.MapPath(seedFile);

            var absolutePath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            var directoryName = Path.GetDirectoryName(absolutePath);
            var path = Path.Combine(directoryName, ".." + seedFile.TrimStart('~').Replace('/', '\\'));

            return path;
        }
    }
}
