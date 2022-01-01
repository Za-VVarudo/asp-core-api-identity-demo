using IdentityDemo.Areas.Identity.Data;
using IdentityDemo.Areas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        public ProductAPIController()
        {

        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetProduct()
        {
            IEnumerable<ProductObject> products = null;
            using (var context = new AppDbContext())
            {
                products = context.Products.ToList();
            }
            return new JsonResult(products);
        }
    }
}
