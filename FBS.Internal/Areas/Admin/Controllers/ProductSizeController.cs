using FBS.Application.DataTranferObjects.Products;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Internal.Areas.Models;
using FootballShop.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FBS.Internal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Quanlysanpham")]
    public class ProductSizeController : BaseAdminController
    {
        private readonly IProductSizeService _productSizeService;

        public ProductSizeController(
            IProductSizeService productSizeService,
            UserManager<User> userManager,
            IUnitOfWork unitOfWork
        ) : base(userManager, unitOfWork)
        {
            _productSizeService = productSizeService;
        }


        public async Task<IActionResult> Index(Guid productColorId)
        {
        
            var sizesDto = await _productSizeService
                .GetByProductColorAsync(productColorId);

            var model = sizesDto.Select(x => new ProductSizeViewModel
            {
                Id = x.Id,
                Size = x.Size,
                Quantity = x.Quantity
            }).ToList();
            var colorQuery = await _unitOfWork
                .GetRepositoryReadOnlyAsync<ProductColor>()
                .QueryAll();

            var productId = await colorQuery
                .Where(x => x.Id == productColorId)
                .Select(x => x.ProductId)
                .FirstOrDefaultAsync();

            ViewBag.ProductColorId = productColorId;
            ViewBag.ProductId = productId;

            return View("Index", model);
        }




        [HttpPost]
        public async Task<IActionResult> SaveProductSize(Guid productColorId, Guid productId,List<string> Sizes,List<int> Quantities)
        {
            for (int i = 0; i < Sizes.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(Sizes[i]))
                    continue;

                var dto = new UpsertProductSizeDto
                {
                    ProductId = productId,
                    ProductColorId = productColorId,
                    Size = Sizes[i],
                    Quantity = Quantities[i]
                };

                await _productSizeService.UpsertAsync(dto);
            }

            return RedirectToAction(
        actionName: "Edit",
        controllerName: "Product",
        routeValues: new { id = productId }
    );
        }
    }
}
