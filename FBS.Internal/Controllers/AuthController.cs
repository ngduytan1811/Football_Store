using Microsoft.AspNetCore.Mvc;

namespace FBS.Internal.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
