using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace IdentityDemo.Areas.Models
{
    public partial class ProductObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = ("int"))]
        public int ProductId { get; set; }
        [Required]
        [Column(TypeName = ("int"))]
        public int CategoryId { get; set; }
        [Required]
        [Column(TypeName = "nvarchar")]
        [StringLength(100)]
        public string ProductName { get; set; }
        [Required]
        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string Weight { get; set; }
        [Required]
        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }
        [Required]
        [Column(TypeName = ("int"))]
        public int UnitsInStock { get; set; }
        
        [Required]
        [Column(TypeName = ("bit"))]
        public bool Status { set; get; } = true;

        public virtual ICollection<OrderDetailObject> OrderDetails { get; set; }
    }
}
