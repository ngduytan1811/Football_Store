using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.Constants;
using FBS.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FootballShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize] 
    public class AdminOrderController : BaseAdminController
    {
        private readonly IOrderService _orderService;

        public AdminOrderController(
            UserManager<User> userManager,
            IUnitOfWork unitOfWork,
            IOrderService orderService
        ) : base(userManager, unitOfWork)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(Guid id)
        {
            await _orderService.UpdateOrderStatus(
                id,
                StatusEnum.Active,
                "Shop đã xác nhận đơn"
            );

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> StartDelivery(Guid id)
        {
            await _orderService.UpdateOrderStatus(
                id,
                StatusEnum.InHandler,
                "Đơn hàng đang được giao"
            );

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Complete(Guid id)
        {
            await _orderService.UpdateOrderStatus(
                id,
                StatusEnum.WaitingApproval,
                "Giao hàng thành công"
            );

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(Guid id)
        {
            await _orderService.UpdateOrderStatus(
                id,
                StatusEnum.Cancel,
                "Shop hủy đơn hàng"
            );

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(Guid id, StatusEnum status)
        {
            string note = status switch
            {
                StatusEnum.Inactive => "Đơn hàng đang chờ xác nhận",
                StatusEnum.Active => "Shop đã xác nhận đơn",
                StatusEnum.InHandler => "Đơn hàng đang được giao",
                StatusEnum.WaitingApproval => "Giao hàng thành công",
                StatusEnum.Cancel => "Shop hủy đơn hàng",
                StatusEnum.NotSeen => "Khách yêu cầu trả hàng",
                StatusEnum.Watched => "Giao hàng thất bại",
                _ => "Cập nhật trạng thái đơn hàng"
            };

            var result = await _orderService.UpdateOrderStatus(id, status, note);

            if (result.Type == GlobalConstants.ResponseType.Error)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction( "Detail", "Order",new { area = "Admin", id }
  );

        }

    }
}
