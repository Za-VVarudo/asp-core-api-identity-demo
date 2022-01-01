using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Models
{
    public class RefreshTokenRequest
    {
        [Required]
        public string Token { set; get; }
        [Required]
        public string RefreshToken { set; get; }
    }
}
