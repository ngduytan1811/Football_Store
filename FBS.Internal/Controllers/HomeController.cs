

using FBS.Application.Services;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Internal.Controllers;
using FootballShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FootballShop.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IProductService _productService;

        public HomeController(UserManager<User> userManager, IUnitOfWork unitOfWork, IProductService productService) : base(userManager, unitOfWork)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var randomProducts = await _productService.GetRandomProducts();
            ViewData["RandomProducts"] = randomProducts;

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> Contact()
        {
            var model = new ContactViewModel();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    model.FullName = user.UserName;       
                    model.Email = user.Email;
                    model.Phone = user.PhoneNumber;
                }
            }

            return View(model);
        }
      
        [HttpPost]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var repo = _unitOfWork.GetRepositoryAsync<Contact>();

            string firstName = "";
            string lastName = "";

            if (!string.IsNullOrWhiteSpace(model.FullName))
            {
                var parts = model.FullName.Trim().Split(' ', 2);
                firstName = parts[0];
                lastName = parts.Length > 1 ? parts[1] : "";
            }

            var contact = new Contact
            {
                FirstName = firstName,
                LastName = lastName,
                Phone = model.Phone,
                Email = model.Email,
                Message = model.Message,
                IsDeleted = false
            };

            await repo.Add(contact);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Bạn đã gửi liên hệ thành công!";
            return RedirectToAction("Contact");
        }





        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
