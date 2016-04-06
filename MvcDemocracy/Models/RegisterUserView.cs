using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcDemocracy.Models
{
    public class RegisterUserView : UserView
    {
        [Display(Name ="Password")]
        [Required(ErrorMessage = "The field {0} is required.")]
        [StringLength(20, ErrorMessage = "The field {0} can contain {1} and minimum {2} caracters", MinimumLength = 8 )]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "The field {0} is required.")]
        [StringLength(20, ErrorMessage = "The field {0} can contain {1} and minimum {2} caracters", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirm does not match.")]
        public string ConfirmPassword { get; set; }
    }
}