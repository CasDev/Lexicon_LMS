namespace LMS.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

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

            Course Course = new Course { Name = ".NET, Våren -16", Description = "En utbildning i .NET C#, .NET MVC5, Bootstrap, AngularJS, etc. etc.", StartDate = new DateTime(2016, 2, 16), EndDate = new DateTime(2016, 8, 15), Users = new List<User>() };
            Course.Users.Add(user);
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
            }

            // TODO: add modules
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
            context.SaveChanges();

            context.Courses.AddOrUpdate(x => x.Name,
                Course);

            user = uManager.FindByName("admin@mail.nu");
            uManager.AddToRole(user.Id, "Teacher");
            uManager.Update(user);
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
