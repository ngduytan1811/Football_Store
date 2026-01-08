using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FBS.Application.DataTranferObjects.Reports;
using Microsoft.AspNetCore.Authorization;

namespace FootballShop.Areas.Admin.Controllers
{
    [Area("Admin")]
   
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

            var orders = await query
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .ToListAsync();

            //tránh lỗi khi k hiện dữ liệu
            if (!orders.Any())
            {
                ViewBag.TotalRevenue = 0;
                ViewBag.TodayRevenue = 0;
                ViewBag.ThisMonthRevenue = 0;

                ViewBag.TotalOrders = 0;
                ViewBag.CompletedOrders = 0;
                ViewBag.CanceledOrders = 0;
                ViewBag.ShippingOrders = 0;
                ViewBag.ProcessingOrders = 0;

                ViewBag.Last7DaysRevenue = new List<RevenueChartDto>();
                ViewBag.TopProducts = new List<TopProductDto>();
                ViewBag.RangeChartData = new List<RevenueChartDto>();

                return View();
            }

            // lấy đơn hợp lệ
            var validOrders = orders
                .Where(o => o.Status != StatusEnum.Cancel && o.CreatedAt.HasValue)
                .ToList();

            // ngày mới nhất
            var lastDate = validOrders.Max(o => o.CreatedAt!.Value.Date);
            var today = lastDate;

           //doanh thu tổng
            ViewBag.TotalRevenue = validOrders.Sum(o =>
                o.OrderItems.Sum(i => (i.Price ?? 0) * (i.Quantity ?? 0)));

            ViewBag.TodayRevenue = validOrders
     .Where(o => o.CreatedAt!.Value.Date == today)
     .Sum(o => o.OrderItems.Sum(i => (i.Price ?? 0) * (i.Quantity ?? 0)));


            ViewBag.ThisMonthRevenue = validOrders
                .Where(o => o.CreatedAt!.Value.Month == today.Month &&
                            o.CreatedAt!.Value.Year == today.Year)
                .Sum(o => o.OrderItems.Sum(i => (i.Price ?? 0) * (i.Quantity ?? 0)));

           // trạng thái đơn hàng
            ViewBag.TotalOrders = orders.Count;
            ViewBag.CanceledOrders = orders.Count(o => o.Status == StatusEnum.Cancel);
            ViewBag.ProcessingOrders = orders.Count(o => o.Status == StatusEnum.WaitingApproval);
            ViewBag.ShippingOrders = orders.Count(o => o.Status == StatusEnum.InHandler);
            ViewBag.CompletedOrders = orders.Count(o => o.Status == StatusEnum.Active);

          // biểu đồ
            var last7Days = Enumerable.Range(0, 7)
    .Select(i => lastDate.AddDays(-i))
    .OrderBy(d => d)
    .Select(day => new RevenueChartDto
    {
        Date = day.ToString("dd/MM"),
        Revenue = validOrders
            .Where(o => o.CreatedAt!.Value.Date == day.Date)
            .Sum(o => o.OrderItems.Sum(i => (i.Price ?? 0) * (i.Quantity ?? 0)))
    })
    .ToList();
            ViewBag.Last7DaysRevenue = last7Days;

            // sản phẩm bán chạy
            var top5Products = validOrders
                .SelectMany(o => o.OrderItems)
                .Where(i => i.Product != null)
                .GroupBy(i => i.ProductId)
                .Select(g => new TopProductDto
                {
                    ProductId = g.Key,
                    ProductName = g.First().Product!.Name,
                    TotalQuantity = g.Sum(x => x.Quantity ?? 0)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(5)
                .ToList();

            ViewBag.TopProducts = top5Products;

       
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            if (!fromDate.HasValue || !toDate.HasValue)
            {
                ViewBag.RevenueInRange = 0;
                ViewBag.RangeChartData = new List<RevenueChartDto>();
            }
            else
            {
                if (fromDate.Value.Date > toDate.Value.Date)
                {
                    ViewBag.DateError = "Ngày bắt đầu không được lớn hơn ngày kết thúc!";
                    ViewBag.RevenueInRange = 0;
                    ViewBag.RangeChartData = new List<RevenueChartDto>();
                    return View();
                }

                var revenueInRange = validOrders
                    .Where(o => o.CreatedAt!.Value.Date >= fromDate.Value.Date &&
                                o.CreatedAt!.Value.Date <= toDate.Value.Date)
                    .Sum(o => o.OrderItems.Sum(i => (i.Price ?? 0) * (i.Quantity ?? 0)));

                ViewBag.RevenueInRange = revenueInRange;

                var daysRange = Enumerable.Range(0, (toDate.Value - fromDate.Value).Days + 1)
                    .Select(i => fromDate.Value.AddDays(i))
                    .ToList();

                var rangeChartData = daysRange.Select(day => new RevenueChartDto
                {
                    Date = day.ToString("dd/MM"),
                    Revenue = validOrders
                        .Where(o => o.CreatedAt!.Value.Date == day.Date)
                        .Sum(o => o.OrderItems.Sum(i => (i.Price ?? 0) * (i.Quantity ?? 0)))
                }).ToList();

                ViewBag.RangeChartData = rangeChartData;
            }

            return View();
        }
    }
}
