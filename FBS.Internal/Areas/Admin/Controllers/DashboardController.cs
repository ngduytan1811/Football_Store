using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FootballShop.Areas.Admin.Controllers
{
    public class DashboardController : BaseAdminController
    {
        public DashboardController(UserManager<User> userManager, IUnitOfWork unitOfWork) : base(userManager, unitOfWork)
        {
            
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
