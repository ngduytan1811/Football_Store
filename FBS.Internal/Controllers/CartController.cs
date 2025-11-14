using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories;
using FBS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FBS.Internal.Controllers
{
    public class CartController : BaseController
    {
        public CartController(UserManager<User> userManager, IUnitOfWork unitOfWork) : base(userManager, unitOfWork)
        {
            
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            return View();
        }
    }
}
