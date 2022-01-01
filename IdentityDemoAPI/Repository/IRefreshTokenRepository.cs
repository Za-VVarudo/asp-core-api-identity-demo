using IdentityDemoAPI.Data;
using IdentityDemoAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Repository
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetRefreshToken(string refreshToken);
        RefreshToken CreateRefreshToken(string userId, Guid tokenJti, double expired);
        Task<(int code, string meesage)> EditRefreshToken(RefreshToken rtk);
    }

    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext context;
        public RefreshTokenRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<RefreshToken> GetRefreshToken(string refreshToken)
        {
            return await context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(rtk => rtk.Id.Equals(refreshToken));
        }

        public RefreshToken CreateRefreshToken (string userId, Guid tokenJti, double expired)
        {
            RefreshToken refreshToken;
            try
            {
                refreshToken = new RefreshToken
                {
                    Id = Guid.NewGuid() + "" + Guid.NewGuid(),
                    Jti = tokenJti,
                    UserId = userId,
                    ExpiredDate = DateTime.UtcNow.AddSeconds(expired)
                };
                context.RefreshTokens.Add(refreshToken);
            } 
            catch {
                return CreateRefreshToken(userId, tokenJti, expired);
            }
            return refreshToken;
        }

        public async Task<(int code, string meesage)> EditRefreshToken (RefreshToken rtk)
        {
            (int code, string message) = (-201, "Not found RefreshToken");
            var refreshToken = await context.RefreshTokens.FindAsync(rtk.Id);
            if (refreshToken != null)
            {
                refreshToken.IsUsed = rtk.IsUsed;
                refreshToken.IsRevoke = rtk.IsRevoke;
                code = 0;
                message = "Edit successfully";
            }
            return (code, message);
        }
    }
}
