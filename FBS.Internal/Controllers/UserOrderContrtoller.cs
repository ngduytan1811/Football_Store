using FBS.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.Constants;
using FBS.Application.DataTranferObjects.Orders;
using FBS.Shared.Enums;

namespace FBS.Internal.Controllers
{
    [Authorize]
    public class UserOrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public UserOrderController(
            UserManager<User> userManager,
            IUnitOfWork unitOfWork,
            IOrderService orderService
        ) : base(userManager, unitOfWork)
        {
            _orderService = orderService;
        }

        
        public async Task<IActionResult> Index()
        {
            if (CurrentUser == null)
                return RedirectToAction("Login", "Auth");
            var orders = await _orderService.GetOrdersByCustomer(CurrentUser.CustomerId);
            orders = orders.OrderBy(o => o.Status == StatusEnum.Cancel).ThenByDescending(o => o.CreatedAt).ToList();
            return View(orders); 
        }

        
        public async Task<IActionResult> Detail(Guid id)
        {
            if (CurrentUser == null)
                return RedirectToAction("Login", "Auth");

            var order = await _orderService.GetOrderDetail(id, CurrentUser.CustomerId);

            if (order == null)
                return NotFound();

            return View(order); 
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Cancel(Guid id)
        {
            if (CurrentUser == null)
                return RedirectToAction("Login", "Auth");

            var result = await _orderService.CancelOrder(id, CurrentUser.CustomerId);

            if (result.Type == GlobalConstants.ResponseType.Error)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateInfo(UpdateOrderInfoDto dto)
        {
            if (CurrentUser == null)
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ";
                return RedirectToAction(nameof(Detail), new { id = dto.OrderId });
            }

            var result = await _orderService.UpdateOrderInfo(
                dto.OrderId,
                CurrentUser.CustomerId,
                dto);

            if (result.Type == GlobalConstants.ResponseType.Error)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Detail), new { id = dto.OrderId });
        }


    }
}
