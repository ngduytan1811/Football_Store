using FBS.Internal.Models;
using FBS.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FBS.Infrastructure.Repositories.Interfaces;

namespace FBS.Internal.Controllers
{
    public class UserController : BaseController
    {
        public UserController(UserManager<User> userManager, IUnitOfWork unitOfWork)
            : base(userManager, unitOfWork)
        {
        }

        // HIỂN THỊ HỒ SƠ
        public IActionResult Profile()
        {
            return View(CurrentUser);
        }

        // FORM EDIT
        public IActionResult Edit()
        {
            return View(CurrentUser);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CurrentUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            // ✔ UPDATE USER (chỉ Email + Phone)
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            await _userManager.UpdateAsync(user);

            // ✔ UPDATE MEMBER
            var repo = _unitOfWork.GetRepositoryAsync<Member>();
            var member = await repo.Single(x => x.UserId == user.Id);

            if (member != null)
            {
                member.FirstName = model.FirstName;
                member.LastName = model.LastName;
                member.PhoneNumber = model.PhoneNumber;
                member.Address = model.Address;

            }
            await repo.Update(member);

            await _unitOfWork.SaveChangesAsync();

            TempData["Success"] = "Cập nhật thông tin thành công!";

            // ✔ RESET Cache trong BaseController
            HttpContext.Items["ResetUserCache"] = true;

            return RedirectToAction("Profile");
        }
    }
}
