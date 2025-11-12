using FBS.Application.DataTranferObjects.Categories;
using FBS.Application.Services.Interfaces;
using FBS.Shared.Constants;
using FootballShop.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FBS.Internal.Areas.Admin.Controllers
{
    public class CategoryController : BaseAdminController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
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

        public IActionResult Edit()
        {
            return View();
        }
    }
}
