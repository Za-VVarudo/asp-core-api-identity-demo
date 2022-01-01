using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Models
{
    public class AuthResponse
    {
        public bool Success { set; get; }
        public string Error { set; get; }
        public string Token { set; get; }
        public string RefreshToken { set; get; }
    }
}
