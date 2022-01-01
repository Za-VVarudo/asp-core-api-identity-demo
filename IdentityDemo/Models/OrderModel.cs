using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Models
{
    public class OrderModel : OrderUpdateModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }
    }

    public class OrderUpdateModel
    {
        [Required]
        [Display(Name = "Order Id")]
        public Guid OrderId { get; set; }
        [Required]
        [Display(Name = "Required Date")]
        public DateTime? RequiredDate { get; set; }
        [Required]
        [Display(Name = "Shipped Date")]
        public DateTime? ShippedDate { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = " {0} Must be greater than or equal {1}")]
        [Display(Name = "Freight")]
        public decimal? Freight { get; set; } = 0;
    }
}
