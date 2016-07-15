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
                    Email = "ari.kylmanen@comhem.se", UserName = "ari.kylmanen@comhem.se", FirstName = "Ari", LastName = "Kylm�nen"
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

            // utg�gna kursen .NET
            Course Course = new Course { Name = ".NET, V�ren -15", Description = "En utbildning i .NET C#, .NET MVC5, Bootstrap, AngularJS, etc. etc.", StartDate = new DateTime(2015, 2, 16), EndDate = new DateTime(2015, 8, 15, 23, 59, 0), Users = new List<User>() };
            context.Courses.AddOrUpdate(x => x.Name,
                Course);
            context.SaveChanges();

            // utg�gna kursen Java
            Course = new Course { Name = "Java, V�ren -15", Description = "En utbildning i Java, Spring-framework MVC, Bootstrap, AngularJS, etc. etc.", StartDate = new DateTime(2015, 2, 3), EndDate = new DateTime(2015, 8, 19, 23, 59, 0), Users = new List<User>() };
            context.Courses.AddOrUpdate(x => x.Name,
                Course);
            context.SaveChanges();

            // "p�g�ende" kursen .NET
            Course = new Course { Name = ".NET, V�ren -16", Description = "En utbildning i .NET C#, .NET MVC5, Bootstrap, AngularJS, etc. etc.", StartDate = new DateTime(2016, 2, 16), EndDate = new DateTime(2016, 8, 15, 23, 59, 0), Users = new List<User>() };
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
                new User { Email = "andrea@skola.se", UserName = "andrea@skola.se", FirstName = "Andrea", LastName = "Lindstr�m" },
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
            context.Courses.AddOrUpdate(x => x.Name,
                Course);
            context.SaveChanges();

            Module Module = new Module { Name = "C#", Description = "Learning C#", StartDate = new DateTime(2016, 3, 2), EndDate = new DateTime(2016, 3, 28, 23, 59, 0), CourseId = Course.Id };
            Module.CourseId = Course.Id;
            context.Modules.AddOrUpdate(m => m.Name,
                Module);
            context.SaveChanges();

            // �ndrade i Seed:en h�r s� att aktiviteterna ligger mer i "synch" till vilka moduler dom tillh�r

            Activity Activity = new Activity { Name = "Pluralsight ( .NET )", Description = "Se videos av XXX XXX p� �mnet .NET Framework", StartDate = null, EndDate = null, Deadline = Module.EndDate, ModuleId = Module.Id, Type = "E-learning" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "Pluralsight ( C# 6 )", Description = "Se videos av XXX XXX p� �mnet C# 6.0", StartDate = null, EndDate = null, Deadline = Module.EndDate, ModuleId = Module.Id, Type = "E-learning" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "�va p� C#", Description = "C#-�vningar", StartDate = new DateTime(2016, 3, 28, 8, 30, 0), EndDate = new DateTime(2016, 3, 28, 17, 0, 0), Deadline = new DateTime(2016, 3, 28, 17, 0, 0), ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            context.SaveChanges();

            Module = new Module { Name = "SQL Server", Description = "Learning how MS SQL Server works", StartDate = new DateTime(2016, 3, 1), EndDate = new DateTime(2016, 5, 13, 23, 59, 0), CourseId = Course.Id };
            Module.CourseId = Course.Id;
            context.Modules.AddOrUpdate(m => m.Name,
                Module);
            context.SaveChanges();

            Activity = new Activity { Name = "�va p� SQL / Learning SQL", Description = "C#-�vningar", StartDate = null, EndDate = null, Deadline = new DateTime(2016, 5, 13, 17, 0, 0), ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            context.SaveChanges();

            Module = new Module { Name = "Identity", Description = "G�r igenom Identity och Individual User Interface(?) med g�stf�rel�sare", StartDate = new DateTime(2016, 5, 16), EndDate = new DateTime(2016, 5, 18, 23, 59, 0), CourseId = Course.Id };
            Module.CourseId = Course.Id;
            context.Modules.AddOrUpdate(m => m.Name,
                Module);

            Activity = new Activity { Name = "F�rel�sning om Identity", Description = "Identity-f�rel�sning", StartDate = new DateTime(2016, 5, 16), EndDate = new DateTime(2016, 5, 17, 17, 30, 0), Deadline = null, ModuleId = Module.Id, Type = "Lecture" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "Skapa project med Individual User Interface", Description = "Identity-�vningar", StartDate = null, EndDate = null, Deadline = null, ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            context.SaveChanges();

            Module = new Module { Name = "�vningstillf�llen", Description = "�va p� diverse", StartDate = new DateTime(2016, 2, 16), EndDate = new DateTime(2016, 8, 15, 23, 59, 0), CourseId = Course.Id };
            Module.CourseId = Course.Id;
            context.Modules.AddOrUpdate(m => m.Name,
                Module);
            context.SaveChanges();

            Activity = new Activity { Name = "�vning i HTML 4.1", Description = "HTML 4.1 �vning. Skapa en enkel sida", StartDate = null, EndDate = new DateTime(2016, 4, 18, 17, 0, 0), Deadline = new DateTime(2016, 4, 18, 15, 30, 0), ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "CSS 3", Description = "�vningar i CSS3", StartDate = null, EndDate = new DateTime(2016, 6, 21, 17, 0, 0), Deadline = new DateTime(2016, 6, 21, 15, 30, 0), ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "JavaScript", Description = "En enkel applikation som skriver ut text i kommando-prompten", StartDate = null, EndDate = new DateTime(2016, 7, 2, 17, 0, 0), Deadline = new DateTime(2016, 7, 2, 15, 30, 0), ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            context.SaveChanges();

            Module = new Module { Name = "Slutprojekt", Description = "Jobba p� slutprojekt i grupp", StartDate = new DateTime(2016, 7, 20), EndDate = new DateTime(2016, 8, 10, 23, 59, 0), CourseId = Course.Id };
            Module.CourseId = Course.Id;
            context.Modules.AddOrUpdate(m => m.Name,
                Module);
            context.SaveChanges();

            Activity = new Activity { Name = "Sprint 1", Description = "F�rsta sprinten i SCRUM-arbetet f�r slutprojektet", StartDate = new DateTime(2016, 7, 20), EndDate = new DateTime(2016, 7, 27, 17, 0, 0), Deadline = null, ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "Sprint 2", Description = "Andra sprinten i SCRUM-arbete f�r slutprojektet", StartDate = new DateTime(2016, 7, 28), EndDate = new DateTime(2016, 8, 5, 17, 0, 0), Deadline = null, ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "Sprint 3", Description = "Tredje sprinten i SCRUM-arbete f�r slutprojektet", StartDate = new DateTime(2016, 8, 6), EndDate = new DateTime(2016, 8, 10, 17, 0, 0), Deadline = null, ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "Slutprojekt", Description = "En inl�mningsuppgift f�r slutprojektet", StartDate = null, EndDate = null, Deadline = new DateTime(2016, 8, 10, 17, 0, 0), ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            context.SaveChanges();

            context.Courses.AddOrUpdate(x => x.Name,
                Course);

            // "p�g�ende" kursen Java
            Course = new Course { Name = "Java, V�ren -16", Description = "En utbildning i Java, Spring-framework, Bootstrap, AngularJS, etc. etc.", StartDate = new DateTime(2016, 2, 13), EndDate = new DateTime(2016, 8, 15, 17, 59, 0), Users = new List<User>() };
            context.Courses.AddOrUpdate(x => x.Name,
                Course);

            foreach (User _user in new User[] {
                new User { Email = "tommy@castell.se", UserName = "tommy@castell.se", FirstName = "Tommy", LastName = "Castell" },
                new User { Email = "monica@castell.se", UserName = "monica@castell.se", FirstName = "Monica", LastName = "Castell" },
                new User { Email = "veronica.brown@sheff.uni.co.uk", UserName = "veronica.brown@sheff.uni.co.uk", FirstName = "Veronica", LastName = "Brown" },
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

            Module = new Module { Name = "Java", Description = "Learning Java", StartDate = new DateTime(2016, 3, 2), EndDate = new DateTime(2016, 3, 28, 23, 59, 0), CourseId = Course.Id };
            Module.CourseId = Course.Id;
            context.Modules.AddOrUpdate(m => m.Name,
                Module);
            context.SaveChanges();

            // �ndrade i Seed:en h�r s� att aktiviteterna ligger mer i "synch" till vilka moduler dom tillh�r

            Activity = new Activity { Name = "Pluralsight ( Java 8 )", Description = "Se videos av XXX XXX p� �mnet Java", StartDate = null, EndDate = null, Deadline = Module.EndDate, ModuleId = Module.Id, Type = "E-learning" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "Pluralsight ( Differences in Java 7 - Java 8 )", Description = "Se videos av XXX XXX p� �mnet 'skillnader mellan Jav 7 och Java 8'", StartDate = null, EndDate = null, Deadline = Module.EndDate, ModuleId = Module.Id, Type = "E-learning" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "�va p� Java", Description = "Java-�vningar", StartDate = new DateTime(2016, 3, 28, 8, 30, 0), EndDate = new DateTime(2016, 3, 28, 17, 0, 0), Deadline = new DateTime(2016, 3, 28, 17, 0, 0), ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            context.SaveChanges();

            Module = new Module { Name = "MySQL", Description = "Learning how MySQL works", StartDate = new DateTime(2016, 3, 1), EndDate = new DateTime(2016, 5, 13, 23, 59, 0), CourseId = Course.Id };
            Module.CourseId = Course.Id;
            context.Modules.AddOrUpdate(m => m.Name,
                Module);
            context.SaveChanges();

            Activity = new Activity { Name = "Learning SQL", Description = "SQL-�vningar", StartDate = null, EndDate = null, Deadline = new DateTime(2016, 5, 13, 17, 0, 0), ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            context.SaveChanges();

            Module = new Module { Name = "Inloggning", Description = "G�r igenom hur man loggar in och authenticeras anv�ndare mot tredje parts inloggningar", StartDate = new DateTime(2016, 5, 16), EndDate = new DateTime(2016, 5, 18, 23, 59, 0), CourseId = Course.Id };
            Module.CourseId = Course.Id;
            context.Modules.AddOrUpdate(m => m.Name,
                Module);

            Activity = new Activity { Name = "F�rel�sning om social media API", Description = "API-f�rel�sning", StartDate = new DateTime(2016, 5, 16), EndDate = new DateTime(2016, 5, 17, 17, 30, 0), Deadline = null, ModuleId = Module.Id, Type = "Lecture" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "Skapa ett project f�r att h�mta information fr�n Facebook", Description = "H�mta social media information via ett �ppet API", StartDate = null, EndDate = null, Deadline = null, ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            context.SaveChanges();

            Module = new Module { Name = "�vningstillf�llen ( Java -16 ) ", Description = "�va p� diverse", StartDate = new DateTime(2016, 2, 16), EndDate = new DateTime(2016, 8, 15, 23, 59, 0), CourseId = Course.Id };
            Module.CourseId = Course.Id;
            context.Modules.AddOrUpdate(m => m.Name,
                Module);
            context.SaveChanges();

            Activity = new Activity { Name = "�vning i HTML 5", Description = "HTML 5-�vning. Skapa en enkel sida", StartDate = null, EndDate = new DateTime(2016, 4, 18, 17, 0, 0), Deadline = new DateTime(2016, 4, 18, 15, 30, 0), ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "�vningar inom CSS 3", Description = "�vningar i CSS3", StartDate = null, EndDate = new DateTime(2016, 6, 21, 17, 0, 0), Deadline = new DateTime(2016, 6, 21, 15, 30, 0), ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "JavaScript / jQuery", Description = "En enkel applikation som h�mtar en HTML-node och modifierar text", StartDate = null, EndDate = new DateTime(2016, 7, 2, 17, 0, 0), Deadline = new DateTime(2016, 7, 2, 15, 30, 0), ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            context.SaveChanges();

            Module = new Module { Name = "Slutprojekt ( Java -16 )", Description = "Jobba p� slutprojekt i grupp", StartDate = new DateTime(2016, 7, 20), EndDate = new DateTime(2016, 8, 10, 23, 59, 0), CourseId = Course.Id };
            Module.CourseId = Course.Id;
            context.Modules.AddOrUpdate(m => m.Name,
                Module);
            context.SaveChanges();

            Activity = new Activity { Name = "Sprint 1 ( Java -16 )", Description = "F�rsta sprinten i SCRUM-arbetet f�r slutprojektet", StartDate = new DateTime(2016, 7, 20), EndDate = new DateTime(2016, 7, 27, 17, 0, 0), Deadline = null, ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "Sprint 2 ( Java -16 )", Description = "Andra sprinten i SCRUM-arbete f�r slutprojektet", StartDate = new DateTime(2016, 7, 28), EndDate = new DateTime(2016, 8, 5, 17, 0, 0), Deadline = null, ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "Sprint 3 ( Java -16 )", Description = "Tredje sprinten i SCRUM-arbete f�r slutprojektet", StartDate = new DateTime(2016, 8, 6), EndDate = new DateTime(2016, 8, 10, 17, 0, 0), Deadline = null, ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            Activity = new Activity { Name = "Slutprojekt ( Java -16 )", Description = "En inl�mningsuppgift f�r slutprojektet", StartDate = null, EndDate = null, Deadline = new DateTime(2016, 8, 10, 17, 0, 0), ModuleId = Module.Id, Type = "Practice" };
            context.Activities.AddOrUpdate(x => x.Name,
                Activity);
            context.SaveChanges();

            context.Courses.AddOrUpdate(x => x.Name,
                Course);

            // loopar alla filer i "~/documents/module/-1/"
            foreach (string file in Directory.GetFiles(MapPath("~/documents/module/-1/").Replace("%20", " ")))
            {
                // h�mtar ut vilken extention det �r, t.ex. .txt
                string extention = System.IO.Path.GetExtension(file);
                // h�mtar ut titel i dokument, t.ex. "teext"
                string name = System.IO.Path.GetFileNameWithoutExtension(file);
                // sparar filen p� disk, samt f� t.ex. ett dokument att spara i databas
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
                    //metadata tillagd i databasen
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

            user = uManager.FindByName("lozano@skola.se");      //Adrian Lozano �r en av l�rarna och f�r h�r tilldelad en l�rarroll. 
            uManager.AddToRole(user.Id, "Teacher");
            uManager.Update(user);

            user = uManager.FindByName("jakobsson@skola.se");   //Oscar Jacobsson �r en annan l�rare, som ocks� f�r en l�rarroll tilldelad h�r. 
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
