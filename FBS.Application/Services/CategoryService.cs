// <copyright file= CatgoryService.cs company= Tan Nguyen>
// Copyright (c) Tan Nguyen. All rights reserved.
// </copyright>

namespace FBS.Application.Services
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using FBS.API.Responses.Base;
    using FBS.Application.DataTranferObjects.Categories;
    using FBS.Application.Services.Interfaces;
    using FBS.Infrastructure.Entities;
    using FBS.Infrastructure.Repositories.Interfaces;
    using FBS.Shared.Constants;
    using FBS.Shared.DataTranferObjects.Base;
    using FBS.Shared.Enums;
    using FBS.Shared.Helpers;

    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseTableResponse<CategoryDto>> GetCategories(BaseSearchDto<CategorySearchDto> dto)
        {
            var result = new BaseTableResponse<CategoryDto>();
            var queryCategory = await _unitOfWork.GetRepositoryReadOnlyAsync<Category>().QueryAll();

            var searchData = dto.SearchParams ?? new CategorySearchDto();

            if (!string.IsNullOrEmpty(searchData?.Name))
            {
                var formattedName = searchData.Name.Trim();
                queryCategory = queryCategory.Where(i => i.Name.Contains(formattedName, StringComparison.OrdinalIgnoreCase));
            }

            result.Total = queryCategory.Count();

            var query = queryCategory.Select(category => new CategoryDto
            {
                Id = category.Id,
                Status = category.Status,
                Name = category.Name,
                Description = category.Description,
                Logo = category.Logo,
                CreatedAt = category.CreatedAt,
            });

            query = dto.ColumnSort switch
            {
                ColumnNames.Order => dto.Asc ? query.OrderBy(i => i.Order) : query.OrderByDescending(i => i.Order),
                ColumnNames.CreatedAt => dto.Asc ? query.OrderBy(i => i.CreatedAt) : query.OrderByDescending(i => i.CreatedAt),
                _ => query,
            };

            var (items, totalPage) = TableResponseHelper.MakeToList(query, result.Total, dto.Start, dto.PageSize);

            result.Items = items;
            result.TotalPage = totalPage;

            return result;
        }

        public async Task<BaseResponse<CategoryDto>> FindById(Guid categoryId)
        {
            var result = new BaseResponse<CategoryDto>();

            var queryCategory = await _unitOfWork.GetRepositoryReadOnlyAsync<Category>().QueryAll();

            var category = queryCategory.FirstOrDefault(i => i.Id == categoryId);

            if (category == null)
            {
                return result;
            }

            result.Data = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Logo = category.Logo,
                Status = category.Status,
            };

            return result;
        }

        public async Task<BaseResponse<string>> CreateCategory(CategorySaveDto dto)
        {
            var result = new BaseResponse<string>();
            var categoryRep = _unitOfWork.GetRepositoryAsync<Category>();

            var newCategory = new Category
            {
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                ParentId = dto.ParentId,
                Status = StatusEnum.Active,
            };

            await categoryRep.Add(newCategory);

            await _unitOfWork.SaveChangesAsync();

            return result;
        }

        public async Task<BaseResponse<string>> UpdateCategory(Guid categoryId, CategorySaveDto dto)
        {
            var result = new BaseResponse<string>();

            var categoryRep = _unitOfWork.GetRepositoryAsync<Category>();

            var category = await categoryRep.Single(x => x.Id == categoryId);

            if (category == null)
            {
                return result;
            }


            category.Name = dto.Name.Trim();
            category.ParentId = dto.ParentId;
            category.Status = dto.Status;
            category.Description = dto.Description;

            await categoryRep.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return result;
        }

        public async Task<BaseResponse<string>> DeleteCategory(Guid categoryId)
        {
            var result = new BaseResponse<string>();

            var categoryRep = _unitOfWork.GetRepositoryAsync<Category>();

            var category = await categoryRep.Single(x => x.Id == categoryId);

            if (category == null)
            {
                return result;
            }

            await categoryRep.Delete(category);

            await _unitOfWork.SaveChangesAsync();

            return result;
        }
    }
}
