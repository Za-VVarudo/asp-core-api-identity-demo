using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace IdentityDemo.Areas.Models
{
    public partial class OrderDetailObject
    {
        [Key]
        [Column(TypeName = ("uniqueidentifier"))]
        public Guid OrderId { get; set; }
        [Key]
        [Column(TypeName = ("int"))]
        public int ProductId { get; set; }
        [Required]
        [Column(TypeName = ("money"))]
        public decimal UnitPrice { get; set; }
        [Required]
        [Column(TypeName = ("int"))]
        public int Quantity { get; set; }
        [Required]
        [Column(TypeName = ("float"))]
        public double Discount { get; set; }

        public virtual OrderObject Order { get; set; }
        public virtual ProductObject Product { get; set; }
    }
}
