using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Models
{
    public class OrderDetailModel
    {
        [Required]
        public Guid BookId { set; get; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be more than or equal 0 ")]
        public int Quantity { set; get; }

        public override bool Equals(object obj)
        {
            return obj is OrderDetailModel model &&
                   BookId.Equals(model.BookId);
        }

        public override int GetHashCode()
        {
            return BookId.GetHashCode();
        }
    }
}
