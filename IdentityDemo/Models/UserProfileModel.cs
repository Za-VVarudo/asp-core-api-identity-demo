using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Models
{
    public class UserProfileModel
    {
        [Required]
        [StringLength(100)]
        [Display( Name = "Full Name")]
        public string FullName { set; get; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Address")]
        public string Address { set; get; }
        [Required]
        [StringLength(50)]
        [Display(Name = "City")]
        public string City { set; get; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Country")]
        public string Country { set; get; }
    }
}
