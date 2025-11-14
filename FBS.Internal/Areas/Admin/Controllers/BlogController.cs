using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FootballShop.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FBS.Internal.Areas.Admin.Controllers
{
    public class BlogController : BaseAdminController
    {
        public BlogController(UserManager<User> userManager, IUnitOfWork unitOfWork) : base(userManager, unitOfWork)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
