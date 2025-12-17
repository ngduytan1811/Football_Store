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

       
        public IActionResult Index()
        {
            var cart = GetCart();
            ViewData["Cart"] = cart;
            return View();
        }

      
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
            if (string.IsNullOrWhiteSpace(request.Size))
            {
                // ❌ AJAX thì trả JSON lỗi
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Vui lòng chọn size" });

                TempData["SizeError"] = "Vui lòng chọn size";
                return RedirectToAction("Detail", "Product", new { id = request.ProductId });
            }

            var cart = GetCart();

            var productData = await _productService.FindById(request.ProductId);
            if (productData?.Data == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Sản phẩm không tồn tại" });

                TempData["Error"] = "Sản phẩm không tồn tại";
                return RedirectToAction("Index", "Product");
            }

            var existingItem = cart.FirstOrDefault(i =>
                i.ProductId == request.ProductId && i.Size == request.Size);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                cart.Add(new CartItemDto
                {
                    ProductId = request.ProductId,
                    ProductName = productData.Data.Name,
                    Price = productData.Data.Price ?? 0m,
                    Color = productData.Data.Color,
                    Size = request.Size,
                    Image = productData.Data.Image,
                    Quantity = request.Quantity,
                    Description = productData.Data.Description
                });
            }

            SaveCart(cart);

          
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = true,
                    count = cart.Sum(x => x.Quantity),
                    subTotal = cart.Sum(x => (x.Price ?? 0m) * x.Quantity)
                });
            }

            // ✅ Form submit → redirect như cũ
            return RedirectToAction("Index");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(Guid productId, string size)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(x =>
                x.ProductId == productId &&
                x.Size == size
            );

            if (item == null)
            {
                return Json(new { success = false });
            }

            cart.Remove(item);
            SaveCart(cart);

            return Json(new
            {
                success = true,
                count = cart.Sum(x => x.Quantity),
                subTotal = cart.Sum(x => (x.Price ?? 0m) * x.Quantity)
            });


            return RedirectToAction("Index");
        }




        [HttpPost]
        public IActionResult UpdateQuantity(Guid productId, string size, int quantity)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(x =>
                x.ProductId == productId && x.Size == size);

            if (item != null && quantity > 0 && quantity <= 20)
            {
                item.Quantity = quantity;
                SaveCart(cart);
            }

            return Json(new { success = true });
        }


       
        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutDto request)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Cart"] = GetCart();
                return View(request);
            }

            var cart = GetCart();
            if (!cart.Any())
            {
                TempData["Error"] = "Giỏ hàng trống";
                return RedirectToAction("Index");
            }

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

        [HttpGet]
        public IActionResult GetCartCount()
        {
            var cart = GetCart();
            var count = cart.Sum(x => x.Quantity);
            return Json(new { count });
        }
        [HttpGet]
        public IActionResult SideCart()
        {
            ViewData["Cart"] = GetCart();
            return PartialView("_CartSidebar");
        }
       

    }
}
