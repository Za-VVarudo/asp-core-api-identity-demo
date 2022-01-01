using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Models
{
    public class ProductModel
    {
        [Required]
        [Range(1, 999, ErrorMessage = " {0} Must between [{1}, {2}]")]
        [Display(Name = "Category Id")]
        public int CategoryId { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Product Name ")]
        public string ProductName { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Weight")]
        public string Weight { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = " {0} Must be greater than or equal {1}")]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = " {0} Must be greater than or equal {1}")]
        [Display(Name = "Units In Stock")]
        public int UnitsInStock { get; set; }
    }

    public class ProductUpdateModel : ProductModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = " {0} Must between [{1}, {2}]")]
        [Display(Name = "Product Id")]
        public int ProductId { get; set; }
    }
}
