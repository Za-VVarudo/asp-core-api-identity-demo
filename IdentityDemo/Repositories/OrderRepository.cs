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
    public class OrderRepository : IOrderRepository
    {
        public async Task<IEnumerable<OrderObject>> GetOrders()
        {
            using (var context = new AppDbContext())
            {
                return await context.Orders.ToListAsync();
            }
        }

        public async Task<OrderObject> GetOrder(string orderId)
        {
            using (var context = new AppDbContext())
            {
                var order = await context.Orders.Where(o => o.OrderId.ToString() == orderId)
                    .Include(u => u.User)
                    .FirstOrDefaultAsync();
                if (order != null)
                {
                    var list = context.OrderDetails.ToList();
                    var details = await context.OrderDetails.Where(d => d.OrderId.ToString().Equals(orderId)).Include(d => d.Product).ToListAsync();
                    order.OrderDetails = details;
                }
                return order;
            }
        }

        public (int, string) AddOrder(OrderObject order, IEnumerable<OrderDetailObject> detailList)
        {
            (int code, string message) result = (-1, "");
            using (var context = new AppDbContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        bool valid = true;
                        context.Orders.Add(order);
                        foreach (var detail in detailList)
                        {
                            var product = context.Products.Find(detail.ProductId);
                            result.code = detail.ProductId;
                            if (product != null && product.UnitsInStock >= detail.Quantity)
                            {
                                product.UnitsInStock -= detail.Quantity;
                                context.SaveChanges();
                                context.ChangeTracker.Clear();

                                detail.Product = null;
                                context.OrderDetails.Add(detail);
                                context.SaveChanges(); 
                            }
                            else
                            {
                                valid = false;
                                result.message = $"Product {product.ProductId} is not in-stock";
                                break;
                            }
                        }
                        if (valid)
                        {
                            result.code = -1;
                            transaction.Commit();
                        }
                    }
                    catch (Exception)
                    {
                        result.code = -13;
                        transaction.Rollback();
                    }
                }
            }
            return result;
        }

        public async Task<(int, string)> EditOrderAsync(OrderUpdateModel model) 
        {
            (int code, string message) = (-100, $"Failed to edit order {model.OrderId}");
            using (var context = new AppDbContext()) 
            {
                var order = await context.Orders.FindAsync(model.OrderId);
                if (order != null)
                {
                    order.RequiredDate = model.RequiredDate;
                    order.ShippedDate = model.ShippedDate;
                    order.Freight = model.Freight;
                    await context.SaveChangesAsync();
                    message = "Order info updated";
                    code = 0;
                }
                else
                {
                    message += " - Order not found";
                }
            }
            return (code, message);
        }
    }
}
