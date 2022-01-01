using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using IdentityDemo.Areas.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityDemo.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the AppUsers class
    public class AppUsers : IdentityUser
    {
        [Required]
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string FullName { set; get; }
        [Required]
        [PersonalData]
        [Column(TypeName = "nvarchar(200)")]
        public string Address { set; get; }
        [Required]
        [PersonalData]
        [Column(TypeName = "nvarchar(50)")]
        public string City { get; set; }
        [Required]
        [PersonalData]
        [Column(TypeName = "nvarchar(50)")]
        public string Country { get; set; }
        [Required]
        [PersonalData]
        [Column(TypeName = "bit")]
        public bool Status { set; get; }
        public virtual ICollection<OrderObject> Orders { get; set; }
    }
}
