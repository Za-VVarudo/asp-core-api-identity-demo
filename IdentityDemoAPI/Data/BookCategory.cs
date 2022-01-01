using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Data
{
    public class BookCategory
    {
        [Column(TypeName = "uniqueidentifier")]
        public Guid BookId { set; get; }
        [Column(TypeName = "int")]
        public int CategoryId { set; get; }
        public Book Book { set; get; }
        public Category Category { set; get; }
    }
}
