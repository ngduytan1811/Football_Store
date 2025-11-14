using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories;
using FBS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FBS.Internal.Controllers
{
    public class ProductController : BaseController
    {
        public ProductController(UserManager<User> userManager, IUnitOfWork unitOfWork) : base(userManager, unitOfWork)
        {
            
        }
        public IActionResult List()
        {
            return View();
        }

        public IActionResult Detail(string productId)
        {
            return View();
        }
    }
}
