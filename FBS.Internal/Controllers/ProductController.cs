using FBS.Application.DataTranferObjects.Cart;
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

            
            var randomProducts = await _productService.GetRandomProducts();

          
            ViewData["Product"] = data.Data;
            ViewData["RandomProducts"] = randomProducts;   
            ViewData["SearchData"] = request ?? new ProductSearchDto();

            return View(new CartItemDto
            {
                ProductId = id
            });
        }


    }
}
