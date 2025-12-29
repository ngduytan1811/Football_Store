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

        
        protected async Task<CurrentUserViewModel?> GetCurrentUserAsync()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return null;

            
            var memberRepo = _unitOfWork.GetRepositoryReadOnlyAsync<Member>();
            var memberQuery = await memberRepo.QueryAll();
            var member = memberQuery.FirstOrDefault(x => x.UserId == user.Id);

           
            if (member == null)
            {
                var writeRepo = _unitOfWork.GetRepositoryAsync<Member>();
                member = new Member
                {
                    UserId = user.Id,
                    FirstName = "Unknown",
                    LastName = "",
                    Address = "",
                    PhoneNumber = user.PhoneNumber ?? "",
                    IsActive = true,
                    Status = StatusEnum.Active,
                    CreatedAt = DateTime.Now
                };

                await writeRepo.Add(member);
                await _unitOfWork.SaveChangesAsync();
            }

            return new CurrentUserViewModel
            {
                UserId = user.Id,
                CustomerId = member.Id,
                UserName = user.UserName,

             
                FirstName = member.FirstName,
                LastName = member.LastName,
                PhoneNumber = member.PhoneNumber,
                Address = member.Address,

                
                Email = user.Email,

                IsAdmin = user.IsAdmin
            };
        }


        protected string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

       
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
          
            if (TempData.ContainsKey("ResetUserCache"))
            {
                _currentUser = null;
            }

            
            _currentUser = null;

         
            ViewBag.CurrentUser = CurrentUser;

           
            ViewData["Cart"] = GetCart();

          
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
