using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Data
{
    public class AppUserRole : IdentityUserRole<string>
    {
        public AppUser User { set; get; }
        public AppRole Role { set; get; }
    }
}
