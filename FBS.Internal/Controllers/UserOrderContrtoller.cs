using FBS.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;

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

        // =====================================
        //  ⭐ DANH SÁCH ĐƠN HÀNG THEO CUSTOMER
        // =====================================
        public async Task<IActionResult> Index()
        {
            if (CurrentUser == null)
                return RedirectToAction("Login", "Auth");

            // Get list orders by CustomerId (Guid)
            var orders = await _orderService.GetOrdersByCustomer(CurrentUser.CustomerId);

            return View(orders); // => List<OrderDto>
        }

        // =====================================
        //  ⭐ CHI TIẾT ĐƠN HÀNG
        // =====================================
        public async Task<IActionResult> Detail(Guid id)
        {
            if (CurrentUser == null)
                return RedirectToAction("Login", "Auth");

            var order = await _orderService.GetOrderDetail(id, CurrentUser.CustomerId);

            if (order == null)
                return NotFound();

            return View(order); // => OrderDto
        }
    }
}
