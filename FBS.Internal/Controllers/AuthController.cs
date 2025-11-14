using FBS.Application.DataTranferObjects.Auth;
using FBS.Application.DataTranferObjects.Users;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FBS.Internal.Controllers
{
    public class AuthController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserService _userService;

        public AuthController(IUserService userService, SignInManager<User> signInManager, UserManager<User> userManager, IUnitOfWork unitOfWork)
            : base(userManager, unitOfWork)
        {
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
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

                return Redirect(returnUrl ?? "/");
            }
            catch (Exception ex)
            {
                return View(request);
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserSaveDto request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var result = await _userService.CreateUser(request);
                if (result.Type != GlobalConstants.ResponseType.Success)
                {
                    return View();
                }

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private async Task<bool> ValidatePassword(LoginDto request)
        {
            var result = await _signInManager.PasswordSignInAsync(request.Username, request.Password, false, true);
            return result.Succeeded;
        }
    }
}
