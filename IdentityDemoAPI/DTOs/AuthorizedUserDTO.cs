using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.DTOs
{
    public class AuthorizedUserDTO
    {
        public string UserId { set; get; }
        public string Username { set; get; }
        public string Email { set; get; }
        public string Address {set;get;}
        public string Role { set; get; }
    }
}
