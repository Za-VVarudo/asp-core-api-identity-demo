using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Data
{
    public class Category
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "int")]
        public int Id { set; get; }
        [Column(TypeName = "nvarchar(200)")]
        public string Name { set; get; }
        public ICollection<BookCategory> BookCategories { set; get; }
    }
}
