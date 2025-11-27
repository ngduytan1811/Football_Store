using FBS.API.Responses.Base;
using FBS.Application.DataTranferObjects.Blog;
using FBS.Shared.DataTranferObjects.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.Services.Interfaces
{
    public interface IBlogService
    {
        Task<BaseTableResponse<BlogDto>> GetBlogs(BaseSearchDto<BlogSearchDto> dto);
        Task<BaseResponse<BlogDto>> FindById(Guid id);
        Task<BaseResponse<string>> CreateBlog(BlogSaveDto dto);
        Task<BaseResponse<string>> UpdateBlog(Guid id, BlogSaveDto dto);
        Task<BaseResponse<string>> DeleteBlog(Guid id);
    }
}
