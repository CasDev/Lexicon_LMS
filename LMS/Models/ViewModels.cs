using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
    public class CreateCourseViewModel
    {
        [Required(ErrorMessage = "Ett namn behövs på en kurs")]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Ett startdatum behövs för en kurs")]
        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ett startdatum behövs för en kurs")]
        [Display(Name = "Slutdatum")]
        public DateTime EndDate { get; set; }
    }

    public class CreateModuleViewModel
    {
        [Required(ErrorMessage = "Ett namn behövs på en modul")]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Ett startdatum behövs på en modul")]
        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ett slutdatum behövs på en modul")]
        [Display(Name = "Slutdatum")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "En modul måste tillhöra en kurs")]
        public int CourseId { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-post måste fyllas i")]
        //[Display(Name = "E-post")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage ="E-post har felaktigt format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lösenord måste fyllas i")]
        [DataType(DataType.Password)]
        //[Display(Name = "Lösenord")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        //[Display(Name = "Remember me?")]
        [Display(Name = "Kom ihåg mig?")]
        public bool RememberMe { get; set; }
    }
}
