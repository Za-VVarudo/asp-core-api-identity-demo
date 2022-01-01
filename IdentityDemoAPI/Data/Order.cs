using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Data
{
    public class Order
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid Id { set; get; }
        [Required]
        [Column(TypeName = "nvarchar(450)")]
        public string UserId { set; get; }
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { set; get; } = DateTime.Now;
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime ShippedDate { set; get; }
        [Required]
        [Column(TypeName = "money")]
        public decimal ShippingFee { set; get; }
        [Required]
        [Column(TypeName = "money")]
        public decimal Total { set; get; }
        public AppUser User { set; get; }

        public ICollection<OrderDetail> OrderDetails { set; get; }
    }
}
