using FBS.Application.DataTranferObjects.Cart;
using FBS.Application.Services;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Internal.Utils;
using FBS.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FBS.Internal.Controllers
{
    public class CartController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public CartController(
            UserManager<User> userManager,
            IProductService productService,
            IOrderService orderService,
            IUnitOfWork unitOfWork
        ) : base(userManager, unitOfWork)
        {
            _productService = productService;
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            return View();
        }

        // =============================
        // 🟦 Thêm vào giỏ hàng
        // =============================
        [HttpPost]
        public async Task<IActionResult> AddToCart(CartItemDto request)
        {
            var cart = GetCart();

            var productData = await _productService.FindById(request.ProductId);

            var product = new CartItemDto
            {
                ProductId = request.ProductId,
                ProductName = productData?.Data?.Name,
                Price = productData?.Data?.Price,
                Color = productData?.Data?.Color,
                Size = request.Size
            };

            var existingItem = cart.FirstOrDefault(i => i.ProductId == request.ProductId && i.Size == request.Size);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                product.Quantity = request.Quantity;
                cart.Add(product);
            }

            SaveCart(cart);
            return RedirectToAction("Index", "Cart");
        }

        // =============================
        // 🟥 Xóa sản phẩm khỏi giỏ hàng
        // =============================
        public IActionResult RemoveFromCart(Guid productId)
        {
            var cart = GetCart();

            var itemToRemove = cart.FirstOrDefault(i => i.ProductId == productId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }

        // =============================
        // 🟩 Cập nhật số lượng — thêm hàm này !!!
        // =============================
        [HttpPost]
        public IActionResult UpdateQuantity(Guid productId, string size, int quantity)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(x => x.ProductId == productId && x.Size == size);
            if (item != null)
            {
                if (quantity > 0)
                    item.Quantity = quantity;

                SaveCart(cart);
            }

            return Json(new { success = true });
        }

        // =============================
        // 🟦 Đặt hàng
        // =============================
        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutDto request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Checkout");
            }

            var cart = GetCart();
            request.CartItems = cart.Select(x => new CartItemDto
            {
                ProductId = x.ProductId,
                Color = x.Color,
                Quantity = x.Quantity,
                Price = x.Price,
                Size = x.Size,
            }).ToList();

            var result = await _orderService.CreateOrder(request);
            if (result.Type == GlobalConstants.ResponseType.Success)
            {
                ClearCart();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

    }
}
