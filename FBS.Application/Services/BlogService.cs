using FBS.API.Responses.Base;
using FBS.Application.DataTranferObjects.Blog;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.DataTranferObjects.Base;
using FBS.Shared.Enums;
using Microsoft.EntityFrameworkCore;
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
            
            dto ??= new BaseSearchDto<BlogSearchDto>();
           
            if (dto.PageSize <= 0)
                dto.PageSize = 20;
            
            if (dto.Page <= 0)
                dto.Page = 1;
            
            int start = dto.Start;

            var repo = _unitOfWork.GetRepositoryReadOnlyAsync<Blog>();
            var query = await repo.QueryAll();

         
            if (!string.IsNullOrEmpty(dto.SearchParams?.Search))
            {
                var key = dto.SearchParams.Search.ToLower();
                query = query.Where(x => x.Title.ToLower().Contains(key));
            }

            
            var result = new BaseTableResponse<BlogDto>();
            result.Total = query.Count();

           
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
            var entity = await repo.Single(x => x.Id == id, 
                include:q =>q.Include(b =>b.Images));

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
                    ProductId = entity.ProductId,
                    Thumbnail = entity.Thumbnail,
                    Images = entity.Images != null
                ? entity.Images.Select(i => i.Image).ToList()
                : new List<string>(),
                    CreatedAt =entity.CreatedAt

                }
            };
        }

        public async Task<BaseResponse<string>> CreateBlog(BlogSaveDto dto)
        {
            var repoBlog = _unitOfWork.GetRepositoryAsync<Blog>();
            var repoBlogImg = _unitOfWork.GetRepositoryAsync<BlogImage>();

            var blog = new Blog
            {   
                
                Title = dto.Title,
                Content = dto.Content,
                Author = dto.Author,
                Thumbnail = dto.Thumbnail,
                ProductId =dto.ProductId,
                Status = StatusEnum.Active,
                CreatedAt = DateTime.Now
            };

            await repoBlog.Add(blog);
            await _unitOfWork.SaveChangesAsync();

           
            if (dto.SubImages != null && dto.SubImages.Count > 0)
            {
                foreach (var img in dto.SubImages)
                {
                    await repoBlogImg.Add(new BlogImage
                    {
                        BlogId = blog.Id,
                        Image = img
                    });
                }

                await _unitOfWork.SaveChangesAsync();
            }

            return new BaseResponse<string>
            {
                Type = "Success",
                Message = "Tạo bài viết thành công"
            };
        }


        public async Task<BaseResponse<string>> UpdateBlog(Guid id, BlogSaveDto dto)
        {
            var repoBlog = _unitOfWork.GetRepositoryAsync<Blog>();
            var repoBlogImg = _unitOfWork.GetRepositoryAsync<BlogImage>();

            
            var blog = await repoBlog.Single(
                x => x.Id == id,
                include: q => q.Include(b => b.Images),
                disableTracking: false
            );

            if (blog == null)
            {
                return new BaseResponse<string>
                {
                    Type = "Error",
                    Message = "Không tìm thấy bài viết"
                };
            }



            blog.ProductId = dto.ProductId;
            blog.Title = dto.Title;
            blog.Content = dto.Content;
            blog.Author = dto.Author;
            blog.Thumbnail = dto.Thumbnail;
            blog.UpdatedAt = DateTime.Now;

            await _unitOfWork.SaveChangesAsync();


            
            var dbImages = blog.Images.Select(x => x.Image).ToList();
            var newList = dto.SubImages ?? new List<string>();

           
            var removeList = dbImages.Except(newList).ToList();
            foreach (var img in removeList)
            {
                var entity = await repoBlogImg.Single(x => x.Image == img);
                if (entity != null)
                    await repoBlogImg.Delete(entity);
            }

            
            var addList = newList.Except(dbImages).ToList();
            foreach (var img in addList)
            {
                await repoBlogImg.Add(new BlogImage
                {
                    BlogId = id,
                    Image = img
                });
            }

            await _unitOfWork.SaveChangesAsync();

            return new BaseResponse<string>
            {
                Type = "Success",
                Message = "Cập nhật bài viết thành công!"
            };
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
