

namespace FBS.Application.Services
{
    using FBS.API.Responses.Base;
    using FBS.Application.DataTranferObjects.Categories;
    using FBS.Application.DataTranferObjects.Users;
    using FBS.Application.Services.Interfaces;
    using FBS.Infrastructure.Entities;
    using FBS.Infrastructure.Repositories.Interfaces;
    using FBS.Shared.Constants;
    using FBS.Shared.DataTranferObjects.Base;
    using FBS.Shared.Enums;
    using FBS.Shared.Helpers;
    using FBS.Shared.Utilities;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;
    using System;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public UserService(
            IUnitOfWork unitOfWork,
            UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<BaseTableResponse<UserDto>> GetUsers(BaseSearchDto<UserSearchDto> dto)
        {
            var result = new BaseTableResponse<UserDto>();

            var queryUser = await _unitOfWork.GetRepositoryReadOnlyAsync<User>().QueryAll();
            queryUser = queryUser.Include(x => x.Member).Where(x => !x.IsAdmin);

            if (dto.SearchParams != null)
            {
                var dataSearch = dto.SearchParams ?? new UserSearchDto();

                if (!string.IsNullOrEmpty(dataSearch.UserName))
                {
                    queryUser = queryUser.Where(x => !string.IsNullOrEmpty(x.UserName) && x.UserName.Trim().ToLower().Contains(dataSearch.UserName.Trim().ToLower()));
                }

                if (!string.IsNullOrEmpty(dataSearch.Email))
                {
                    queryUser = queryUser.Where(x => !string.IsNullOrEmpty(x.Email) && x.Email.Trim().ToLower().Contains(dataSearch.Email.Trim().ToLower()));
                }
            }

            result.Total = queryUser.Count();

            var query = queryUser.Select(x => new UserDto
            {
                Id = x.Id,
                Username = x.UserName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
             
                CreatedAt = x.CreatedAt,
                FullName = x.IsAdmin ? "Admin" : $"{x.Member.FirstName} {x.Member.LastName}",
                IsActive = x.IsActive

            });

            query = dto.ColumnSort switch
            {
                ColumnNames.CreatedAt => dto.Asc ? query.OrderBy(x => x.CreatedAt) : query.OrderByDescending(x => x.CreatedAt),
                _ => query,
            };

            var (items, totalPage) = TableResponseHelper.MakeToList(query, result.Total, dto.Start, dto.PageSize);

            result.Items = items;
            result.TotalPage = totalPage;

            return result;
        }
        public async Task<BaseResponse<UserDto>> FindById(Guid userId)
        {
            var result = new BaseResponse<UserDto>();

            var queryUser = await _unitOfWork.GetRepositoryReadOnlyAsync<User>().QueryAll();

            var user = queryUser.Include(x => x.UserRoles).Where(x => x.Id == userId).FirstOrDefault();
            if (user == null)
            {
                return result;
            }

            var data = new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Status = user.Status,
                IsActive = user.IsActive
            };

            result.Data = data;
            return result;
        }
        public async Task<BaseResponse<string>> CreateUser(UserSaveDto dto)
        {
            var result = new BaseResponse<string>();

            var errMessage = await CheckUserExists(dto);

            if (!string.IsNullOrEmpty(errMessage))
            {
                result.Type = GlobalConstants.ResponseType.Error;
                result.Message = errMessage;
                return result;
            }

            var newUser = new User
            {
                UserName = dto.UserName,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Status = StatusEnum.Active,
                CreatedAt = DateTime.Now,
            };

            var defaultPassword = PasswordUtils.GeneratePassword(8);
            var resultCreate = await _userManager.CreateAsync(newUser, defaultPassword);

            if (resultCreate != null && resultCreate.Succeeded)
            {
                var memberRep = _unitOfWork.GetRepositoryAsync<Member>();
                var newMember = new Member
                {
                    PhoneNumber = dto.PhoneNumber,
                    Address = dto.Address,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    UserId = newUser.Id
                };

                await memberRep.Add(newMember);
                await _unitOfWork.SaveChangesAsync();
            }

            return result;
        }
        public async Task<BaseResponse<string>> ResetPassword(string email)
        {
            var result = new BaseResponse<string>();

            var userRep = _unitOfWork.GetRepositoryAsync<User>();

            var user = await userRep.Single(x => x.Email == email);

            var passwordReset = PasswordUtils.GeneratePassword(8);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resultResetPassword = await _userManager.ResetPasswordAsync(user, token, passwordReset);
                if (!resultResetPassword.Succeeded)
                {
                    return result;
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return result;
        }
        public async Task<BaseResponse<string>> UpdateUser(Guid userId, UserSaveDto dto)
        {
            var result = new BaseResponse<string>();

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return result;
            }

            var errorMessage = await CheckUserExists(dto, userId);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.UpdatedAt = DateTime.Now;
           
            user.IsActive = dto.IsActive;


            var resultUpdate = await _userManager.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return result;
        }
        public async Task<BaseResponse<string>> DeleteUser(Guid userId)
        {
            var result = new BaseResponse<string>();

            var userRep = _unitOfWork.GetRepositoryAsync<User>();

            var user = await userRep.Single(x => x.Id == userId);
            if (user == null)
            {
                return result;
            }

            await userRep.Delete(user);

            await _unitOfWork.SaveChangesAsync();

            return result;
        }
        private async Task<string?> CheckUserExists(UserSaveDto dto, Guid? userId = null)
        {
            var queryUser = await _unitOfWork.GetRepositoryReadOnlyAsync<User>().QueryAll();
            if (userId.HasValue)
            {
                queryUser = queryUser.Where(x => x.Id != userId);
            }

            if (!string.IsNullOrEmpty(dto.UserName) && queryUser.Where(x => !string.IsNullOrEmpty(x.UserName) && x.UserName.Equals(dto.UserName, StringComparison.OrdinalIgnoreCase)).Any())
            {
                return "Tài khoản đã tồn tại";
            }

            if (!string.IsNullOrEmpty(dto.Email) && queryUser.Where(x => !string.IsNullOrEmpty(x.Email) && x.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase)).Any())
            {
                return "Email đã tồn tại";
            }

            return string.Empty;
        }
    }
}
