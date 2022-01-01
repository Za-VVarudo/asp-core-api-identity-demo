using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace IdentityDemoAPI.Data
{
    // Add profile data for application users by adding properties to the AppUser class
    public class AppUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(200)")]
        public string Address { set; get; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { set; get; }
        [Column(TypeName = "bit")]
        public bool IsActive { set; get; }
        public ICollection<AppUserRole> UserRoles { set; get; }
        public ICollection<Order> Orders { set; get; }
    }
}
