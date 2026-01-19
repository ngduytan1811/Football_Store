using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FootballShop.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FBS.Internal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactController : BaseAdminController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContactController(
            UserManager<User> userManager,
            IUnitOfWork unitOfWork
        ) : base(userManager, unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

       
        public async Task<IActionResult> Index(string? keyword)
        {
            var repo = _unitOfWork.GetRepositoryAsync<Contact>();

            var contacts = await repo.FindByAsync(x => !x.IsDeleted);

            bool empty = false;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim().ToLower();

                contacts = contacts
                    .Where(x =>
                    (
                        (
                            ((x.FirstName ?? "") + " " + (x.LastName ?? "")).ToLower()
                        )
                        .Contains(keyword)
                    )
                    ||
                    (
                        x.Phone != null && x.Phone.Contains(keyword)
                    ))
                    .ToList();

                if (contacts.Count == 0)
                    empty = true;
            }

            ViewBag.Keyword = keyword;
            ViewBag.Empty = empty;

            return View(contacts
                .OrderByDescending(x => x.CreatedAt)
                .ToList());
        }

        [Authorize(Roles = "Lienhe")]
        [Authorize(Policy = "Contact.Manage")]
        public async Task<IActionResult> Detail(Guid id)
        {
            var repo = _unitOfWork.GetRepositoryAsync<Contact>();

            var contact = await repo.FindById(id);

            if (contact == null || contact.IsDeleted)
                return NotFound();

            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Lienhe")]
        [Authorize(Policy = "Contact.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var repo = _unitOfWork.GetRepositoryAsync<Contact>();

            var contact = await repo.FindById(id);
            if (contact == null || contact.IsDeleted)
                return NotFound();

            contact.IsDeleted = true;

            await repo.Update(contact);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa liên hệ thành công!";
            return RedirectToAction(nameof(Index));
        }

    }
}
