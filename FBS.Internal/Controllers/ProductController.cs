using FBS.Application.DataTranferObjects.Products;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.DataTranferObjects.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FBS.Internal.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        public ProductController(UserManager<User> userManager, IUnitOfWork unitOfWork, IProductService productService) : base(userManager, unitOfWork)
        {
            _productService = productService;
        }
        public async Task<IActionResult> List(Guid? categoryId = null, int page = 1)
        {
            var dataSearch = new BaseSearchDto<ProductSearchDto>()
            {
                SearchParams = new ProductSearchDto
                {
                    CategoryId = categoryId,
                },
                Page = page,
            };

            var data = await _productService.GetProducts(dataSearch);
            var startIndex = dataSearch.Start + 1;
            data.Items?.ForEach(i => i.Index = startIndex++);
            ViewData["Products"] = data;

            return View();
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            var data = await _productService.FindById(id);
            ViewData["Product"] = data.Data;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Review(ProductReviewSaveDto request)
        {
            var response = await _productService.CreateProductReview(request);
            return RedirectToAction("Detail", new { id = request.ProductId });
        }
    }
}
