using AngleSharp.Io;
using FBS.Application.DataTranferObjects.Auth;
using FBS.Application.Services;
using FBS.Infrastructure.Entities;
using FBS.Shared.Constants;
using FBS.Shared.DataTranferObjects.Base;
using FootballShop.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FBS.Internal.Areas.Admin.Controllers
{
    public class AuthController : BaseAdminController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect(returnUrl ?? "/admin/dashboard");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto request, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var user = await _userManager.FindByNameAsync(request.Username);
                if (user == null)
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
                    return View(request);
                }

                var validateLogin = await ValidatePassword(request);
                if (!validateLogin)
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
                    return View(request);
                }

                return Redirect(returnUrl ?? "/admin/dashboard");
            }
            catch (Exception ex)
            {
                return View(request);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth", new { Area = "Admin" });
        }

        private async Task<bool> ValidatePassword(LoginDto request)
        {
            var result = await _signInManager.PasswordSignInAsync(request.Username, request.Password, false, true);
            //if (result.IsLockedOut)
            //{
            //    return response;
            //}

            return result.Succeeded;
        }
    }
}
