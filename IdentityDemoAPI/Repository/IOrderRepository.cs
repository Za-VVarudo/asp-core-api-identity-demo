using IdentityDemoAPI.Data;
using IdentityDemoAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Repository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrderAsync();
        Task<(int, string)> CreateOrder(OrderInsertModel model);
        Task<(int, string)> EditOrder(OrderUpdateModel model);
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext context;
        public OrderRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Order>> GetAllOrderAsync()
        {
            return await context.Orders.Include(o => o.OrderDetails).ToListAsync();
        }

        public async Task<(int, string)> CreateOrder(OrderInsertModel model)
        {
            (int code, string message) = (-102, $"Create order failed - UserId {model.UserId} not found");
            var orderId = Guid.NewGuid();

            var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id.Equals(model.UserId));
            if (user != null)
            {
                code = -103;

                List<OrderDetail> detailList = new();
                decimal total = 0;
                foreach (var detail in model.OrderDetails)
                {
                    var book = await context.Books.FindAsync(detail.BookId);
                    if (book == null)
                    {
                        message = $"Create order failed - Book Id {detail.BookId} not found";
                        return (code, message);
                    } 
                    else
                    {
                        int remainQuantity = book.TotalQuantity - detail.Quantity;
                        if (remainQuantity < 0)
                        {
                            code = -104;
                            message = $"Create order failed - Book ISBN {book.ISBN} quantity is insufficient [ Order: {detail.Quantity}, Remain: {book.TotalQuantity} ]";
                            return (code, message);
                        }
                        else
                        {
                            decimal detailTotal = book.Price * detail.Quantity;
                            total += detailTotal;

                            book.TotalQuantity = remainQuantity;
                            detailList.Add(new OrderDetail { OrderId = orderId, BookId = detail.BookId, CurrentPrice = book.Price, Quantity = detail.Quantity, Total = detailTotal });
                        }
                    }
                }

                var order = new Order
                {
                    Id = orderId,
                    CreateDate = DateTime.Now,
                    ShippedDate = model.ShippedDate,
                    ShippingFee = model.ShippingFee,
                    UserId = model.UserId,
                    Total = total,
                    OrderDetails = detailList
                };
                await context.Orders.AddAsync(order);
                code = 0;
                message = $"Created Successfully - Order Id {orderId}";
            }
            return (code, message);
        }

        public async Task<(int, string)> EditOrder(OrderUpdateModel model)
        {
            (int code, string message) = (-201, $"Create order failed - Order Id {model.OrderId} not found");

            var order = await context.Orders.FindAsync(model.OrderId);
            if (order != null)
            {
                order.ShippedDate = model.ShippedDate;
                order.ShippingFee = model.ShippingFee;
                code = 0;
                message = "Edit Successfully";
            }
            return (code, message);
        }
    }
}
