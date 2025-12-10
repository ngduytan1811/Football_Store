using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FBS.Application.DataTranferObjects.Reports;

namespace FootballShop.Areas.Admin.Controllers
{
    [Area("admin")]
    public class DashboardController : BaseAdminController
    {
        public DashboardController(UserManager<User> userManager, IUnitOfWork unitOfWork)
            : base(userManager, unitOfWork)
        {
        }

        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
        {
            var repo = _unitOfWork.GetRepositoryReadOnlyAsync<Order>();
            var query = await repo.QueryAll();

            var orders = query
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .ToList();

            // Nếu không có dữ liệu -> tránh lỗi
            if (!orders.Any())
            {
                ViewBag.TotalRevenue = 0;
                ViewBag.TodayRevenue = 0;
                ViewBag.ThisMonthRevenue = 0;
                ViewBag.Last7DaysRevenue = new List<RevenueChartDto>();
                ViewBag.TopProducts = new List<TopProductDto>();
                ViewBag.RangeChartData = new List<RevenueChartDto>();
                return View();
            }

            // Lấy ngày mới nhất trong DB để chart không bị lệch thời gian thực
            var lastDate = orders
                .Where(o => o.CreatedAt.HasValue)
                .Max(o => o.CreatedAt.Value.Date);

            var today = lastDate;

            // =====================================================================
            // 🟦 1. DOANH THU TỔNG QUAN
            // =====================================================================

            ViewBag.TotalRevenue = orders.Sum(o =>
                o.OrderItems.Sum(i => (i.Price ?? 0) * (i.Quantity ?? 0)));

            ViewBag.TodayRevenue = orders
                .Where(o => o.CreatedAt.HasValue &&
                            o.CreatedAt.Value.Date == today)
                .Sum(o => o.OrderItems.Sum(i => (i.Price ?? 0) * (i.Quantity ?? 0)));

            ViewBag.ThisMonthRevenue = orders
                .Where(o => o.CreatedAt.HasValue &&
                            o.CreatedAt.Value.Month == today.Month &&
                            o.CreatedAt.Value.Year == today.Year)
                .Sum(o => o.OrderItems.Sum(i => (i.Price ?? 0) * (i.Quantity ?? 0)));



            // =====================================================================
            // 🟩 2. TRẠNG THÁI ĐƠN HÀNG
            // =====================================================================

            ViewBag.TotalOrders = orders.Count;
            ViewBag.CompletedOrders = orders.Count(o => o.Status == StatusEnum.Cancel);
            ViewBag.CanceledOrders = orders.Count(o => o.Status == StatusEnum.WaitingApproval);
            ViewBag.ShippingOrders = orders.Count(o => o.Status == StatusEnum.InHandler);
            ViewBag.ProcessingOrders = orders.Count(o => o.Status == StatusEnum.Active);


            // =====================================================================
            // 🟧 3. BIỂU ĐỒ DOANH THU 7 NGÀY GẦN NHẤT
            // =====================================================================

            var last7Days = Enumerable.Range(0, 7)
                .Select(i => lastDate.AddDays(-i).Date)
                .OrderBy(d => d)
                .Select(day => new RevenueChartDto
                {
                    Date = day.ToString("dd/MM"),
                    Revenue = orders
                        .Where(o => o.CreatedAt.HasValue &&
                                    o.CreatedAt.Value.Date == day)
                        .Sum(o => o.OrderItems.Sum(i => (i.Price ?? 0) * (i.Quantity ?? 0)))
                })
                .ToList();

            ViewBag.Last7DaysRevenue = last7Days;


            // =====================================================================
            // 🟨 4. TOP 5 SẢN PHẨM BÁN CHẠY
            // =====================================================================

            var top5Products = orders
                .SelectMany(o => o.OrderItems)
                .Where(i => i.Product != null)
                .GroupBy(i => i.ProductId)
                .Select(g => new TopProductDto
                {
                    ProductId = g.Key,
                    ProductName = g.First().Product.Name,
                    TotalQuantity = g.Sum(x => x.Quantity ?? 0)
                })
                .OrderByDescending(g => g.TotalQuantity)
                .Take(5)
                .ToList();

            ViewBag.TopProducts = top5Products;


            // =====================================================================
            // 🟥 5. DOANH THU THEO KHOẢNG NGÀY
            // =====================================================================

            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            if (!fromDate.HasValue || !toDate.HasValue)
            {
                ViewBag.RevenueInRange = 0;
                ViewBag.RangeChartData = new List<RevenueChartDto>();
            }
            else
            {
                // ⭐ KIỂM TRA: ngày bắt đầu không được lớn hơn ngày kết thúc
                if (fromDate.Value.Date > toDate.Value.Date)
                {
                    ViewBag.DateError = "Ngày bắt đầu không được lớn hơn ngày kết thúc!";
                    ViewBag.RevenueInRange = 0;
                    ViewBag.RangeChartData = new List<RevenueChartDto>();
                    return View();
                }

                // ⭐ TÍNH DOANH THU TRONG KHOẢNG NGÀY
                var revenueInRange = orders
                    .Where(o => o.CreatedAt.HasValue &&
                                o.CreatedAt.Value.Date >= fromDate.Value.Date &&
                                o.CreatedAt.Value.Date <= toDate.Value.Date)
                    .Sum(o => o.OrderItems.Sum(i => (i.Price ?? 0) * (i.Quantity ?? 0)));

                ViewBag.RevenueInRange = revenueInRange;

                // ⭐ TẠO DANH SÁCH CÁC NGÀY TRONG KHOẢNG
                var daysRange = Enumerable.Range(0, (toDate.Value - fromDate.Value).Days + 1)
                    .Select(i => fromDate.Value.AddDays(i))
                    .ToList();

                var rangeChartData = daysRange.Select(day => new RevenueChartDto
                {
                    Date = day.ToString("dd/MM"),
                    Revenue = orders
                        .Where(o => o.CreatedAt.HasValue &&
                                    o.CreatedAt.Value.Date == day.Date)
                        .Sum(o => o.OrderItems.Sum(i => (i.Price ?? 0) * (i.Quantity ?? 0)))
                }).ToList();

                ViewBag.RangeChartData = rangeChartData;
            }


            return View();
        }
    }
}
