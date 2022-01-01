using IdentityDemoAPI.Data;
using IdentityDemoAPI.DTOs;
using IdentityDemoAPI.Interfaces;
using IdentityDemoAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AppUser> userManager;

        public UserController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("{page:int?}")]
        public async Task<IActionResult> GetAllUser(string search, int page = 1)
        {
            var userRepo = unitOfWork.UserRepository;
            var list = await userRepo.GetAllUser();
            PagingEnumerable<UserDTO> pagingList;

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                list = list.Where(u => search.Equals(u.UserId) ||
                                       search.Equals(u.UserName) ||
                                       search.Equals(u.Email) ||
                                       search.Equals(u.PhoneNumber));
            }

            pagingList = new PagingEnumerable<UserDTO>(list)
            {
                Page = page
            };

            return Ok(pagingList);
        }

        [HttpDelete("{userId:Guid}")]
        public async Task<IActionResult> RemoveUser(Guid userId)
        {
            var userRepo = unitOfWork.UserRepository;

            (int code, string message) = await userRepo.RemoveUser(userId.ToString().ToLower());

            if (code == 0)
                return await unitOfWork.Complete() ? Ok(new { message }) : Ok();

            return BadRequest(message);
        }

        //Change password => revoke all refresh token with userId 
    }
}
