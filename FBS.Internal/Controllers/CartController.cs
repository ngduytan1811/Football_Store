using FBS.Application.DataTranferObjects.Cart;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
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

        // ============================================
        // 🛒 TRANG GIỎ HÀNG
        // ============================================
        public IActionResult Index()
        {
            var cart = GetCart();
            ViewData["Cart"] = cart;
            return View();
        }

        // ============================================
        // 🟦 TRANG CHECKOUT
        // ============================================
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = GetCart();
            ViewData["Cart"] = cart;
            return View(new CheckoutDto());
        }

        // ============================================
        // ➕ THÊM VÀO GIỎ HÀNG
        // ============================================
        [HttpPost]
        public async Task<IActionResult> AddToCart(CartItemDto request)
        {
            var cart = GetCart();

            var productData = await _productService.FindById(request.ProductId);

            var newItem = new CartItemDto
            {
                ProductId = request.ProductId,
                
                ProductName = productData?.Data?.Name,
                Price = productData?.Data?.Price,
                Color = productData?.Data?.Color,
                Size = request.Size,
                Image = productData?.Data?.Image,
                Quantity = request.Quantity
                
            };

            // Tìm item theo product + size
            var existingItem = cart.FirstOrDefault(i =>
                i.ProductId == request.ProductId && i.Size == request.Size);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                cart.Add(newItem);
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // ============================================
        // ❌ XÓA SP KHỎI GIỎ
        // ============================================
        public IActionResult RemoveFromCart(Guid productId)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }

        // ============================================
        // 🔄 CẬP NHẬT SỐ LƯỢNG (ĐÃ FIX CHUẨN)
        // ============================================
        [HttpPost]
        public IActionResult UpdateQuantity(Guid productId, int quantity)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
                SaveCart(cart);
            }

            return Json(new { success = true });
        }

        // ============================================
        // ✔ HOÀN TẤT ĐẶT HÀNG
        // ============================================
        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutDto request)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Cart"] = GetCart();
                return View(request);
            }

            var cart = GetCart();

            // 🔥 GÁN CUSTOMER ID VÀ EMAIL TỪ USER ĐANG ĐĂNG NHẬP
            request.CustomerId = CurrentUser.CustomerId;
            request.Email = CurrentUser.Email;

            request.CartItems = cart.Select(x => new CartItemDto
            {
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                Price = x.Price,
                Size = x.Size,
                Color = x.Color,
                Image = x.Image
               
            }).ToList();

            var result = await _orderService.CreateOrder(request);

            ClearCart();
            TempData["OrderSuccess"] = "Bạn đã đặt hàng thành công!";

            return RedirectToAction("Index", "Cart");
        }

    }
}
