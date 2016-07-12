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
                new User {
                    Email = "lozano@skola.se", UserName = "lozano@skola.se", FirstName = "Adrian", LastName = "Lozano"
                },
                new User {
                    Email = "jakobsson@skola.se", UserName = "jakobsson@skola.se", FirstName = "Oscar", LastName = "Jakobsson"
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
                new User { Email = "thyrhaug@skola.se", UserName = "thyrhaug@skola.se", FirstName = "Andreas", LastName = "Thyrhaug" },
                new User { Email = "jonsson@skola.se", UserName = "jonsson@skola.se", FirstName = "Thomas", LastName = "Jonsson" },
                new User { Email = "andrea@skola.se", UserName = "andrea@skola.se", FirstName = "Andrea", LastName = "Lindström" },
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
            
            Module Module = new Module { Name = "C#", Description = "Learning C#", StartDate = new DateTime(2016, 3, 2), EndDate = new DateTime(2016, 3, 28, 23, 59, 0) };
            Module.CourseId = Course.Id;
            context.Modules.AddOrUpdate(m => m.Name,
                Module);
            context.SaveChanges();

            // Ändrade i Seed:en här så att aktiviteterna ligger mer i "synch" till vilka moduler dom tillhör

            Activity Activity = new Activity { Name = "Pluralsight ( .NET )", Description = "Se videos av XXX XXX på ämnet .NET Framework", StartDate = null, EndDate = null, Deadline = Module.EndDate, ModuleId = Module.Id, Type = "E-learning" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "Pluralsight ( C# 6 )", Description = "Se videos av XXX XXX på ämnet C# 6.0", StartDate = null, EndDate = null, Deadline = Module.EndDate, ModuleId = Module.Id, Type = "E-learning" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "Öva på C#", Description = "C#-övningar", StartDate = new DateTime(2016, 3, 28, 8, 30, 0), EndDate = new DateTime(2016, 3, 28, 17, 0, 0), Deadline = new DateTime(2016, 3, 28, 17, 0, 0), ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            context.SaveChanges();

            Module = new Module { Name = "SQL Server", Description = "Learning how MS SQL Server works", StartDate = new DateTime(2016, 3, 1), EndDate = new DateTime(2016, 5, 13, 23, 59, 0) };
            Module.CourseId = Course.Id;
            context.Modules.AddOrUpdate(m => m.Name,
                Module);
            context.SaveChanges();

            Activity = new Activity { Name = "Öva på SQL / Learning SQL", Description = "C#-övningar", StartDate = null, EndDate = null, Deadline = new DateTime(2016, 5, 13, 17, 0, 0), ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            context.SaveChanges();

            // TODO:

            Module = new Module { Name = "Identity", Description = "Går igenom Identity och Individual User Interface(?) med gästföreläsare", StartDate = new DateTime(2016, 5, 16), EndDate = new DateTime(2016, 5, 18, 23, 59, 0) };
            Module.CourseId = Course.Id;
            context.Modules.AddOrUpdate(m => m.Name,
                Module);

            Activity = new Activity { Name = "Föreläsning om Identity", Description = "Identity-föreläsning", StartDate = new DateTime(2016, 5, 16), EndDate = new DateTime(2016, 5, 17, 17, 30, 0), Deadline = null, ModuleId = Module.Id, Type = "Lecture" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "Skapa project med Individual User Interface", Description = "Identity-övningar", StartDate = null, EndDate = null, Deadline = null, ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            context.SaveChanges();

            Module = new Module { Name = "Övningstillfällen", Description = "Öva på diverse", StartDate = new DateTime(2016, 2, 16), EndDate = new DateTime(2016, 8, 15, 23, 59, 0) };
            Module.CourseId = Course.Id;
            context.Modules.AddOrUpdate(m => m.Name,
                Module);
            context.SaveChanges();

            Activity = new Activity { Name = "Övning i HTML 4.1", Description = "HTML 4.1 övning. Skapa en enkel sida", StartDate = null, EndDate = new DateTime(2016, 4, 18, 17, 0, 0), Deadline = new DateTime(2016, 4, 18, 15, 30, 0), ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "CSS 3", Description = "Övningar i CSS3", StartDate = null, EndDate = new DateTime(2016, 6, 21, 17, 0, 0), Deadline = new DateTime(2016, 6, 21, 15, 30, 0), ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "JavaScript", Description = "En enkel applikation som skriver ut text i kommando-prompten", StartDate = null, EndDate = new DateTime(2016, 7, 2, 17, 0, 0), Deadline = new DateTime(2016, 7, 2, 15, 30, 0), ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            context.SaveChanges();

            context.Courses.AddOrUpdate(x => x.Name,
                Course);

            foreach (string file in Directory.GetFiles(MapPath("~/documents/module/-1/").Replace("%20", " "))) {
                string extention = System.IO.Path.GetExtension(file);
                string name = System.IO.Path.GetFileNameWithoutExtension(file);
                Document Document = DocumentCRUD.SaveDocument(MapPath("~/documents/module/3/").Replace("%20", " "), name, extention, File.ReadAllBytes(file));
                if (Document != null)
                {
                    Document.Name = name;
                    Document.Description = name;
                    Document.UploadTime = DateTime.Now;
                    Document.ActivityId = null;
                    Document.CourseId = null;
                    Document.ModifyUserId = null;
                    Document.ModuleId = 3;
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

            user = uManager.FindByName("lozano@skola.se");
            uManager.AddToRole(user.Id, "Teacher");
            uManager.Update(user);

            user = uManager.FindByName("jakobsson@skola.se");
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
