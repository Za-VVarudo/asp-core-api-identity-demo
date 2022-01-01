using IdentityDemo.Areas.Models;
using IdentityDemo.Helpers;
using IdentityDemo.Models;
using IdentityDemo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IdentityDemo.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrders()
        {
            IOrderRepository orderRepo = new OrderRepository();
            var list = await orderRepo.GetOrders();
            return View(list.OrderBy(o => o.OrderDate));
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrderDetail (string orderId)
        {
            IOrderRepository orderRepo = new OrderRepository();
            var order = await orderRepo.GetOrder(orderId);
            if (order == null)
            {
                TempData["Message"] = "Order id not found !";
                TempData["Code"] = -1;
                return RedirectToAction("GetOrders");
            }
            decimal total = order.OrderDetails.Sum(od => od.Quantity * od.UnitPrice * (1 - (decimal)od.Discount));
            ViewBag.Total = total;
            return View(order);
        }

        public async Task<IActionResult> GetOwnOrderDetail(string orderId)
        {
            IOrderRepository orderRepo = new OrderRepository();
            var order = await orderRepo.GetOrder(orderId);
            if (order.UserId != User.GetNameIdentifier())
            {
                return RedirectToAction("Index", "Home");
            }
            decimal total = order.OrderDetails.Sum(od => od.Quantity * od.UnitPrice * (1 - (decimal)od.Discount));
            ViewBag.Total = total;
            return View(order);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditOrder(string orderId)
        {
            IOrderRepository orderRepo = new OrderRepository();
            var order = await orderRepo.GetOrder(orderId);
            if (order == null)
            {
                TempData["Message"] = "Order id not found !";
                TempData["Code"] = -1;
                return RedirectToAction("GetOrders");
            }
            else
            {
                if (DateTime.Now.CompareTo(order.ShippedDate) > 0)
                {
                    TempData["Message"] = "Can not edit shipped order";
                    TempData["Code"] = -401;
                    return RedirectToAction("GetOrders");
                }
            }
            var model = new OrderUpdateModel
            {
                OrderId = order.OrderId,
                RequiredDate = order.RequiredDate,
                ShippedDate = order.ShippedDate,
                Freight = order.Freight
            };
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditOrder(OrderUpdateModel model)
        {
            IOrderRepository orderRepo = new OrderRepository();
            var order = await orderRepo.GetOrder(model.OrderId + "");
            if (order == null)
            {
                TempData["Message"] = "Order id not found !";
                TempData["Code"] = -1;
                return RedirectToAction("GetOrders");
            }
            else
            {
                if (DateTime.Now.CompareTo(order.ShippedDate) > 0)
                {
                    TempData["Message"] = "Can not edit shipped order";
                    TempData["Code"] = -401;
                    return RedirectToAction("GetOrders");
                }
                if (!(model.RequiredDate >= model.ShippedDate && model.ShippedDate >= order.OrderDate))
                {
                    ViewBag.Message = $"Shipped Date and Required Date must after {order.OrderDate.ToString("MM/dd/yyyy")}, Shipped Date must before {model.RequiredDate?.ToString("MM/dd/yyyy")}";
                    ViewBag.Code = -101;
                    return View();
                }
            }
            (int code, string message) = await orderRepo.EditOrderAsync(model);
            ViewBag.Message = message;
            ViewBag.Code = code;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateOrder(string email)
        {
            email = email?.ToLower();
            UserRepository userRepo = new UserRepository();
            var userEmails = await userRepo.GetOnlyUserRoleAsync();
            var comboList = userEmails.Where(u => u.Id != User.GetNameIdentifier()).Select(u => new UserCombo { Id = u.Id, Email = u.Email }).ToList();
            if (email != null)
            {
                var selectedObject = comboList.Where(u => u.Email?.ToLower() == email).FirstOrDefault();
                if (selectedObject == null)
                {
                    TempData["Message"] = $"User id: {email} not found !";
                    TempData["Code"] = -2;
                    return RedirectToAction("GetOrders");
                }
                ViewBag.SelectedEmail = email;
                return View();
            }
            else
            {
                comboList.Insert(0, new UserCombo());
                var comboEmail = new SelectList(comboList, "Email", "Email");
                ViewBag.CmbEmail = comboEmail;
                return View();
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderModel insertModel)
        {
            if (ModelState.IsValid)
            {
                var create = insertModel.OrderDate;
                var shipped = insertModel.ShippedDate;
                var required = insertModel.RequiredDate;
                if (required >= create && required >= shipped && shipped >= create)
                {
                    UserRepository userRepo = new UserRepository();
                    var user = (await userRepo.GetUsersAsync()).Where(u => u.Email == insertModel.Email).FirstOrDefault();
                    if (user != null)
                    {                        
                        var session = HttpContext.Session;
                        var detailList = new Dictionary<int, OrderDetailObject>();
                        session.SetObject("NewOrder", insertModel);
                        session.SetObject("DetailList", detailList);
                        return RedirectToAction("AddOrderDetail");
                    }
                    else
                    {
                        TempData["Message"] = $"User id: {insertModel.Email} not found !";
                        TempData["Code"] = 2;
                        return RedirectToAction("GetOrders");
                    }
                }
                else
                {
                    ViewBag.SelectedEmail = insertModel.Email;
                    ViewBag.Message = "Shipped Date must be in [Create Date, RequiredDate]";
                    ViewBag.Code = 1;
                }
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddOrderDetail()
        {
            var session = HttpContext.Session;
            var order = session.GetObject<OrderModel>("NewOrder");
            if (order == null) return RedirectToAction("Index", "Home");
            ViewBag.CmbProduct = await GetCmbProduct();
            ViewBag.NewOrder = HttpContext.Session.GetObject<OrderModel>("NewOrder");
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddProductToOrder(OrderDetailModel model)
        {
            var session = HttpContext.Session;
            var order = session.GetObject<OrderModel>("NewOrder");
            var detailList = session.GetObject<Dictionary<int, OrderDetailObject>>("DetailList");
            if (detailList == null) detailList = new();
            if (order == null) return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                ProductRepository productRepo = new ProductRepository();
                var product = (await productRepo.GetProducts()).Where(p => p.ProductId == model.ProductId && p.UnitPrice > 0).FirstOrDefault();
                if (product == null)
                {
                    ViewBag.Message = $"Product {model.ProductId} not found";
                    ViewBag.Code = -1;
                }
                else
                {
                    if (product.UnitsInStock < model.Quantity)
                    {
                        ViewBag.Message = $"Product [{product.ProductId}-{product.ProductName}] in stock is: {product.UnitsInStock}";
                        ViewBag.Code = -2;
                    }
                    else
                    {
                        var detail = new OrderDetailObject()
                        {
                            OrderId = order.OrderId,
                            ProductId = model.ProductId,
                            UnitPrice = product.UnitPrice,
                            Quantity = model.Quantity,
                            Discount = model.Discount,
                            Product = product
                        };
                        bool valid = AddDetail(ref detailList, detail, product.UnitsInStock);
                        if (!valid)
                        {
                            ViewBag.Message = $"Product {product.ProductId} in stock is: {product.UnitsInStock}";
                            ViewBag.Code = -2;
                        }
                    }
                }
            }
            session.SetObject("DetailList", detailList);
            ViewBag.DetailList = detailList.Values.OrderBy(d => d.ProductId).ToList();
            ViewBag.NewOrder = order;
            ViewBag.CmbProduct = await GetCmbProduct();
            return View("AddOrderDetail");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveProductFromOrder(int productId)
        {
            var session = HttpContext.Session;
            var order = session.GetObject<OrderModel>("NewOrder");
            var detailList = session.GetObject<Dictionary<int, OrderDetailObject>>("DetailList");
            if (detailList == null) detailList = new();
            if (order == null) return RedirectToAction("Index", "Home");

            RemoveDetail(ref detailList, productId);
            session.SetObject("DetailList", detailList);
            ViewBag.DetailList = detailList.Values.OrderBy(d => d.ProductId).ToList();
            ViewBag.NewOrder = order;
            ViewBag.CmbProduct = await GetCmbProduct();
            return View("AddOrderDetail");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CancelOrder()
        {
            var session = HttpContext.Session;
            session.Remove("NewOrder");
            session.Remove("DetailList");
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> ConfirmOrder()
        {
            var session = HttpContext.Session;
            var order = session.GetObject<OrderModel>("NewOrder");
            var detailList = session.GetObject<Dictionary<int, OrderDetailObject>>("DetailList");
            if (detailList == null) detailList = new();
            if (order == null) return RedirectToAction("Index", "Home");

            UserRepository userRepo = new UserRepository();
            var user = (await userRepo.GetUsersAsync()).Where(u => u.Email.Equals(order.Email)).FirstOrDefault();
            if (user == null)
            {
                TempData["Message"] = $"User id: {order.Email} not found !";
                TempData["Code"] = 2;
                return RedirectToAction("GetOrders");
            }
            var detailValues = detailList.Values.OrderBy(d => d.ProductId).ToList();
            var orderObject = new OrderObject
            {
                UserId = user.Id,
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                RequiredDate = order.RequiredDate,
                ShippedDate = order.ShippedDate,
                Freight = order.Freight
            };

            IOrderRepository orderRepo = new OrderRepository();
            (int code, string message) = orderRepo.AddOrder(orderObject, detailValues);
            if (code < 0)
            {
                ViewBag.DetailList = detailValues;
                return View(order);
            }
            else
            {
                if (code == -13) return RedirectToAction("Index", "Home");
                ViewBag.Message = message;
                ViewBag.Code = -69;
            }
            ViewBag.DetailList = detailValues;
            ViewBag.NewOrder = order;
            ViewBag.CmbProduct = await GetCmbProduct();
            return View("AddOrderDetail");
        }

        [Authorize(Roles = "Admin")]
        private async Task<SelectList> GetCmbProduct()
        {
            ProductRepository productRepo = new ProductRepository();
            var productList = (await productRepo.GetProducts()).Where(p => p.UnitsInStock > 0).Select(p => new ProductCombo { ProductId = p.ProductId, ProductName = p.ProductName }).ToList();
            productList.Insert(0, new ProductCombo());
            return new SelectList(productList, "ProductId", "ProductName");
        }

        [Authorize(Roles = "Admin")]
        private bool AddDetail(ref Dictionary<int, OrderDetailObject> detailList, OrderDetailObject detail, int maxQuantity)
        {
            if (detailList.ContainsKey(detail.ProductId))
            {
                var pos = detailList[detail.ProductId];
                if (pos.Quantity + detail.Quantity > maxQuantity) return false;
                pos.Quantity += detail.Quantity;
                return true;
            }
            else
            {
                detailList.Add(detail.ProductId, detail);
                return true;
            }
        }

        [Authorize(Roles = "Admin")]
        private bool RemoveDetail(ref Dictionary<int, OrderDetailObject> detailList, int productId)
        {
            return detailList.Remove(productId);
        }
    }
}
