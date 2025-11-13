using Microsoft.AspNetCore.Mvc;

namespace FBS.Internal.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(string blogId)
        {
            return View();
        }
    }
}
