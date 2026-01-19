using FBS.Application.DataTranferObjects.Cart;
using FBS.Application.DataTranferObjects.Products;
using FBS.Application.Services;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Internal.Models;
using FBS.Shared.DataTranferObjects.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FBS.Internal.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IProductReviewService _productReviewService;
        private object _productSizeService;

        public ProductController(
            UserManager<User> userManager,
            IUnitOfWork unitOfWork,
            IProductService productService,
            IProductReviewService productReviewService
        ) : base(userManager, unitOfWork)
        {
            _productService = productService;
            _productReviewService = productReviewService;
            _productSizeService = productService;
        }

        public async Task<IActionResult> List(ProductSearchDto request)
        {
            var dataSearch = new BaseSearchDto<ProductSearchDto>()
            {
                SearchParams = request,
                Page = request.Page,
            };

            var data = await _productService.GetProducts(dataSearch);

            var startIndex = dataSearch.Start + 1;
            data.Items?.ForEach(i => i.Index = startIndex++);

            ViewData["Products"] = data;
            ViewData["SearchData"] = request ?? new ProductSearchDto();

            return View();
        }

        public async Task<IActionResult> Detail(Guid id, ProductSearchDto request)
        {
            var data = await _productService.FindById(id);
            if (data?.Data == null)
                return RedirectToAction("List");

            var product = data.Data;

            var colorRepo = _unitOfWork.GetRepositoryReadOnlyAsync<ProductColor>();
            var productColors = await (await colorRepo.QueryAll())
                .Where(c => c.ProductId == id)
                .Include(c => c.ProductSizes)
                .ToListAsync();

            ViewBag.Colors = productColors.Select(c => new
            {
                id = c.Id,
                color = c.Color
            }).ToList();

            var sizeStocks = productColors.SelectMany(c => c.ProductSizes.Select(ps => new SizeStockViewModel
                 {
                     Size = ps.Size,
                     Quantity = ps.Quantity
                 }))
                 .ToList();

            ViewBag.SizeStocks = sizeStocks;


            var randomProducts = await _productService.GetRandomProducts();
            var reviews = await _productReviewService.GetReviews(id);

            product.Reviews = reviews.Select(r => new ProductReivewDto
            {
                FullName = r.FullName,
                Message = r.Message
            }).ToList();

            ViewData["Product"] = product;
            ViewData["RandomProducts"] = randomProducts;
            ViewData["Reviews"] = product.Reviews;
            ViewData["SearchData"] = request ?? new ProductSearchDto();

            return View(new CartItemDto { ProductId = id });
        }


        [HttpGet]
        public async Task<IActionResult> GetStockByColor(Guid productColorId)
        {
            var sizes = await _unitOfWork
                .GetRepositoryReadOnlyAsync<ProductSize>()
                .QueryAll();

            var data = await sizes
                .Where(x => x.ProductColorId == productColorId)
                .Select(x => new
                {
                    size = x.Size,
                    quantity = x.Quantity
                })
                .ToListAsync();

            return Json(data);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Review(Guid ProductId, string FullName, string Message)
        {
            if (ProductId == Guid.Empty)
                return RedirectToAction("Detail", new { id = ProductId });

            if (string.IsNullOrWhiteSpace(Message))
            {
                TempData["Error"] = "Nội dung đánh giá không được để trống!";
                return RedirectToAction("Detail", new { id = ProductId, tab = "review" });
            }

            var user = await _userManager.GetUserAsync(User);

          
           
           
            var review = new ProductReview
            {
                ProductId = ProductId,
                FullName = user.UserName,
                Message = Message,
                CreatedAt = DateTime.Now
            };

            var repo = _unitOfWork.GetRepositoryAsync<ProductReview>();
            await repo.Add(review);
            await _unitOfWork.SaveChangesAsync();

            TempData["Success"] = "Gửi đánh giá thành công!";

            return RedirectToAction("Detail", new { id = ProductId, tab = "review" });
        }

    }
}
