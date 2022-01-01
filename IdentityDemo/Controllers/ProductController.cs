using IdentityDemo.Models;
using IdentityDemo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ProductController : Controller
    {
        private IProductRepository repo;
        public async Task<IActionResult> GetProducts()
        {
            repo = new ProductRepository();
            var list = await repo.GetProducts();
            return View(list);
        }
        
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(ProductModel insertModel)
        {
            if (!ModelState.IsValid)
                return View(insertModel);
            repo = new ProductRepository();
            (int code, string message) = repo.InsertProduct(insertModel);
            if (code == 0) return RedirectToAction("GetProducts");
            ViewBag.Message = message;
            ViewBag.Code = code;
            return View();
        }

        public IActionResult EditProduct(int productId)
        {
            repo = new ProductRepository();
            var productModel = repo.GetDetails(productId).Result;
            if (productModel == null)
            {
                TempData["Message"] = $"Product ID: {productId} not found";
                TempData["Code"] = -1;
                return RedirectToAction("GetProducts");
            }
            return View(productModel);
        }
        [HttpPost]
        public IActionResult EditProduct(ProductUpdateModel updateModel)
        {
            if (!ModelState.IsValid)
                return View(updateModel);
            repo = new ProductRepository();
            (int code, string message) = repo.UpdateProduct(updateModel);
            ViewBag.Message = message;
            ViewBag.Code = code;
            return View(updateModel);
        }
        public IActionResult DeleteProduct(int productId)
        {
            repo = new ProductRepository();
            (int code, string message) = repo.DeleteProduct(productId);
            TempData["Message"] = message;
            TempData["Code"] = code;
            return RedirectToAction("GetProducts");
        }
    }
}
