using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Data
{
    public class OrderDetail
    {
        [Column(TypeName = "uniqueidentifier")]
        public Guid OrderId { set; get; }
        [Column(TypeName = "uniqueidentifier")]
        public Guid BookId { set; get; }
        [Required]
        [Column(TypeName = "int")]
        public int Quantity { set; get; }
        [Required]
        [Column(TypeName = "money")]
        public decimal CurrentPrice { set; get; }
        [Required]
        [Column(TypeName = "money")]
        public decimal Total { set; get; }

        public Order Order { set; get; }
        public Book Book { set; get; }
    }
}
