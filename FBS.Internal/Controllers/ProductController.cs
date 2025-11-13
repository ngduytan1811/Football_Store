using Microsoft.AspNetCore.Mvc;

namespace FBS.Internal.Controllers
{
    public class ProductController : Controller
    {
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
