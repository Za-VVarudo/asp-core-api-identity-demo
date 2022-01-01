using IdentityDemoAPI.Data;
using IdentityDemoAPI.Interfaces;
using IdentityDemoAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace IdentityDemoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : AuthControllerBase
    {
        private static readonly DateTime date_1_1_1970 = new DateTime(1970, 1, 1);
        public TokenController(IConfiguration configuration, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IUnitOfWork unitOfWork) : base(configuration, userManager, signInManager, unitOfWork)
        {

        }
        [HttpPost("refresh")]
        public async Task<IActionResult> GetRefreshedToken(RefreshTokenRequest tokenRequest)
        {
            var responseMessage = new AuthResponse
            {
                Error = "Token is not expired yet"
            };
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jwtSecurityToken = handler.ReadJwtToken(tokenRequest.Token);
                int expired = jwtSecurityToken.Payload.Exp ?? 0;

                if (IsTokenExpired(expired))
                {
                    var refreshTokenRepo = unitOfWork.RefreshTokenRepository;
                    var dbRefreshToken = await refreshTokenRepo.GetRefreshToken(tokenRequest.RefreshToken);
                    if (dbRefreshToken == null)
                    {
                        responseMessage.Error = "Invalid Refresh token";
                    }
                    else
                    {
                        if (dbRefreshToken.IsRevoke || dbRefreshToken.IsUsed)
                        {
                            responseMessage.Error = "Refresh token is used or revoked";
                        }
                        else
                        {
                            if (dbRefreshToken.Jti.ToString().Equals(jwtSecurityToken.Id))
                            {
                                string userId = jwtSecurityToken.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).FirstOrDefault();

                                if (userId.Equals(dbRefreshToken.UserId))
                                {
                                    if (DateTime.UtcNow <= dbRefreshToken.ExpiredDate)
                                    {
                                        var user = await userManager.FindByIdAsync(userId);
                                        var dto = await GetUserDTO(user);
                                        var tokenJti = new Guid();

                                        dbRefreshToken.IsUsed = true;
                                        await refreshTokenRepo.EditRefreshToken(dbRefreshToken);
                                        await unitOfWork.Complete();

                                        responseMessage.Error = null;
                                        responseMessage.Success = true;
                                        responseMessage.Token = GetJwtTokenString(dto, tokenJti);
                                        responseMessage.RefreshToken = (await GenerateRefreshToken(userId, tokenJti)).Id;

                                        return Ok(responseMessage);
                                    }
                                    responseMessage.Error = "Expired Refresh token";
                                }
                                else
                                {
                                    responseMessage.Error = "Invalid Payload";
                                }
                            }
                            else
                            {
                                responseMessage.Error = "Information mismatch";
                            }
                        }
                    }
                }
            }
            catch
            {
                responseMessage.Error = "Invalid token";
            }
            return BadRequest(responseMessage);
        }

        private bool IsTokenExpired(int expired)
        {
            return (DateTime.UtcNow - date_1_1_1970).TotalSeconds >= expired;
        }
    }
}
