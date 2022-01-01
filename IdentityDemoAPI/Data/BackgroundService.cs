using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Data
{
    public class BackgroundService
    {
        public Guid Id { set; get; }
        public string Name { set; get; }
        public DateTime FiredTime { set; get; }
        public string Content { set; get; }
    }
}
