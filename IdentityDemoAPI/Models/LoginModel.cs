using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { set; get; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { set; get; }
    }
}
