// <copyright file= ICategoryService.cs company= Tan Nguyen>
// Copyright (c) Tan Nguyen. All rights reserved.
// </copyright>

namespace FBS.Application.Services.Interfaces
{
    using System.Threading.Tasks;
    using FBS.API.Responses.Base;
    using FBS.Application.DataTranferObjects.Categories;
    using FBS.Shared.DataTranferObjects.Base;

    public interface ICategoryService
    {
        Task<BaseTableResponse<CategoryDto>> GetCategories(BaseSearchDto<CategorySearchDto> dto);

        Task<BaseResponse<List<CategoryDto>>> GetCategoryDropdown();

        Task<BaseResponse<CategoryDto>> FindById(Guid categoryId);

        Task<BaseResponse<string>> CreateCategory(CategorySaveDto dto);

        Task<BaseResponse<string>> UpdateCategory(Guid categoryId, CategorySaveDto dto);

        Task<BaseResponse<string>> DeleteCategory(Guid categoryId);
    }
}
