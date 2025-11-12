using AngleSharp.Io;
using FBS.Application.DataTranferObjects.Categories;
using FBS.Application.DataTranferObjects.Users;
using FBS.Application.Services;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.Constants;
using FBS.Shared.DataTranferObjects.Base;
using FootballShop.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Threading.Tasks;

namespace FBS.Internal.Areas.Admin.Controllers
{
    public class UserController : BaseAdminController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var request = new BaseSearchDto<UserSearchDto>();
            var users = await _userService.GetUsers(request);
            var startIndex = request.Start + 1;
            users.Items?.ForEach(i => i.Index = startIndex++);
            ViewData["Users"] = users;

            return View();
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserSaveDto request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var result = await _userService.CreateUser(request);
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

        public async Task<IActionResult> Edit(Guid userId)
        {
            var user = await _userService.FindById(userId);
            if (user.Data == null)
            {
                return View();
            }

            var model = new UserSaveDto
            {
                Id = user.Data.Id,
                UserName = user.Data.Username,
                PhoneNumber = user.Data.PhoneNumber,
                Email = user.Data.Email,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid userId, UserSaveDto request)
        {
            var user = await _userService.FindById(userId);
            if (user.Data == null)
            {
                return View();
            }

            var result = await _userService.UpdateUser(userId, request);
            if (result.Type == GlobalConstants.ResponseType.Success)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Edit", "User", new { userId = userId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid userId)
        {
            var user = await _userService.FindById(userId);
            if (user.Data == null)
            {
                return RedirectToAction("Index");
            }

            var result = await _userService.DeleteUser(userId);
            if (result.Type == GlobalConstants.ResponseType.Success)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}
