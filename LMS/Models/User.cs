using LMS.Models.DataAccess;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace LMS.Models
{
    public class User : ApplicationUser
    {
        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Display(Name = "Efternamn")]
        public string LastName { get; set; }
        
        public int? CoursesId { get; set; } //Foreign Key, Nullable
        public virtual Course Courses { get; set; } //Ovanstående Foreign Key kopplas via denna virtual property 

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public bool IsTeacher()
        {
            IdentityRole teacher = new ApplicationDbContext().Roles.FirstOrDefault(r => r.Name == "Teacher");

            return (this.Roles.FirstOrDefault(r => r.RoleId == teacher.Id) != null);
        }

        public bool IsStudent()
        {
            IdentityRole student = new ApplicationDbContext().Roles.FirstOrDefault(r => r.Name == "Student");

            return (this.Roles.FirstOrDefault(r => r.RoleId == student.Id) != null);

        }
    }
}