using FBS.Application.DataTranferObjects.Categories;
using FBS.Application.DataTranferObjects.Products;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.Constants;
using FBS.Shared.DataTranferObjects.Base;
using FootballShop.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static FBS.Shared.Constants.ContactConstants;

namespace FBS.Internal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductController : BaseAdminController
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private Guid? categoryId;

        public ProductController(
            IProductService productService,
            ICategoryService categoryService,
            UserManager<User> userManager,
            IUnitOfWork unitOfWork
        ) : base(userManager, unitOfWork)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

       
        public async Task<IActionResult> Index(int page = 1, Guid? categoryId = null)
        {
            var dataSearch = new BaseSearchDto<ProductSearchDto>()
            {
                Page = page,
                SearchParams = new ProductSearchDto()
                {
                    CategoryId = categoryId
                }
            };

            var data = await _productService.GetProducts(dataSearch);
            var startIndex = dataSearch.Start + 1;

            data.Items?.ForEach(i => i.Index = startIndex++);

            ViewData["Products"] = data;
            ViewData["Page"] = page;

            var drop = await _categoryService.GetCategoryDropdown();
            var parentCategories = drop.Data.Where(x => x.ParentId == null).ToList();

            ViewData["Categories"] = parentCategories;


            // Giữ danh mục đã chọn
            ViewData["SelectedCategoryId"] = categoryId;

            return View();
        }

        [Authorize(Roles = "Quanlysanpham")]
        [Authorize(Policy = "Product.Create")]
        public async Task<IActionResult> Create()
        {
            var drop = await _categoryService.GetCategoryDropdown();
            ViewData["Categories"] = drop?.Data;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Quanlysanpham")]
        [Authorize(Policy = "Product.Create")]
        public async Task<IActionResult> Create(ProductSaveDto request)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Categories"] = (await _categoryService.GetCategoryDropdown()).Data;
                return View(request);
            }
            Console.WriteLine("=== DEBUG SUB IMAGES ===");
            Console.WriteLine("SubImageFiles = " + (request.SubImageFiles?.Count ?? 0));

          // update ảnh chính
            if (request.ImageFile != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(request.ImageFile.FileName);

                var folder = Path.Combine(Directory.GetCurrentDirectory(),
                                          "wwwroot", "theme", "client", "img", "product");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await request.ImageFile.CopyToAsync(stream);
                }

                request.Image = fileName;
            }

            // update ảnh phụ
            if (request.SubImageFiles != null && request.SubImageFiles.Count > 0)
            {
                request.SubImages = new List<string>();

                var folder = Path.Combine(Directory.GetCurrentDirectory(),
                                          "wwwroot", "theme", "client", "img", "product");

                foreach (var file in request.SubImageFiles)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var fullPath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    request.SubImages.Add(fileName);
                }
            }

            var result = await _productService.CreateProduct(request);

            if (result.Type != GlobalConstants.ResponseType.Success)
            {
                ViewData["Categories"] = (await _categoryService.GetCategoryDropdown()).Data;
                return View(request);
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Quanlysanpham")]
        [Authorize(Policy = "Product.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            ViewData["Categories"] = (await _categoryService.GetCategoryDropdown()).Data;

            var res = await _productService.FindById(id);
            if (res.Data == null)
                return RedirectToAction("Index");

            var dto = new ProductSaveDto
            {
                Id = res.Data.Id,
                Image = res.Data.Image,
                Name = res.Data.Name,
                Description = res.Data.Description,
                Detail = res.Data.Detail,
                Price = res.Data.Price,
                Discount = res.Data.Discount,
                Color = res.Data.Color,
                Sizes = res.Data.Sizes,
                Status = res.Data.Status,
                CategoryId = res.Data.CategoryId,
                Brand = res.Data.Brand,
                SubImages = res.Data.SubImages ,
           
                
            };
            if (!string.IsNullOrEmpty(res.Data.Detail))
            {
               
                var parts = res.Data.Detail.Split(
                    new[] { "\n\n" },
                    StringSplitOptions.None
                );

                dto.DetailPart1 = parts.Length > 0 ? parts[0] : null;
                dto.DetailPart2 = parts.Length > 1 ? parts[1] : null;
            }

            return View(dto);
        }

   
        [HttpPost]
        [Authorize(Roles = "Quanlysanpham")]
        [Authorize(Policy = "Product.Edit")]
        public async Task<IActionResult> Update(Guid id, ProductSaveDto request)
        {
            var current = await _productService.FindById(id);
            if (current.Data == null)
                return RedirectToAction("Index");

           // xử lý ảnh phụ
            var finalImages = new List<string>();

            request.SubImageFiles = request.SubImageFiles ?? new List<IFormFile>();
            request.OldSubImages = request.OldSubImages ?? new List<string>();

            for (int i = 0; i < 3; i++)
            {
                var oldImg = request.OldSubImages.Count > i ? request.OldSubImages[i] : null;
                var newFile = request.SubImageFiles.Count > i ? request.SubImageFiles[i] : null;

                if (newFile != null)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(newFile.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(),
                                            "wwwroot", "theme", "client", "img", "product",
                                            fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await newFile.CopyToAsync(stream);
                    }

                    finalImages.Add(fileName);
                }
                else
                {
                    finalImages.Add(oldImg);
                }
            }


            request.SubImages = finalImages;

            // update product
            var result = await _productService.UpdateProduct(id, request, finalImages);

            if (result.Type == GlobalConstants.ResponseType.Success)
                return RedirectToAction("Index");

            return RedirectToAction("Edit", new { id });
        }
        
        [HttpPost]
        [Authorize(Roles = "Quanlysanpham")]
        [Authorize(Policy = "Product.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _productService.FindById(id);
            if (product.Data == null)
                return RedirectToAction("Index");

            await _productService.DeleteProduct(id);

            return RedirectToAction("Index");
        }
    }
}
