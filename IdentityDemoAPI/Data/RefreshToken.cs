using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Data
{
    public class RefreshToken
    {
        [Column(TypeName = "varchar(72)")]
        public string Id { set; get; }
        [Column(TypeName = "uniqueidentifier")]
        public Guid Jti { set; get; }
        [Column(TypeName = "nvarchar(450)")]
        public string UserId { set; get; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { set; get; } = DateTime.UtcNow;
        [Column(TypeName = "datetime")]
        public DateTime ExpiredDate { set; get; }
        [Column(TypeName = "bit")]
        public bool IsUsed { set; get; } = false;
        [Column(TypeName = "bit")]
        public bool IsRevoke { set; get; } = false;

        public AppUser User { set; get; }
    }
}
