using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballShop.Areas.Admin.Controllers
{
    [Area("admin")]
    public class OrderController : BaseAdminController
    {
        public OrderController(UserManager<User> userManager, IUnitOfWork unitOfWork)
            : base(userManager, unitOfWork)
        {
        }

        // 📌 DANH SÁCH ĐƠN HÀNG
        public async Task<IActionResult> Index()
        {
            var repo = _unitOfWork.GetRepositoryReadOnlyAsync<Order>();
            var query = await repo.QueryAll();

            var orders = query
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return View(orders);
        }

        // 📌 CHI TIẾT ĐƠN HÀNG
        public async Task<IActionResult> Detail(Guid id)
        {
            var repo = _unitOfWork.GetRepositoryReadOnlyAsync<Order>();
            var query = await repo.QueryAll();

            var order = query
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product) // load Product
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }


        // 📌 CẬP NHẬT TRẠNG THÁI ĐƠN HÀNG
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(Guid id, string status)
        {
            var repo = _unitOfWork.GetRepositoryAsync<Order>();
            var order = await repo.Single(x => x.Id == id);

            if (order == null)
                return NotFound();

            if (Enum.TryParse<StatusEnum>(status, out var newStatus))
            {
                order.Status = newStatus;
            }
            else
            {
                TempData["Error"] = "Trạng thái không hợp lệ!";
                return RedirectToAction("Detail", new { id });
            }


            await repo.Update(order);
            await _unitOfWork.SaveChangesAsync();

            TempData["Success"] = "Cập nhật trạng thái thành công!";
            return RedirectToAction("Detail", new { id });
        }
    }
}
