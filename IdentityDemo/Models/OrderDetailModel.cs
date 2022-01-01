using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Models
{
    public class OrderDetailModel
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "{0} Must be equal or more than 0")]
        [Display(Name = "Product Id")]
        public int ProductId { get; set; }
        public string ProductName { set; get; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "{0} Must be equal or more than 0")]
        [Display(Name = "Quantity")]
        public int Quantity { set; get; }
        [Required]
        [Range(0, 1, ErrorMessage = "{0} Must be between [0, 1]")]
        [Display(Name = "Discount")]
        public double Discount { set; get; }
    }
}
