using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Repository
{
    public class PagingEnumerable<T>
    {
        public PagingEnumerable(IEnumerable<T> list)
        {
            List = list ?? new List<T>();
        }
        private readonly IEnumerable<T> List;
        public int NumPerPage { set; get; } = 10;
        public int Page { set; get; } = 1;
        public int MaxPage
        {
            get
            {
                double count = List.Count();
                return (int)Math.Ceiling(count / NumPerPage);
            }
        }

        public IEnumerable<T> PagingList
        {
            get
            {
                var list = List.Skip((Page - 1) * NumPerPage).Take(NumPerPage);
                return list;
            }
        }
    }
}
