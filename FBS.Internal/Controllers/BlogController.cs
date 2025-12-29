using FBS.Application.DataTranferObjects.Blog;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.DataTranferObjects.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FBS.Internal.Controllers
{
    [Route("blogs")]   
    public class BlogController : BaseController
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

       
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var search = new BaseSearchDto<BlogSearchDto>
            {
                Page = 1,
                PageSize = 20,
                SearchParams = new BlogSearchDto()
            };

            var result = await _blogService.GetBlogs(search);
            return View(result.Items);
        }

        
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> Detail(Guid id)
        {
            var result = await _blogService.FindById(id);
            if (result.Data == null) return RedirectToAction("Index");

            return View(result.Data);
        }

    }
}
