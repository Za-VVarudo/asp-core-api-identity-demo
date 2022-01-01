using IdentityDemo.Areas.Identity.Data;
using IdentityDemo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Repositories
{
    public class UserRepository
    {
        public async Task<IEnumerable<IdentityUserRole<string>>> GetUserRolesAsync()
        {
            using (var context = new AppDbContext())
            {
                return await context.UserRoles.ToListAsync();
            }
        }
        public async Task<IEnumerable<AppUsers>> GetUsersAsync()
        {
            using (var context = new AppDbContext())
            {
                var listUserRoles = await context.UserRoles.ToListAsync();
                var listUser = await context.Users.Include(u => u.Orders.OrderBy(o => o.OrderDate))
                                          .Where(u => u.Status)
                                          .ToListAsync();
                var list = from u in listUser
                           join ur in listUserRoles on u.Id equals ur.UserId
                           where ur.RoleId != "AD"
                           select u;
                return list;
            }
        }
        public async Task<IEnumerable<AppUsers>> GetOnlyUserRoleAsync()
        {
            using (var context = new AppDbContext())
            {
                var listUser = await context.Users.ToListAsync();
                var listUserRoles = await context.UserRoles.ToListAsync();
                var list = from u in listUser
                           join ur in listUserRoles on u.Id equals ur.UserId
                           where ur.RoleId != "AD" && u.Status
                           select u;
                return list;
            }
        }
        public async Task<AppUsers> GetUserAsync(string id)
        {
            using (var context = new AppDbContext())
            {
                return await context.Users.Where(u => u.Id == id && u.Status).Include(u => u.Orders.OrderBy(o => o.OrderDate)).FirstOrDefaultAsync();
            }
        }

        public async Task<(int , string)> RemoveUser(string id)
        {
            (int code, string message) = (1, $"Failed to remove User id: {id}");
            using (var context = new AppDbContext())
            {
                var user = await context.Users.FindAsync(id);
                if (user != null)
                {
                    user.LockoutEnabled = true;
                    user.LockoutEnd = DateTime.MaxValue;
                    user.Status = false;
                    context.SaveChanges();
                    code = 0;
                    message = "Removed";
                }
                else
                {
                    message += "- User not found";
                }
            }
            return (code, message);
        }

        public async Task<UserProfileModel> GetUserProfileAsync(string id)
        {
            using (var context = new AppDbContext())
            {
                var user = await context.Users.FindAsync(id);
                return new UserProfileModel
                {
                    FullName = user.FullName,
                    Address = user.Address,
                    City = user.City,
                    Country = user.Country
                };
            }
        }

        public async Task<(int, string)> UpdateProfileAsync(UserProfileModel profile, string id)
        {
            (int code, string message) = (-200, "Failed to update profile");
            using (var context = new AppDbContext())
            {
                var user = await context.Users.FindAsync(id);
                if (user != null)
                {
                    user.FullName = profile.FullName;
                    user.Address = profile.Address;
                    user.City = profile.City;
                    user.Country = profile.Country;
                    await context.SaveChangesAsync();
                    code = 0;
                    message = "Profile updated";
                }
                else
                {
                    code = -201;
                    message += $" - User id: {id} not found";
                }
            }
            return (code, message);
        }
    }
}
