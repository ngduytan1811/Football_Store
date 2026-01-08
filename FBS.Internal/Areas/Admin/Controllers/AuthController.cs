using FBS.Application.DataTranferObjects.Auth;
using FBS.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace FBS.Internal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous] // Chỉ AuthController mới allow anonymous
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        // Các role admin được phép truy cập
        private readonly string[] _allowedRoles =
           { "Admin", "Quanlysanpham", "Quanlydonhang", "Baiviet", "Lienhe" };

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /Admin/Auth/Login
        public IActionResult Login(string? returnUrl = null)
        {
            // Nếu đã đăng nhập rồi thì redirect thẳng dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                return Redirect("/Admin/Dashboard");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Admin/Auth/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto request, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(request);

            // Tìm user theo username hoặc email
            var user = await _userManager.FindByNameAsync(request.Username)
                       ?? await _userManager.FindByEmailAsync(request.Username?.ToLower());

            if (user == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
                return View(request);
            }

            // Kiểm tra mật khẩu
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
                return View(request);
            }

            // Kiểm tra role admin
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Any(r => _allowedRoles.Contains(r)))
            {
                ModelState.AddModelError("", "Bạn không có quyền truy cập trang quản trị!");
                return View(request);
            }

            // Login user
            await _signInManager.SignInAsync(user, isPersistent: request.RememberMe);

            // Redirect về returnUrl hoặc dashboard
            if (string.IsNullOrEmpty(returnUrl) || returnUrl.ToLower().Contains("/admin/login"))
            {
                return Redirect("/Admin/Dashboard");
            }

            return Redirect(returnUrl);
        }

        // GET: /Admin/Auth/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Admin/Auth/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterAdminDto request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var user = new User
            {
                UserName = request.Username,
                Email = request.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);

                return View(request);
            }

            // Gán role Admin nếu chưa có
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
                await _userManager.AddToRoleAsync(user, "Admin");

            return RedirectToAction("Login");
        }

        // GET: /Admin/Auth/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Admin/Auth/ForgotPassword
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.IsInRoleAsync(user, "Admin"))
            {
                ModelState.AddModelError("", "Email không tồn tại hoặc không phải admin!");
                return View(request);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, "Admin@123");

            ViewBag.Message = "Mật khẩu mới: Admin@123";
            return View();
        }

        // POST: /Admin/Auth/Logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth", new { Area = "Admin" });
        }
    }
}
