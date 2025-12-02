using FBS.Application.DataTranferObjects.Cart;
using FBS.Application.DataTranferObjects.Categories;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Internal.Models;
using FBS.Internal.Utils;
using FBS.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace FBS.Internal.Controllers
{
    public class BaseController : Controller
    {
        private const string CartSessionKey = "CartSession";

        protected readonly UserManager<User> _userManager;
        protected readonly IUnitOfWork _unitOfWork;

        private CurrentUserViewModel? _currentUser;

        public BaseController(UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        // LẤY USER HIỆN TẠI (có cache)
        protected CurrentUserViewModel? CurrentUser
        {
            get
            {
                if (_currentUser != null)
                    return _currentUser;

                _currentUser = GetCurrentUserAsync().GetAwaiter().GetResult();
                return _currentUser;
            }
        }

        // LẤY DỮ LIỆU USER TỪ DB
        protected async Task<CurrentUserViewModel?> GetCurrentUserAsync()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return null;

            // Lấy Customer theo UserId
            var customerRepo = _unitOfWork.GetRepositoryReadOnlyAsync<Customer>();
            var customerQuery = await customerRepo.QueryAll();
            var customer = customerQuery.FirstOrDefault(x => x.UserId == user.Id);

            // Nếu chưa có Customer thì tạo mới
            if (customer == null)
            {
                var writeRepo = _unitOfWork.GetRepositoryAsync<Customer>();
                customer = new Customer
                {
                    UserId = user.Id,
                    FirstName = "Unknown",
                    LastName = "",
                    Address = "",
                    PhoneNumber = user.PhoneNumber ?? "0000000000",
                    Email = user.Email ?? "noemail@example.com",
                    IsActive = true,
                    Status = StatusEnum.Active,
                    CreatedAt = DateTime.Now
                };

                await writeRepo.Add(customer);
                await _unitOfWork.SaveChangesAsync();
            }


            return new CurrentUserViewModel
            {
                UserId = user.Id,
                CustomerId = customer.Id,       // <-- BẮT BUỘC PHẢI GÁN
                UserName = user.UserName,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                IsAdmin = user.IsAdmin
            };
        }


        protected string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // CHẠY TRƯỚC MỖI ACTION
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Reset cache user để luôn lấy dữ liệu mới nhất
            _currentUser = null;

            // Load lại thông tin user
            ViewBag.CurrentUser = CurrentUser;

            // Load Cart
            ViewData["Cart"] = GetCart();

            // Load danh mục
            var query = await _unitOfWork.GetRepositoryReadOnlyAsync<Category>().QueryAll();
            var allCategories = query.Where(x => x.IsActive).Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.Name,
                ParentId = x.ParentId,
                Status = x.Status
            }).ToList();

            var rootCategories = allCategories.Where(x => !x.ParentId.HasValue).ToList();

            foreach (var item in rootCategories)
                item.Items = allCategories.Where(x => x.ParentId == item.Id).ToList();

            ViewBag.Categories = rootCategories;

            await next();
        }

        // CART FUNCTION
        protected List<CartItemDto> GetCart()
        {
            var cart = HttpContext.Session.Get<List<CartItemDto>>(CartSessionKey);
            return cart ?? new List<CartItemDto>();
        }

        protected void SaveCart(List<CartItemDto> cart)
        {
            HttpContext.Session.Set(CartSessionKey, cart);
        }

        protected void ClearCart()
        {
            HttpContext.Session.Remove(CartSessionKey);
        }
    }
}
