namespace LMS.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
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
            var uStore = new UserStore<ApplicationUser>(context);
            var uManager = new UserManager<ApplicationUser>(uStore);
            var rStore = new RoleStore<IdentityRole>(context);
            var rManager = new RoleManager<IdentityRole>(rStore);

            var users = new ApplicationUser[] {
                new ApplicationUser() {
                    Email = "castell_john@hotmail.com", UserName = "kaffeutvecklare"
                },
                new ApplicationUser()
                {
                    Email = "admin@mail.nu", UserName = "admin"
                }
            };
            foreach (ApplicationUser _user in users)
            {
                uManager.Create(_user, "password");
            }

            foreach (var roleName in new[] { "Teacher", "Student" })
            {
                var role = new IdentityRole { Name = roleName };
                rManager.Create(role);
            }

            ApplicationUser user = uManager.FindByName("kaffeutvecklare");
            uManager.AddToRole(user.Id, "Student");
            uManager.Update(user);

            user = uManager.FindByName("admin");
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
