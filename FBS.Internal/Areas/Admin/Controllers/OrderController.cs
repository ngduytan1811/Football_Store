using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : BaseAdminController
    {
        public OrderController(UserManager<User> userManager, IUnitOfWork unitOfWork)
            : base(userManager, unitOfWork)
        {
        }

       
       
        public async Task<IActionResult> Index(string? keyword, DateTime? fromDate, DateTime? toDate, StatusEnum? status)
        {
            var repo = _unitOfWork.GetRepositoryReadOnlyAsync<Order>();
            var query = await repo.QueryAll();   

         
            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

    
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

        
            if (fromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= fromDate);

            if (toDate.HasValue)
                query = query.Where(o => o.CreatedAt < toDate.Value.AddDays(1));

    
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
        
        [Authorize(Roles = "Quanlydonhang")]
        [Authorize(Policy = "Order.View")]
        public async Task<IActionResult> Detail(Guid id)
        {
            var repo = _unitOfWork.GetRepositoryReadOnlyAsync<Order>();
            var query = await repo.QueryAll();

            var order = query
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product) 
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }
      
        [HttpPost]
        [Authorize(Roles = "Quanlydonhang")]     
        [Authorize(Policy = "Order.Update")]
        public async Task<IActionResult> UpdateStatus(Guid id, StatusEnum status)
        {
            var readRepo = _unitOfWork.GetRepositoryReadOnlyAsync<Order>();
            var order = await (await readRepo.QueryAll())
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            if (order.PaymentMethod == "VietQR"
                && order.PaymentStatus == PaymentStatusEnum.Unpaid)
            {
                TempData["Error"] = "Đơn VietQR chưa thanh toán";
                return RedirectToAction("Detail", new { id });
            }

            if (!order.IsStockDeducted &&
                (status == StatusEnum.Active ||
                 status == StatusEnum.InHandler ||
                 status == StatusEnum.WaitingApproval))
            {
                var colorRepo = _unitOfWork.GetRepositoryReadOnlyAsync<ProductColor>();
                var sizeRepo = _unitOfWork.GetRepositoryAsync<ProductSize>();

                var colors = await (await colorRepo.QueryAll())
                    .Include(c => c.ProductSizes)
                    .ToListAsync();

                foreach (var item in order.OrderItems)
                {
                    var color = colors.FirstOrDefault(c =>
                        c.ProductId == item.ProductId &&
                        c.Color == item.ProductColor);

                    if (color == null) continue;

                    var size = color.ProductSizes
                        .FirstOrDefault(ps => ps.Size == item.ProductSize);

                    if (size == null) continue;

                    if (size.Quantity < item.Quantity)
                    {
                        TempData["Error"] = "Không đủ tồn kho";
                        return RedirectToAction("Detail", new { id });
                    }

                    size.Quantity -= item.Quantity.Value;
                    await sizeRepo.Update(size);
                }

                order.IsStockDeducted = true;
            }

            var writeRepo = _unitOfWork.GetRepositoryAsync<Order>();
            order.Status = status;
            await writeRepo.Update(order);
            await _unitOfWork.SaveChangesAsync();

            TempData["Success"] = "Cập nhật trạng thái thành công";
            return RedirectToAction("Detail", new { id });
        }
        [HttpPost]
        [Authorize(Roles = "Quanlydonhang")]

        public async Task<IActionResult> ConfirmPayment(Guid id)
        {
            var repo = _unitOfWork.GetRepositoryAsync<Order>();
            var order = await repo.Single(x => x.Id == id);

            if (order == null)
                return NotFound();

            if (order.PaymentMethod != "VietQR")
            {
                TempData["Error"] = "Đơn hàng này không phải VietQR!";
                return RedirectToAction("Detail", new { id });
            }

            if (order.PaymentStatus == PaymentStatusEnum.Paid)
            {
                TempData["Error"] = "Đơn hàng đã được thanh toán!";
                return RedirectToAction("Detail", new { id });
            }

            order.PaymentStatus = PaymentStatusEnum.Paid;
            order.Status = StatusEnum.Active; // bắt đầu xử lý

            await repo.Update(order);
            await _unitOfWork.SaveChangesAsync();

            TempData["Success"] = "Đã xác nhận thanh toán VietQR!";
            return RedirectToAction("Detail", new { id });
        }

    }
}
