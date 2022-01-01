using IdentityDemoAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.DTOs
{
    public class UserDTO
    {
        public string UserId { set; get; }
        public string UserName { set; get; }
        public string Email { set; get; }
        public string PhoneNumber { set; get; }
        public string Address { set; get; }
        public bool IsActive { set; get; }
        public ICollection<AppUserRole> UserRoles { set; get; }
    }
}
