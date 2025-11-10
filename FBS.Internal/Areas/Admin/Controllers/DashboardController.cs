using Microsoft.AspNetCore.Mvc;

namespace FootballShop.Areas.Admin.Controllers
{
    public class DashboardController : BaseAdminController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
