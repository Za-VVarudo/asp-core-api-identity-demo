using IdentityDemoAPI.Data;
using IdentityDemoAPI.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserDTO>> GetAllUser();
        Task<(int, string)> RemoveUser(string userId);
    }

    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext context;
        public UserRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUser()
        {
            return await context.Users
                .Include(u => u.UserRoles)
                .Select(u => new UserDTO 
                { 
                    Address = u.Address,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    PhoneNumber = u.PhoneNumber,
                    UserId = u.Id,
                    UserName = u.UserName,
                    UserRoles = u.UserRoles
                })
                .ToListAsync();
        }
        public async Task<(int, string)> RemoveUser(string userId)
        {
            (int code, string message) = (-301, $"Remove failed - UserId {userId} not found");

            var user = await context.Users.Where(u => u.Id.Equals(userId)).Include(u => u.UserRoles).FirstOrDefaultAsync();
            if (user != null)
            {
                bool isAdmin = user.UserRoles.Where(ur => ur.RoleId.Equals("AD")).FirstOrDefault() != null;
                if (isAdmin)
                {
                    code = -300;
                    message = "Remove failed - Cannot remove Admin";
                }
                else
                {
                    user.LockoutEnd = DateTime.MaxValue;
                    user.IsActive = false;
                    code = 0;
                    message = "Remove Successfully";
                }
            }
            return (code, message);
        }
    }
}
