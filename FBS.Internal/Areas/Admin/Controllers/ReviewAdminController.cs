using Microsoft.AspNetCore.Mvc;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Internal.Areas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FootballShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ReviewAdminController : BaseAdminController
    {
        public ReviewAdminController(UserManager<User> userManager, IUnitOfWork unitOfWork)
            : base(userManager, unitOfWork) { }

        public async Task<IActionResult> Index(string? keyword, Guid? productId)
        {
            var repo = _unitOfWork.GetRepositoryReadOnlyAsync<ProductReview>();
            var query = await repo.QueryAll();

            query = query.Include(x => x.Product);

            
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(r =>
                    r.FullName.Contains(keyword) ||
                    r.Message.Contains(keyword));
            }

            // Lọc theo sản phẩm
            if (productId != null && productId != Guid.Empty)
            {
                query = query.Where(r => r.ProductId == productId);
            }

            // Lấy danh sách sản phẩm để đổ dropdown
            var productRepo = _unitOfWork.GetRepositoryReadOnlyAsync<Product>();
            var productQuery = await productRepo.QueryAll();
            var productList = await productQuery.ToListAsync();

            ViewBag.Products = productList;

            var reviews = await query
                .Select(r => new ProductReviewViewModel
                {
                    Id = r.Id,
                    ProductName = r.Product != null ? r.Product.Name : "Không xác định",
                    FullName = r.FullName,
                    Message = r.Message,
                    CreatedDate = r.CreatedAt
                })
                .ToListAsync();

            return View(reviews);
        }

        [Authorize(Roles = "Baiviet")]
        [Authorize(Policy = "Review.Manage")]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Repository ghi
            var repo = _unitOfWork.GetRepositoryAsync<ProductReview>();

            // Lấy review theo id
            var review = await repo.FindById(id);

            if (review != null)
            {
                
                await repo.Delete(id);
                await _unitOfWork.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
