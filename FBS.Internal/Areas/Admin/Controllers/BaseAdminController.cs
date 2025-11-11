using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballShop.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class BaseAdminController : Controller
    {
        
    }
}
