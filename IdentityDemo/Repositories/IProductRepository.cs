using IdentityDemo.Areas.Models;
using IdentityDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Repositories
{
    interface IProductRepository
    {
        Task<List<ProductObject>> GetProducts();
        Task<ProductUpdateModel> GetDetails(int productId);
        (int, string) InsertProduct(ProductModel product);
        (int, string) UpdateProduct(ProductUpdateModel product);
        (int, string) DeleteProduct(int productId);
    }
}
