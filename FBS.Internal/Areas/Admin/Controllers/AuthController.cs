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
    [AllowAnonymous] 
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        
        private readonly string[] _allowedRoles =
           { "Admin", "Quanlysanpham", "Quanlydonhang", "Baiviet", "Lienhe" };

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }       
        public IActionResult Login(string? returnUrl = null)
        {
            
            if (User.Identity?.IsAuthenticated == true)
            {
                return Redirect("/Admin/Dashboard");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }     
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto request, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(request);

         
            var user = await _userManager.FindByNameAsync(request.Username)
                       ?? await _userManager.FindByEmailAsync(request.Username?.ToLower());

            if (user == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
                return View(request);
            }

           
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
                return View(request);
            }

            
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Any(r => _allowedRoles.Contains(r)))
            {
                ModelState.AddModelError("", "Bạn không có quyền truy cập trang quản trị!");
                return View(request);
            }

      
            await _signInManager.SignInAsync(user, isPersistent: request.RememberMe);

          
            if (string.IsNullOrEmpty(returnUrl) || returnUrl.ToLower().Contains("/admin/login"))
            {
                return Redirect("/Admin/Dashboard");
            }

            return Redirect(returnUrl);
        }      
        public IActionResult Register()
        {
            return View();
        }     
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

      
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
                await _userManager.AddToRoleAsync(user, "Admin");

            return RedirectToAction("Login");
        }
      
        public IActionResult ForgotPassword()
        {
            return View();
        }    
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
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth", new { Area = "Admin" });
        }
    }
}
