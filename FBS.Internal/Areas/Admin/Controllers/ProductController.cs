using FBS.Application.DataTranferObjects.Categories;
using FBS.Application.DataTranferObjects.Products;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.Constants;
using FBS.Shared.DataTranferObjects.Base;
using FootballShop.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FBS.Internal.Areas.Admin.Controllers
{
    public class ProductController : BaseAdminController
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

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

        // ============================
        // INDEX
        // ============================
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

        // ============================
        // CREATE
        // ============================
        public async Task<IActionResult> Create()
        {
            var drop = await _categoryService.GetCategoryDropdown();
            ViewData["Categories"] = drop?.Data;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductSaveDto request)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Categories"] = (await _categoryService.GetCategoryDropdown()).Data;
                return View(request);
            }
            Console.WriteLine("=== DEBUG SUB IMAGES ===");
            Console.WriteLine("SubImageFiles = " + (request.SubImageFiles?.Count ?? 0));

            // ====== UPLOAD ẢNH CHÍNH ======
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

            // ====== UPLOAD ẢNH PHỤ ======
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

        // ============================
        // EDIT
        // ============================
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
                Color = res.Data.Color,
                Sizes = res.Data.Sizes,
                Status = res.Data.Status,
                CategoryId = res.Data.CategoryId,
                Brand = res.Data.Brand,
                SubImages = res.Data.SubImages ,
           
                
            };

            return View(dto);
        }

        // ============================
        // UPDATE
        // ============================
        [HttpPost]
        public async Task<IActionResult> Update(Guid id, ProductSaveDto request)
        {
            var current = await _productService.FindById(id);
            if (current.Data == null)
                return RedirectToAction("Index");

            // ====== ẢNH CHÍNH ======
            if (request.ImageFile != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(request.ImageFile.FileName);

                var folder = Path.Combine(Directory.GetCurrentDirectory(),
                                          "wwwroot", "theme", "client", "img", "product");

                var fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await request.ImageFile.CopyToAsync(stream);
                }

                request.Image = fileName;
            }
            else
            {
                request.Image = current.Data.Image; // giữ ảnh cũ
            }

            // ====== ẢNH PHỤ ======
            var newImages = new List<string>();

            if (request.SubImageFiles != null && request.SubImageFiles.Count > 0)
            {
                foreach (var file in request.SubImageFiles)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

                    var path = Path.Combine(Directory.GetCurrentDirectory(),
                                            "wwwroot", "theme", "client", "img", "product",
                                            fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    newImages.Add(fileName);
                }
            }
            request.SubImages = newImages; 
            request.OldSubImages = request.OldSubImages ?? new List<string>();

            var result = await _productService.UpdateProduct(id, request, newImages);


            if (result.Type == GlobalConstants.ResponseType.Success)
                return RedirectToAction("Index");

            return RedirectToAction("Edit", new { id });
        }

        // ============================
        // DELETE
        // ============================
        [HttpPost]
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
