using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Models
{
    public class OrderModel
    {
        [Required]
        [ShippedDateValidation(ErrorMessage = "Shipped Date can not be in the past")]
        public DateTime ShippedDate { set; get; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Shipping Fee must be more than or equal 0")]
        public decimal ShippingFee { set; get; }
    }

    public class OrderInsertModel : OrderModel
    {
        [Required]
        public string UserId { set; get; }
        [Required]
        [OrderDetailsSetValidation(ErrorMessage = "Duplicate order details found")]
        public OrderDetailModel[] OrderDetails { set; get; }
    }
    public class OrderUpdateModel : OrderModel
    {
        [Required]
        public Guid OrderId { set; get; }
    }

    class ShippedDateValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var shippedDate = Convert.ToDateTime(value);
            return shippedDate >= DateTime.Now;
        }
    }

    class OrderDetailsSetValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var orderDetails = (OrderDetailModel[])value ?? Array.Empty<OrderDetailModel>();
            var set = new HashSet<OrderDetailModel>(orderDetails);
            return set.Count == orderDetails.Length;
        }
    }
}
