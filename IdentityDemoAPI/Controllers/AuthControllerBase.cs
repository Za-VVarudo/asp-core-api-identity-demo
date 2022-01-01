using IdentityDemoAPI.Data;
using IdentityDemoAPI.DTOs;
using IdentityDemoAPI.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Controllers
{
    public class AuthControllerBase : ControllerBase
    {
        protected readonly IConfiguration configuration;
        protected readonly UserManager<AppUser> userManager;
        protected readonly SignInManager<AppUser> signInManager;
        protected readonly IUnitOfWork unitOfWork;

        public AuthControllerBase(IConfiguration configuration, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IUnitOfWork unitOfWork)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.unitOfWork = unitOfWork;
        }

        protected List<Claim> GetClaims(AuthorizedUserDTO dto)
        {
            return new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, dto.UserId),
                    new Claim(ClaimTypes.Email, dto.Email),
                    new Claim(ClaimTypes.Name, dto.Username),
                    new Claim(ClaimTypes.StreetAddress, dto.Address),
                    new Claim(ClaimTypes.Role, dto.Role)
                };
        }

        protected ClaimsPrincipal CreateCookieAuthClaimsPrincipal(AuthorizedUserDTO dto)
        {
            var claims = GetClaims(dto);
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(claimsIdentity);
        }
        protected async Task StoreInCookie(AuthorizedUserDTO dto)
        {
            var claimsPrincipal = CreateCookieAuthClaimsPrincipal(dto);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        }

        protected JwtSecurityToken GetJwtToken(AuthorizedUserDTO dto, Guid tokenJti)
        {
            var claims = GetClaims(dto);
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, tokenJti.ToString()));

            string issuer = configuration["JWT:Issuer"];
            string audience = configuration["JWT:Audience"];
            string key = configuration["JWT:Key"];

            double expiredTime = Convert.ToDouble(configuration["JWT:ExpiredSeconds"]);

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credential = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512);
            var expired = DateTime.UtcNow.AddSeconds(expiredTime);

            return new JwtSecurityToken(issuer, audience, claims, expires: expired, signingCredentials: credential);
        }

        protected string GetJwtTokenString(AuthorizedUserDTO dto, Guid tokenJti)
        {
            var token = GetJwtToken(dto, tokenJti);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }

        protected async Task<RefreshToken> GenerateRefreshToken(string userId, Guid tokenJti)
        {
            double expired = Convert.ToDouble(configuration["JWT:RefreshExpiredSeconds"]);

            var refreshToken = unitOfWork.RefreshTokenRepository.CreateRefreshToken(userId, tokenJti, expired);
            await unitOfWork.Complete();

            return refreshToken;
        }
        protected async Task<AuthorizedUserDTO> GetUserDTO(AppUser user)
        {
            string role = (await userManager.GetRolesAsync(user)).FirstOrDefault();
            return new AuthorizedUserDTO { UserId = user.Id, Email = user.Email, Username = user.UserName, Address = user.Address, Role = role };
        }


    }
}
