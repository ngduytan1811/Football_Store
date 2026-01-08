using FBS.Application.DataTranferObjects.Blog;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.DataTranferObjects.Base;
using FootballShop.Areas.Admin.Controllers;
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

        public BlogController(
            UserManager<User> userManager,
            IUnitOfWork unitOfWork,
            IBlogService blogService)
            : base(userManager, unitOfWork)
        {
            _blogService = blogService;
        }

       
        public async Task<IActionResult> Index()
        {
            var data = await _blogService.GetBlogs(new BaseSearchDto<BlogSearchDto>());
            ViewData["Blogs"] = data;
            return View();
        }

        
        public IActionResult Create()
        {
            return View();
        }

       
        
        [HttpPost]
        public async Task<IActionResult> Create(BlogSaveDto dto)
        {
            // Ghép 2 đoạn nội dung vào 1 
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

            // Lưu vào data
            await _blogService.CreateBlog(dto);

            return RedirectToAction("Index", "Blog", new { area = "Admin" });
        }

        
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _blogService.FindById(id);

            if (result.Data == null)
                return RedirectToAction("Index", "Blog", new { area = "Admin" });

            var data = result.Data;

            // Tách nội dung
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
                SubImages = data.Images
            };

            return View(dto);
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, BlogSaveDto dto)
        {
            // Lấy blog cũ
            var oldBlog = await _blogService.FindById(id);
            if (oldBlog.Data == null)
                return RedirectToAction("Index", "Blog", new { area = "Admin" });

            //ghép content
            dto.Content = $"{dto.ContentPart1}\n\n<!--IMG-BLOCK-->\n\n{dto.ContentPart2}";

            //xử lý thumnailfile
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
        public async Task<IActionResult> Delete(Guid id)
        {
            await _blogService.DeleteBlog(id);
            return RedirectToAction("Index", "Blog", new { area = "Admin" });

        }
    }
}
