using IdentityDemoAPI.Data;
using IdentityDemoAPI.DTOs;
using IdentityDemoAPI.Helpers;
using IdentityDemoAPI.Interfaces;
using IdentityDemoAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : AuthControllerBase
    {
        
        public AuthController(IConfiguration configuration, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IUnitOfWork unitOfWork) : base(configuration, userManager, signInManager, unitOfWork) 
        {

        }

        [HttpPost("v1/login")]
        public async Task<IActionResult> Login([FromForm] LoginModel loginModel)
        {
            var dto = await GetAuthorizedUserDTO();
            if (dto != null) return Ok(new { Message = "You are already logged in", User = dto});

            var user = await userManager.FindByEmailAsync(loginModel.Email);
            if (user == null) return BadRequest(new AuthResponse { Error = "Login failed - Email is not registered" });

            var result = await signInManager.CheckPasswordSignInAsync(user, loginModel.Password, false);
            if (result.Succeeded)
            {
                dto = await GetUserDTO(user);
                //await StoreInCookie(dto);
                //return Ok(dto);
                Guid tokenJti = Guid.NewGuid();
                var tokenString = GetJwtTokenString(dto, tokenJti);

                var refreshToken = await GenerateRefreshToken(dto.UserId, tokenJti);

                return Ok(new AuthResponse { Success = true, Token = tokenString, RefreshToken = refreshToken.Id });
            }
            return Unauthorized(new AuthResponse{ Error = result.ToString() });
        }

        [HttpPost("v1/register")]
        public async Task<IActionResult> Register([FromForm] RegisterModel registerModel)
        {
            bool isExist = (await userManager.FindByEmailAsync(registerModel.Email)) != null;
            if (isExist) return BadRequest($"Email {registerModel.Email} is registered ");
            else
            {
                var user = new AppUser
                {
                    UserName = registerModel.Email.ToLower(),
                    Email = registerModel.Email,
                    Address = registerModel.Address,
                    CreateDate = DateTime.UtcNow,
                    IsActive = true,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, registerModel.Password);
                if (result.Succeeded)
                {
                    result = await userManager.AddToRoleAsync(user, "US");
                    if (result.Succeeded)
                    {
                        return Ok(new { Message = "Created Successfully" });
                    }
                    return Unauthorized(new { Message = result.ToString() });
                }
                return BadRequest(new { Message = result.ToString() });
            }
        }
        [Authorize]
        [HttpGet("v1/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("You are logged out");
        }
        private async Task<AuthorizedUserDTO> GetAuthorizedUserDTO()
        {
            AuthorizedUserDTO dto = null;
            if (User.Identity.IsAuthenticated)
            {
                var user = await userManager.FindByEmailAsync(User.GetEmail() ?? "");
                dto = await GetUserDTO(user);
            }
            return dto;
        }

        /*
        private string GetResultErrorMessage(string message, IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                message += "\n - " + error.Description;
            }
            return message;
        }
        */
    }
}
