using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
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
