
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
                Image = data.Data.Image,
                Name = data.Data.Name,
                Description = data.Data.Description,
                Price = data.Data.Price,
                Sizes = data.Data.Sizes,
                Color = data.Data.Color,
                Status = data.Data.Status,
                CategoryId = data.Data.CategoryId,
                Brand = data.Data.Brand
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(Guid id, ProductSaveDto request)
        {
            // Lấy sản phẩm hiện tại
            var current = await _productService.FindById(id);
            if (current.Data == null)
                return RedirectToAction("Index");

            // Nếu có upload ảnh mới
            if (request.ImageFile != null)
            {
                // Tạo tên file mới
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.ImageFile.FileName);

                // Đường dẫn thư mục lưu ảnh (bạn đang dùng theme/client/img/product)
                var folderPath = Path.Combine(
     Directory.GetCurrentDirectory(),
     "wwwroot",
     "theme",
     "client",
     "img",
     "product"
 );


                // Nếu thư mục chưa tồn tại → tạo
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fullPath = Path.Combine(folderPath, fileName);

                // Lưu file vào thư mục
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await request.ImageFile.CopyToAsync(stream);
                }

                // Gán tên file ảnh mới vào DTO
                request.Image = fileName;
            }
            else
            {
                // Nếu không upload ảnh mới → giữ ảnh cũ
                request.Image = current.Data.Image;
            }

            // Cập nhật sản phẩm
            var result = await _productService.UpdateProduct(id, request);

            if (result.Type == GlobalConstants.ResponseType.Success)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Edit", new { id = id });
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
