using FBS.Application.DataTranferObjects.Cart;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Internal.Models;
using FBS.Internal.Utils;
using Microsoft.AspNetCore.Http;
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
                {
                    return _currentUser;
                }

                _currentUser = GetCurrentUserAsync().GetAwaiter().GetResult();
                return _currentUser;
            }
        }


        protected async Task<CurrentUserViewModel?> GetCurrentUserAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    var queryMember = await _unitOfWork.GetRepositoryReadOnlyAsync<Member>().QueryAll();
                    var member = queryMember.FirstOrDefault(x => x.UserId == user.Id);
                    return new CurrentUserViewModel
                    {
                        UserName = user.UserName,
                        PhoneNumber = member?.PhoneNumber,
                        IsAdmin = user.IsAdmin,
                        FirstName = member?.FirstName,
                        LastName = member?.LastName,
                    };
                }
            }
            return null;
        }

        protected string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var cart = GetCart();
            ViewData["Cart"] = cart;

            var user = CurrentUser;

            if (user != null)
            {
                ViewData["CurrentUser"] = user;
            }
            else
            {
                ViewData["CurrentUser"] = null;
            }
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
