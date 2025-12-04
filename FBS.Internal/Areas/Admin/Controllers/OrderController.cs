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
        public async Task<IActionResult> Index(string? keyword, DateTime? fromDate, DateTime? toDate, StatusEnum? status)
        {
            var repo = _unitOfWork.GetRepositoryReadOnlyAsync<Order>();
            var query = await repo.QueryAll();   // IQueryable

            // =============================
            // 1️⃣ Lọc trạng thái đơn hàng
            // =============================
            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            // =============================
            // 2️⃣ Lọc theo từ khóa
            // =============================
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim().ToLower();

                query = query.Where(o =>
                    (o.CustomerName != null && o.CustomerName.ToLower().Contains(keyword)) ||
                    (o.CustomerPhone != null && o.CustomerPhone.Contains(keyword)) ||
                    (o.CustomerAddress != null && o.CustomerAddress.ToLower().Contains(keyword)) ||
                    o.Id.ToString().Contains(keyword)
                );
            }

            // =============================
            // 3️⃣ Lọc theo ngày
            // =============================
            if (fromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= fromDate);

            if (toDate.HasValue)
                query = query.Where(o => o.CreatedAt < toDate.Value.AddDays(1));

            // =============================
            // 4️⃣ Trả kết quả
            // =============================
            var orders = query
                .OrderByDescending(o => o.CreatedAt)
                .ToList();
            ViewBag.TotalOrders = orders.Count();
            ViewBag.TotalPending = orders.Count(o => o.Status == StatusEnum.Inactive);        
            ViewBag.TotalProcessing = orders.Count(o => o.Status == StatusEnum.Active);          
            ViewBag.TotalShipping = orders.Count(o => o.Status == StatusEnum.InHandler);       
            ViewBag.TotalSuccess = orders.Count(o => o.Status == StatusEnum.Cancel);         
            ViewBag.TotalCanceled = orders.Count(o => o.Status == StatusEnum.WaitingApproval); 

            if (!orders.Any())
            {
                TempData["SearchError"] = "Không tìm thấy đơn hàng phù hợp!";
            }
           

            ViewBag.Keyword = keyword;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.Status = status;

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
