using FBS.Application.DataTranferObjects.Auth;
using FBS.Application.DataTranferObjects.Users;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FBS.Internal.Controllers
{
    public class AuthController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserService _userService;

        public AuthController(
            IUserService userService,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IUnitOfWork unitOfWork)
            : base(userManager, unitOfWork)
        {
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

      

        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto request, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(request);

            var user = await _userManager.FindByNameAsync(request.Username);

            if (user == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
                return View(request);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError("", "Tài khoản đã bị khóa. Vui lòng liên hệ hỗ trợ!");
                return View(request);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                request.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
                return View(request);
            }

            user.LastLogin = DateTime.Now;
            await _userManager.UpdateAsync(user);

            return Redirect(returnUrl ?? "/");
        }

      

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserSaveDto request)
        {
            if (!ModelState.IsValid)
                return View(request);

            try
            {
                // kiểm tra trùng
                var existingUser = await _userManager.FindByNameAsync(request.UserName);
                if (existingUser != null)
                {
                    ModelState.AddModelError("UserName", "Tên tài khoản đã tồn tại!");
                    return View(request);
                }

                // Kiểm tra trùng 
                var existingEmail = await _userManager.FindByEmailAsync(request.Email);
                if (existingEmail != null)
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng!");
                    return View(request);
                }

                
                var user = new User
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    Address = request.Address,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                        ModelState.AddModelError("", err.Description);

                    return View(request);
                }

                return RedirectToAction("Login");
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra! Vui lòng thử lại.");
                return View(request);
            }
        }

      

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
