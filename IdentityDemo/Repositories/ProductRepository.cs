using IdentityDemo.Areas.Identity.Data;
using IdentityDemo.Areas.Models;
using IdentityDemo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public async Task<List<ProductObject>> GetProducts()
        {
            using (var context = new AppDbContext())
            {
                return await context.Products.Where(p => p.Status == true).ToListAsync();
            }
        }

        public async Task<ProductUpdateModel> GetDetails(int productId)
        {
            using (var context = new AppDbContext())
            {
                var product = await context.Products
                    .Where(p => p.ProductId == productId && p.Status == true)
                    .FirstOrDefaultAsync();
                return new ProductUpdateModel
                {
                    ProductId = product.ProductId,
                    CategoryId = product.CategoryId,
                    ProductName = product.ProductName,
                    UnitPrice = product.UnitPrice,
                    UnitsInStock = product.UnitsInStock,
                    Weight = product.Weight
                };
            }
        }

        public (int, string) InsertProduct(ProductModel product)
        {
            (int code, string message) = (-1, "Duplicate ProductId !");
            using (var context = new AppDbContext())
            {
                try
                {
                    var productObject = new ProductObject
                    {
                        CategoryId = product.CategoryId,
                        ProductName = product.ProductName,
                        UnitPrice = product.UnitPrice,
                        UnitsInStock = product.UnitsInStock,
                        Weight = product.Weight,
                        Status = true
                    };
                    context.Products.Add(productObject);
                    context.SaveChanges();
                    code = 0;
                    message = "Added successfully !";
                }
                catch
                {

                }
            }
            return (code, message);
        }

        public (int, string) UpdateProduct(ProductUpdateModel product)
        {
            (int code, string message) = (-1, "Failed to update !");
            using (var context = new AppDbContext())
            {
                var productObject = context.Products.Find(product.ProductId);
                if (productObject != null)
                {
                    productObject.ProductName = product.ProductName;
                    productObject.CategoryId = product.CategoryId;
                    productObject.UnitPrice = product.UnitPrice;
                    productObject.UnitsInStock = product.UnitsInStock;
                    productObject.Weight = product.Weight;
                    context.SaveChanges();
                    code = 0;
                    message = "Edit successfully !";
                }
                else
                {
                    code = 1;
                    message = "Failed to edit - Product not found !";
                }
            }
            return (code, message);
        }

        public (int, string) DeleteProduct(int productId)
        {
            (int code, string message) = (-1, "Failed to remove !");
            using (var context = new AppDbContext())
            {
                var productObject = context.Products.Find(productId);
                if (productObject != null)
                {
                    productObject.Status = false;
                    context.SaveChanges();
                    code = 0;
                    message = "Remove successfully !";
                }
                else
                {
                    code = 1;
                    message = "Failed to remove - Product not found !";
                }
            }
            return (code, message);
        }
    }
}
