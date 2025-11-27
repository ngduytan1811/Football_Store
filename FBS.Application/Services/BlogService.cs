using FBS.API.Responses.Base;
using FBS.Application.DataTranferObjects.Blog;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.DataTranferObjects.Base;
using FBS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.Services
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BlogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseTableResponse<BlogDto>> GetBlogs(BaseSearchDto<BlogSearchDto> dto)
        {
            // Nếu dto null => khởi tạo mặc định
            dto ??= new BaseSearchDto<BlogSearchDto>();

            // Nếu PageSize truyền vào <= 0 => set mặc định
            if (dto.PageSize <= 0)
                dto.PageSize = 20;

            // Nếu Page truyền vào <= 0 => set mặc định
            if (dto.Page <= 0)
                dto.Page = 1;

            // Start = (Page - 1) * PageSize => đã có sẵn trong BaseSearchDto
            int start = dto.Start;

            var repo = _unitOfWork.GetRepositoryReadOnlyAsync<Blog>();
            var query = await repo.QueryAll();

            // SEARCH
            if (!string.IsNullOrEmpty(dto.SearchParams?.Search))
            {
                var key = dto.SearchParams.Search.ToLower();
                query = query.Where(x => x.Title.ToLower().Contains(key));
            }

            // Chuẩn bị response
            var result = new BaseTableResponse<BlogDto>();
            result.Total = query.Count();

            // Lấy danh sách blog
            var items = query
                .OrderByDescending(x => x.CreatedAt)
                .Skip(start)
                .Take(dto.PageSize)
                .Select(x => new BlogDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Author = x.Author,
                    Thumbnail = x.Thumbnail,
                    CreatedAt = x.CreatedAt
                })
                .ToList();

            result.Items = items;
            result.TotalPage = (int)Math.Ceiling(result.Total / (double)dto.PageSize);

            return result;
        }



        public async Task<BaseResponse<BlogDto>> FindById(Guid id)
        {
            var repo = _unitOfWork.GetRepositoryReadOnlyAsync<Blog>();
            var entity = await repo.Single(x => x.Id == id);

            if (entity == null)
                return new BaseResponse<BlogDto>();

            return new BaseResponse<BlogDto>
            {
                Data = new BlogDto
                {
                    Id = entity.Id,
                    Title = entity.Title,
                    Content = entity.Content,
                    Author = entity.Author,
                    Thumbnail = entity.Thumbnail
                }
            };
        }

        public async Task<BaseResponse<string>> CreateBlog(BlogSaveDto dto)
        {
            var repo = _unitOfWork.GetRepositoryAsync<Blog>();

            var blog = new Blog
            {
                Title = dto.Title,
                Content = dto.Content,
                Author = dto.Author,
                Thumbnail = dto.Thumbnail,
                Status = StatusEnum.Active
            };

            await repo.Add(blog);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponse<string>();
        }

        public async Task<BaseResponse<string>> UpdateBlog(Guid id, BlogSaveDto dto)
        {
            var repo = _unitOfWork.GetRepositoryAsync<Blog>();
            var entity = await repo.Single(x => x.Id == id);

            if (entity == null)
                return new BaseResponse<string>();

            entity.Title = dto.Title;
            entity.Content = dto.Content;
            entity.Author = dto.Author;
            entity.Thumbnail = dto.Thumbnail;

            await repo.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponse<string>();
        }

        public async Task<BaseResponse<string>> DeleteBlog(Guid id)
        {
            var repo = _unitOfWork.GetRepositoryAsync<Blog>();
            var entity = await repo.Single(x => x.Id == id);

            if (entity == null)
                return new BaseResponse<string>();

            await repo.Delete(entity);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponse<string>();
        }
    }
}
