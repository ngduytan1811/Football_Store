using AngleSharp.Io;
using FBS.Application.DataTranferObjects.Categories;
using FBS.Application.DataTranferObjects.Products;
using FBS.Application.DataTranferObjects.Users;
using FBS.Application.Services;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.Constants;
using FBS.Shared.DataTranferObjects.Base;
using FootballShop.Areas.Admin.Controllers;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FBS.Internal.Areas.Admin.Controllers
{
    public class ProductController : BaseAdminController
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService, UserManager<User> userManager, IUnitOfWork unitOfWork) : base(userManager, unitOfWork)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var dataSearch = new BaseSearchDto<ProductSearchDto>()
            {
                Page = page,
            };

            var data = await _productService.GetProducts(dataSearch);
            var startIndex = dataSearch.Start + 1;
            data.Items?.ForEach(i => i.Index = startIndex++);
            ViewData["Products"] = data;
            return View();
        }

        public async Task<IActionResult> Create()
        {
            var dataDrop = await _categoryService.GetCategoryDropdown();
            ViewData["Categogries"] = dataDrop?.Data;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductSaveDto request)
        {
            if (!ModelState.IsValid)
            {
                var dataDrop = await _categoryService.GetCategoryDropdown();
                ViewData["Categories"] = dataDrop?.Data;
                
                return View(request);
            }

            try
            {
                var result = await _productService.CreateProduct(request);
                if (result.Type != GlobalConstants.ResponseType.Success)
                {
                    return View(request);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var dataDrop = await _categoryService.GetCategoryDropdown();
                ViewData["Categories"] = dataDrop?.Data;
                return View(request);
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var dataDrop = await _categoryService.GetCategoryDropdown();
            ViewData["Categogries"] = dataDrop?.Data;

            var data = await _productService.FindById(id);
            if (data.Data == null)
            {
                return RedirectToAction("Create");
            }

            var model = new ProductSaveDto
            {
                Id = data.Data.Id,
                Name = data.Data.Name,
                Description = data.Data.Description,
                Price = data.Data.Price,
                Sizes = data.Data.Sizes,
                Color = data.Data.Color,
                Status = data.Data.Status,
                CategoryId = data.Data.CategoryId,
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(Guid id, ProductSaveDto request)
        {
            var user = await _productService.FindById(id);
            if (user.Data == null)
            {
                return View();
            }

            var result = await _productService.UpdateProduct(id, request);
            if (result.Type == GlobalConstants.ResponseType.Success)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Edit", "Product", new { id = id });
        }


        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _productService.FindById(id);
            if (product.Data == null)
            {
                return RedirectToAction("Index");
            }

            var result = await _productService.DeleteProduct(id);
            if (result.Type == GlobalConstants.ResponseType.Success)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}
