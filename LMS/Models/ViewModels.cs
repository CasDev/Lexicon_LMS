using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
    public class CreateUserViewModel
    {
        [Required( ErrorMessage = "Ett förnamn behövs på en användare" )]
        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Ett efternamn behövs på en användare")]
        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Ett lösenord behövs till en användare")]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Ett lösenord behövs till en användare")]
        [Display(Name = "Konfirmera lösenord")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "En roll behövs till en användare")]
        [Display(Name = "Roll")]
        public string Role { get; set; }

        [Required(ErrorMessage = "En e-post måste fyllas i")]
        [Display(Name = "Login")]
        [EmailAddress(ErrorMessage = "E-post har felaktigt format")]
        public string Email { get; set; }
    }

    public class AddUserToCourseViewModel
    {

    }

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

    public class EditCourseViewModel
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

    public class EditModuleViewModel
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

    public class CreateActivityViewModel
    {
        [Display(Name = "Typ")]
        [Required (ErrorMessage = "Kurstyp")]
        public string Type { get; set; }            //Marie 

        [Display(Name = "Namn")]
        [Required(ErrorMessage = "Kursnamn")]
        public string Name { get; set; }            //Marie 

        [Display(Name = "Beskrivning")]
        [Required(ErrorMessage = "Kursbeskrivning")]
        public string Description { get; set; }     //Marie

        [Display(Name = "Startdatum")]              //Marie
        [Required(ErrorMessage = "Kursdatum")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Slutdatum")]               //Marie 
        [Required (ErrorMessage = "Slutdatum")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Inlämningsdatum")]         //Marie 
        [Required(ErrorMessage = "Inlämningsdatum")]
        public DateTime? Deadline { get; set; }

        [Required]
        public int? ModuleId { get; set; }
    }

    public class EditActivityViewModel
    {
        [Display(Name = "Namn")]
        [Required(ErrorMessage = "Nytt namn")]
        public string Name { get; set; }            //Marie 

        [Display(Name = "Beskrivning")]
        [Required(ErrorMessage = "Ny beskrivning")]
        public string Description { get; set; }     //Marie

        [Display(Name = "Startdatum")]              //Marie
        [Required (ErrorMessage = "Nytt startdatum")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Slutdatum")]               //Marie 
        [Required (ErrorMessage = "Nytt slutdatumdatum")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Inlämningsdatum")]         //Marie 
        [Required (ErrorMessage = "Nytt inlämningsdatum")] //Marie
        public DateTime? Deadline { get; set; }
    }

    public class DeleteActivityViewModel
    {
          public bool Confirm { get; set; }
    }
}
