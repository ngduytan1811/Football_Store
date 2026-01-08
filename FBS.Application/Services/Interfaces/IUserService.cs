

namespace FBS.Application.Services.Interfaces
{
    using System.Threading.Tasks;
    using FBS.API.Responses.Base;
    using FBS.Application.DataTranferObjects.Users;
    using FBS.Shared.DataTranferObjects.Base;

    public interface IUserService
    {
        Task<BaseTableResponse<UserDto>> GetUsers(BaseSearchDto<UserSearchDto> dto);

        Task<BaseResponse<UserDto>> FindById(Guid userId);

        Task<BaseResponse<string>> CreateUser(UserSaveDto dto);

        Task<BaseResponse<string>> UpdateUser(Guid userId, UserSaveDto dto);

        Task<BaseResponse<string>> DeleteUser(Guid userId);
    }
}
