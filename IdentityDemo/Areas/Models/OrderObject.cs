using IdentityDemo.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace IdentityDemo.Areas.Models
{
    public partial class OrderObject
    {
        [Key]
        [Column(TypeName = ("uniqueidentifier"))]
        public Guid OrderId { get; set; }
        [Required]
        [Column(TypeName = ("datetime"))]
        public DateTime OrderDate { get; set; }
        [Column(TypeName = ("datetime"))]
        public DateTime? RequiredDate { get; set; }
        [Column(TypeName = ("datetime"))]
        public DateTime? ShippedDate { get; set; }
        [Column(TypeName = ("money"))]
        public decimal? Freight { get; set; } = 0;
        [Required]
        [Column(TypeName = ("nvarchar"))]
        [StringLength(450)]
        public string UserId { get; set; }
        public virtual AppUsers User { get; set; }
        public virtual ICollection<OrderDetailObject> OrderDetails { get; set; }
    }
}
