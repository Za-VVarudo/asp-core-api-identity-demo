using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Data
{
    public class Book
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid Id { set; get; }
        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string Title { set; get; }
        [Required]
        [Column(TypeName = "nvarchar(13)")]
        public string ISBN { set; get; }
        [Required]
        [Column(TypeName = "date")]
        public DateTime PublishedDate { set; get; }
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Author { set; get; }
        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string Publisher { set; get; }
        [Required]
        [Column(TypeName = "int")]
        public int TotalQuantity { set; get; }
        [Required]
        [Column(TypeName = "money")]
        public decimal Price { set; get; }
        [Column(TypeName = "nvarchar(250)")]
        public string ImageUrl { set; get; }
        [Required]
        [Column(TypeName = "bit")]
        public bool Status { set; get; }

        public ICollection<BookCategory> BookCategories { set; get; }
    }
}
