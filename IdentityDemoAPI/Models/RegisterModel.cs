using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Models
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { set; get; }
        [Required]
        [StringLength(50, ErrorMessage = " {0} Must be from {2} to {1} characters "), MinLength(6)]
        [DataType(DataType.Password)]
        public string Password { set; get; }
        [Required]
        [StringLength(200, ErrorMessage = " {0} Must be at most {1} characters ")]
        [Display(Name = "Address")]
        public string Address { set; get; }

    }
}
