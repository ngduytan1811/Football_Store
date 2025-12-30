using FBS.Application.DataTranferObjects.Cart;
using FBS.Application.DataTranferObjects.Products;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.DataTranferObjects.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FBS.Internal.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IProductReviewService _productReviewService;

        public ProductController(
            UserManager<User> userManager,
            IUnitOfWork unitOfWork,
            IProductService productService,
            IProductReviewService productReviewService
        ) : base(userManager, unitOfWork)
        {
            _productService = productService;
            _productReviewService = productReviewService;
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

            return View(new CartItemDto
            {
                ProductId = id
            });
        }

        [HttpPost]
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
                FullName = FullName,
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
