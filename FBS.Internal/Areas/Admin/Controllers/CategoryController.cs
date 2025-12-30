using FBS.Application.DataTranferObjects.Categories;
using FBS.Application.DataTranferObjects.Users;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.Constants;
using FBS.Shared.DataTranferObjects.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using FootballShop.Areas.Admin.Controllers;

namespace FBS.Internal.Areas.Admin.Controllers
{
    public class CategoryController : BaseAdminController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(
            ICategoryService categoryService,
            UserManager<User> userManager,
            IUnitOfWork unitOfWork
        ) : base(userManager, unitOfWork)
        {
            _categoryService = categoryService;
        }

       
        public async Task<IActionResult> Index(int page = 1)
        {
            var dataSearch = new BaseSearchDto<CategorySearchDto>()
            {
                Page = page,
            };

            var data = await _categoryService.GetCategories(dataSearch);

       
            var startIndex = dataSearch.Start + 1;
            data.Items?.ForEach(i => i.Index = startIndex++);

            ViewData["Categories"] = data;
            return View();
        }

      
        public async Task<IActionResult> Create()
        {
            var dropdown = await _categoryService.GetCategoryDropdown(null);
            ViewData["Categories"] = dropdown?.Data;  

            return View();
        }

      
        [HttpPost]
        public async Task<IActionResult> Create(CategorySaveDto model)
        {
            if (!ModelState.IsValid)
            {
                var dropdown = await _categoryService.GetCategoryDropdown(null);
                ViewData["Categories"] = dropdown?.Data;
                return View(model);
            }

            var result = await _categoryService.CreateCategory(model);

            if (result.Type != GlobalConstants.ResponseType.Success)
                return View(model);

            return RedirectToAction("Index");
        }

       
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            //  tất cả category
            var dropdown = await _categoryService.GetCategoryDropdown(null);
            var list = dropdown?.Data ?? new List<CategoryDto>();

            // lấy dữ liệu khi sửa
            var data = await _categoryService.FindById(id);
            if (data.Data == null)
                return RedirectToAction("Index");

            var model = new CategorySaveDto
            {
                Id = data.Data.Id,
                Name = data.Data.Name,
                Description = data.Data.Description,
                ParentId = data.Data.ParentId
            };

            
            ViewData["Categories"] = list.Where(x => x.Id != id).ToList();

            return View(model);
        }

       
        [HttpPost]
        public async Task<IActionResult> Update(Guid id, CategorySaveDto model)
        {
            if (!ModelState.IsValid)
            {
                var dropdown = await _categoryService.GetCategoryDropdown(null);
                var list = dropdown?.Data ?? new List<CategoryDto>();
                ViewData["Categories"] = list.Where(x => x.Id != id).ToList();

                return View("Edit", model);
            }

            await _categoryService.UpdateCategory(id, model);
            return RedirectToAction("Index");
        }

        
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var category = await _categoryService.FindById(id);
            if (category.Data == null)
                return RedirectToAction("Index");

            await _categoryService.DeleteCategory(id);
            return RedirectToAction("Index");
        }
    }
}
