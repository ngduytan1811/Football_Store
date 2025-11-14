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

        public IActionResult Contact()
        {
            return View();
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
