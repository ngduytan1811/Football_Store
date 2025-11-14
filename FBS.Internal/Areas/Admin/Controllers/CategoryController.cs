using AngleSharp.Io;
using FBS.Application.DataTranferObjects.Categories;
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
    public class CategoryController : BaseAdminController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService, UserManager<User> userManager, IUnitOfWork unitOfWork) : base(userManager, unitOfWork)
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
            var data = await _categoryService.GetCategoryDropdown();
            ViewData["Categogries"] = data?.Data;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategorySaveDto request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var result = await _categoryService.CreateCategory(request);
                if (result.Type != GlobalConstants.ResponseType.Success)
                {
                    return View();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var dataDrop = await _categoryService.GetCategoryDropdown();
            ViewData["Categogries"] = dataDrop?.Data;

            var data = await _categoryService.FindById(id);
            if (data.Data == null)
            {
                return RedirectToAction("Create");
            }

            var model = new CategorySaveDto
            {
                Id = data.Data.Id,
                Name = data.Data.Name,
                Description = data.Data.Description,
                ParentId = data.Data.ParentId,
                Status = data.Data.Status,
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(Guid id, CategorySaveDto request)
        {
            var user = await _categoryService.FindById(id);
            if (user.Data == null)
            {
                return View();
            }

            var result = await _categoryService.UpdateCategory(id, request);
            if (result.Type == GlobalConstants.ResponseType.Success)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Edit", "Category", new { id = id });
        }


        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var category = await _categoryService.FindById(id);
            if (category.Data == null)
            {
                return RedirectToAction("Index");
            }

            var result = await _categoryService.DeleteCategory(id);
            if (result.Type == GlobalConstants.ResponseType.Success)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}
