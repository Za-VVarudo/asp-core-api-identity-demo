
using IdentityDemoAPI.Data;
using IdentityDemoAPI.Interfaces;
using IdentityDemoAPI.Models;
using IdentityDemoAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration configuration;
        public BookController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
        }
        [Authorize]
        [HttpGet("{page:int?}")]
        public async Task<IActionResult> GetAllBooks(string search, int page = 1, int categoryId = 0)
        {      
            if (page < 0 || categoryId < 0) return BadRequest("Page not found");

            var bookRepo = unitOfWork.BookRepository;
            IEnumerable<Book> list;
            PagingEnumerable<Book> pagingList;

            if (categoryId == 0)
                list = await bookRepo.GetAllBookAsync();  
            else
                list = (await bookRepo.GetAllBookAsync()).Where(b => b.BookCategories.FirstOrDefault(b => b.CategoryId == categoryId) != null); 

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                list = list.Where(b => b.ISBN == search || b.Title.ToLower().Contains(search));
            }
            pagingList = new PagingEnumerable<Book>(list)
            {
                Page = page
            };
            return Ok(pagingList);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> InsertNewBook([FromForm] BookInsertModel model)
        {
            var bookRepo = unitOfWork.BookRepository;
            string _deleteUrl = null;

            if (model.Image != null)
            {
                var (imageUrl, deleteUrl) = await GetHostingImageUrl(model.Image);
                model.ImageUrl = imageUrl;
                _deleteUrl = deleteUrl;
            }

            (int code, string message) = await bookRepo.InsertNewBook(model);
            if (code == 0 && await unitOfWork.Complete())
                return Ok(new { message });

            await DeleteImage(_deleteUrl);
            return BadRequest(new { message });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> EditBook([FromForm] BookUpdateModel model)
        {
            var bookRepo = unitOfWork.BookRepository;
            string _deleteUrl = null;

            if (model.Image != null)
            {
                var (imageUrl, deleteUrl) = await GetHostingImageUrl(model.Image);
                model.ImageUrl = imageUrl;
                _deleteUrl = deleteUrl;
            }

            (int code, string message) = await bookRepo.EditBook(model);
            if (code == 0) 
                return await unitOfWork.Complete() ? Ok(new { message }) : Ok();

            await DeleteImage(_deleteUrl);
            return BadRequest(new { message });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{bookId:Guid}")]
        public async Task<IActionResult> DeleteBook(Guid bookId)
        {
            var bookRepo = unitOfWork.BookRepository;
            (int code, string message) = await bookRepo.DeleteBook(bookId);
            if (code == 0) return await unitOfWork.Complete() ? Ok(new { message }) : Ok();
            return BadRequest(new { message });
        }

        private async Task<(string imageUrl, string deleteUrl)> GetHostingImageUrl(IFormFile image)
        {
            string imageUrl = null;
            string deleteUrl = null;
            var section = configuration.GetSection("ImageHostingService");
            string hostingUrl = section["HostingUrl"] + section["SecretKey"];
            if (image.Length > 0 && hostingUrl != null)
            {
                using (var multiPartForm = new MultipartFormDataContent())
                {
                    var streamContent = new StreamContent(image.OpenReadStream());
                    multiPartForm.Add(streamContent, "image", Guid.NewGuid().ToString());
                    var client = new HttpClient();
                    var result = await client.PostAsync(hostingUrl, multiPartForm);
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsStringAsync();
                        JObject jObject = JObject.Parse(response);
                        if (jObject != null)
                        {
                            imageUrl = jObject["data"]["url"].ToString();
                            deleteUrl = jObject["data"]["delete_url"].ToString();
                        }
                    }
                }
            }
            return (imageUrl, deleteUrl);
        }

        private static async Task DeleteImage(string deleteUrl)
        {
            using var httpClient = await new HttpClient().GetAsync(deleteUrl);
        }
    }
}
