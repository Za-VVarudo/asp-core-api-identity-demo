using IdentityDemo.Areas.Models;
using IdentityDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderObject>> GetOrders();
        Task<OrderObject> GetOrder(string orderId);
        (int, string) AddOrder(OrderObject order, IEnumerable<OrderDetailObject> detailList);
        Task<(int, string)> EditOrderAsync(OrderUpdateModel model);
    }
}
