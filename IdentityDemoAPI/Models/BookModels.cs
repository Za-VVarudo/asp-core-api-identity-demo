using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Models
{
    public class BookModel
    {
        [Required]
        [StringLength(200, ErrorMessage = "Book Title must be as most {1} characters")]
        public string Title { set; get; }
        [Required]
        [PublishedDateValidate(ErrorMessage = "Published Date must be in the past")]
        public DateTime PublishedDate { set; get; }
        [Required]
        [StringLength(100, ErrorMessage = "Author's Name must be as most {1} characters")]
        public string Author { set; get; }
        [Required]
        [StringLength(200, ErrorMessage = "Author's Name must be as most {1} characters")]
        public string Publisher { set; get; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Total Quantity must be at least 0")]
        public int TotalQuantity { set; get; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be at least 0")]
        public decimal Price { set; get; }
        [BookCategoriesQuantityValidate(ErrorMessage = "A book must have as least one category")]
        [BookCategoriesSetValidate(ErrorMessage = "Dupplicate categories found - All category must be unique")]
        public int[] BookCategories { set; get; }
        public IFormFile Image { set; get; }
        public string ImageUrl { set; get; }
    }

    public class BookUpdateModel : BookModel
    {
        [Required]
        public Guid BookId { set; get; }
    }
    public class BookInsertModel : BookModel
    {
        [Required]
        [RegularExpression("^[0-9]{13}$", ErrorMessage = "ISBN must be 13 digits")]
        public string ISBN { set; get; }
    }

    class PublishedDateValidate : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var publishedDate = Convert.ToDateTime(value);
            return DateTime.Now >= publishedDate;
        }
    }

    class BookCategoriesQuantityValidate : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var categories = (int[]) value;
            return categories != null && categories.Length > 0;
        }
    }
    class BookCategoriesSetValidate : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var categories = (int[])value ?? new int[] { };
            HashSet<int> set = new(categories);
            return categories.Length == set.Count;
        }
    }
}
