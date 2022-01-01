using IdentityDemoAPI.Data;
using IdentityDemoAPI.Interfaces;
using IdentityDemoAPI.Models;
using IdentityDemoAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{page:int?}")]
        public async Task<IActionResult> GetAllOrder(Guid? orderId, int page = 1)
        {
            var orderRepo = unitOfWork.OrderRepository;
            IEnumerable<Order> list = await orderRepo.GetAllOrderAsync();
            PagingEnumerable<Order> pagingList;

            if (orderId != null)
                list = list.Where(o => o.Id.Equals(orderId));
            pagingList = new(list)
            {
                Page = page
            };
            return Ok(pagingList);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderInsertModel model)
        {
            var orderRepo = unitOfWork.OrderRepository;
            (int code, string message) = await orderRepo.CreateOrder(model);

            if (code == 0 && await unitOfWork.Complete())
                return Ok(new { message });

            return BadRequest(new { message});
        }

        [HttpPut]
        public async Task<IActionResult> EditOrder(OrderUpdateModel model)
        {
            var orderRepo = unitOfWork.OrderRepository;
            (int code, string message) = await orderRepo.EditOrder(model);

            if (code == 0)
                return await unitOfWork.Complete() ? Ok(new { message}) : Ok();

            return BadRequest(new { message });
        }
    }
}
