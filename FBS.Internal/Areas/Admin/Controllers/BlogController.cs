using FBS.Application.DataTranferObjects.Blog;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.DataTranferObjects.Base;
using FootballShop.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FBS.Internal.Areas.Admin.Controllers
{
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

        // ============================
        // LIST
        // ============================
        public async Task<IActionResult> Index()
        {
            var data = await _blogService.GetBlogs(new BaseSearchDto<BlogSearchDto>());
            ViewData["Blogs"] = data;
            return View();
        }

        // ============================
        // CREATE (GET)
        // ============================
        public IActionResult Create()
        {
            return View();
        }

        // ============================
        // CREATE (POST)
        // ============================
        [HttpPost]
        public async Task<IActionResult> Create(BlogSaveDto dto)
        {
            // Xử lý upload ảnh trước khi lưu
            if (dto.ThumbnailFile != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.ThumbnailFile.FileName);

                var folder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "blog"
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

            await _blogService.CreateBlog(dto);
            return RedirectToAction("Index");
        }

        // ============================
        // EDIT (GET)
        // ============================
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _blogService.FindById(id);

            if (result.Data == null)
                return RedirectToAction("Index");

            var dto = new BlogSaveDto
            {
                Id = result.Data.Id,
                Title = result.Data.Title,
                Content = result.Data.Content,
                Author = result.Data.Author,
                Thumbnail = result.Data.Thumbnail
            };

            return View(dto);
        }

        // ============================
        // EDIT (POST)
        // ============================
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, BlogSaveDto dto)
        {
            // Nếu upload ảnh mới → xử lý ảnh
            if (dto.ThumbnailFile != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.ThumbnailFile.FileName);

                var folder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "blog"
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

            await _blogService.UpdateBlog(id, dto);

            return RedirectToAction("Index");
        }

        // ============================
        // DELETE
        // ============================
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _blogService.DeleteBlog(id);
            return RedirectToAction("Index");
        }
    }
}
