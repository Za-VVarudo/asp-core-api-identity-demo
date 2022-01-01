using IdentityDemoAPI.Data;
using IdentityDemoAPI.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
    [Route("api/external-login")]
    [ApiController]
    public class ExternalLoginController : AuthControllerBase
    {
        public ExternalLoginController(IConfiguration configuration, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IUnitOfWork unitOfWork) : base(configuration, userManager, signInManager, unitOfWork) { }

        [HttpGet("login-google")]
        public IActionResult LoginGoogle()
        {
            string provider = "Google";
            return GetChallengeResult(provider);
        }

        [HttpGet("login-facebook")]
        public IActionResult LoginFacebook()
        {
            string provider = "Facebook";
            return GetChallengeResult(provider);
        }

        [HttpGet("external-login-callback")]
        public async Task<IActionResult> GetCallback(string remoteError = null)
        {
            if (remoteError != null)
            {
                return GetReturnErrorMessage("Error from provider", remoteError);
            }
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            //get external login info returned by middleware
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null) return GetReturnErrorMessage("External Login Information not found");
            else
            {
                var user = await userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));
                if (user == null) return BadRequest("Email is not registered");

                bool isExisted = (await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey)) != null;
                if (!isExisted) await userManager.AddLoginAsync(user, info);

                var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);
                if (result.Succeeded)
                {
                    var dto = await GetUserDTO(user);
                    await StoreInCookie(dto);
                    return Ok(new { Message = info.ProviderDisplayName + " Login Successfully" , User = dto });
                }
                else
                {
                    return GetReturnErrorMessage("Sign In Error:", result.ToString());
                }
            }
        }

        private JsonResult GetReturnErrorMessage(string message, string remoteError = null)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return new JsonResult(new { Message = message + ": \n" + remoteError });
        }

        private ChallengeResult GetChallengeResult(string provider)
        {
            string redirectUrl = "api/external-login/external-login-callback";
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }
    }
}
