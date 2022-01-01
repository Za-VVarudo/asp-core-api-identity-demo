using IdentityDemo.Areas.Identity.Data;
using IdentityDemo.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;
using IdentityDemo.Repositories;
using IdentityDemo.Models;

namespace IdentityDemo.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private UserRepository repo;
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            repo = new UserRepository();
            var list = await repo.GetUsersAsync();
            return View(list);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMemberDetail(string memberId)
        {
            repo = new UserRepository();
            var member = await repo.GetUserAsync(memberId);
            if (member == null)
            {
                TempData["Message"] = "Member not found";
                TempData["Code"] = -1;
                return RedirectToAction("Index");
            }
            return View(member);
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetOwnDetails()
        {
            repo = new UserRepository();
            var member = await repo.GetUserAsync(User.GetNameIdentifier());
            if (member == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(member);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveMember(string memberId)
        {
            repo = new UserRepository();
            (int code, string message) = await repo.RemoveUser(memberId);
            TempData["Message"] = message;
            TempData["Code"] = code;
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> UpdateProfile()
        {
            repo = new UserRepository();
            var profile = await repo.GetUserProfileAsync(User.GetNameIdentifier());
            return View(profile);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserProfileModel profile)
        {
            if (!ModelState.IsValid) return View();
            repo = new UserRepository();
            (int code, string message) = await repo.UpdateProfileAsync(profile, User.GetNameIdentifier());
            ViewBag.Message = message;
            ViewBag.Code = code;
            return View();
        }
    }
}
