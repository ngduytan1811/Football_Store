using FBS.Application.DataTranferObjects.Blog;
using FBS.Application.DataTranferObjects.Products;
using FBS.Application.Services;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.DataTranferObjects.Base;
using FootballShop.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;
using System.Collections.Generic;

namespace FBS.Internal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : BaseAdminController
    {
        private readonly IBlogService _blogService;
        private readonly IProductService _productService;
        public BlogController(
            UserManager<User> userManager,
            IUnitOfWork unitOfWork,
            IBlogService blogService,
            IProductService productService)
            : base(userManager, unitOfWork)
        {
            _blogService = blogService;
            _productService = productService;
        }

       
        public async Task<IActionResult> Index()
        {
            var data = await _blogService.GetBlogs(new BaseSearchDto<BlogSearchDto>());
            ViewData["Blogs"] = data;
            return View();
        }

        [Authorize(Roles = "Baiviet")]
        [Authorize(Policy = "Blog.Creat")]
        public async Task<IActionResult> Create()
        {
            var response = await _productService.GetProducts(
        new BaseSearchDto<ProductSearchDto>()
    );

            ViewBag.Products = response.Items; 

            return View();
        }



        [HttpPost]
        [Authorize(Roles = "Baiviet")]
        [Authorize(Policy = "Blog.Creat")]
        public async Task<IActionResult> Create(BlogSaveDto dto)
        {
            // ghép 2 đoạn nội dung 
            dto.Content = $"{dto.ContentPart1}\n\n<!--IMG-BLOCK-->\n\n{dto.ContentPart2}";

            
            if (dto.ThumbnailFile != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.ThumbnailFile.FileName);

                var folder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot", "uploads", "blog"
                );

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.ThumbnailFile.CopyToAsync(stream);
                }

                dto.Thumbnail = $"uploads/blog/{fileName}";
            }

           
            dto.SubImages = new List<string>();

            if (dto.SubImageFiles != null && dto.SubImageFiles.Count > 0)
            {
                var folder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot", "uploads", "blog"
                );

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                foreach (var file in dto.SubImageFiles)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var fullPath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    dto.SubImages.Add($"uploads/blog/{fileName}");
                }
            }

            // lưu vào data
            await _blogService.CreateBlog(dto);

            return RedirectToAction("Index", "Blog", new { area = "Admin" });
        }

        [Authorize(Roles = "Baiviet")]
        [Authorize(Policy = "Blog.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _blogService.FindById(id);

            if (result.Data == null)
                return RedirectToAction("Index", "Blog", new { area = "Admin" });

            var data = result.Data;

            // tách nội dung
            var parts = (data.Content ?? "").Split("<!--IMG-BLOCK-->");
            string p1 = parts.Length > 0 ? parts[0] : "";
            string p2 = parts.Length > 1 ? parts[1] : "";

            var dto = new BlogSaveDto
            {
                Id = data.Id,
                Title = data.Title,
                Author = data.Author,
                Content = data.Content,
                ContentPart1 = p1,
                ContentPart2 = p2,
                Thumbnail = data.Thumbnail,
                SubImages = data.Images,
                ProductId = data.ProductId  
            };
            var productResponse = await _productService.GetProducts(
    new BaseSearchDto<ProductSearchDto>()
);

            ViewBag.Products = productResponse.Items;
            return View(dto);
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Baiviet")]
        [Authorize(Policy = "Blog.Edit")]
        public async Task<IActionResult> Edit(Guid id, BlogSaveDto dto)
        {
            // lấy bài viết cũ
            var oldBlog = await _blogService.FindById(id);
            if (oldBlog.Data == null)
                return RedirectToAction("Index", "Blog", new { area = "Admin" });

            //ghép content
            dto.Content = $"{dto.ContentPart1}\n\n<!--IMG-BLOCK-->\n\n{dto.ContentPart2}";

            //xử lý 
            if (dto.ThumbnailFile == null)
            {
                dto.Thumbnail = oldBlog.Data.Thumbnail;
            }
            else
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.ThumbnailFile.FileName);
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "blog");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                var fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                    await dto.ThumbnailFile.CopyToAsync(stream);

                dto.Thumbnail = $"uploads/blog/{fileName}";
            }

           //xử lý ảnh phụ
            dto.SubImages = new List<string>(); 

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/blog");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            // ảnh phụ mới
            if (dto.SubImageFiles != null && dto.SubImageFiles.Count > 0)
            {
                foreach (var file in dto.SubImageFiles)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var fullPath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                        await file.CopyToAsync(stream);

                    dto.SubImages.Add($"uploads/blog/{fileName}");
                }
            }

            // ảnh phụ cũ
            if (dto.OldSubImages != null && dto.OldSubImages.Any())
            {
                dto.SubImages.AddRange(dto.OldSubImages);
            }

            // k được chùng ảnh
            dto.SubImages = dto.SubImages.Distinct().ToList();

           // gán ảnh phụ cho blog cũ
            oldBlog.Data.Images = dto.SubImages;

            
            await _blogService.UpdateBlog(id, dto);// lưu vào data

            return RedirectToAction("Index", "Blog", new { area = "Admin" });
        }

        
        [HttpPost]
        [Authorize(Roles = "Baiviet")]
        [Authorize(Policy = "Blog.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _blogService.DeleteBlog(id);
            return RedirectToAction("Index", "Blog", new { area = "Admin" });

        }
    }
}
