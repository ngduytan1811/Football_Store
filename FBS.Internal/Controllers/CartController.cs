
using FBS.Application.DataTranferObjects.Cart;
using FBS.Application.Services;
using FBS.Application.Services.Interfaces;
using FBS.Infrastructure.Entities;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Internal.Models;
using FBS.Internal.Utils;
using FBS.Shared.Constants;
using FBS.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FBS.Internal.Controllers
{
    public class CartController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private int totalQuantity;

        private readonly VietQRService _vietQRService;

        public CartController(
            UserManager<User> userManager,
            IProductService productService,
            IOrderService orderService,
            IUnitOfWork unitOfWork,
            VietQRService vietQRService
        ) : base(userManager, unitOfWork)
        {
            _productService = productService;
            _orderService = orderService;
            _vietQRService = vietQRService;
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
            decimal shipping = 30000m;
            decimal subtotal = cart.Sum(x => (x.Price ?? 0m) * x.Quantity);
            decimal total = subtotal + shipping;

            var model = new CheckoutDto
            {
                CartItems = cart,
                SubTotal = subtotal,
                ShippingFee = shipping,
                TotalAmount = total
            };

            return View(model);
        }




        [HttpPost]
        public async Task<IActionResult> AddToCart(CartItemDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Size))
            {
              
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
            if (totalQuantity > 10)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Số lượng tối đa là 10 sản phẩm. Vui lòng liên hệ shop." });

                TempData["Error"] = "Số lượng tối đa là 10 sản phẩm. Vui lòng liên hệ shop.";
                return RedirectToAction("Detail", "Product", new { id = request.ProductId });
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
            var cart = GetCart();

            if (!cart.Any())
            {
                TempData["Error"] = "Giỏ hàng trống";
                return RedirectToAction("Index", "Home");
            }

            request.CartItems = cart;
            CalculateAmount(request, cart);

            if (!ModelState.IsValid)
                return View(request);

            // ===== COD =====
            if (request.PaymentMethod == "COD")
            {
                await _orderService.CreateOrder(request);
                ClearCart();

                TempData["OrderSuccess"] = "Đặt hàng COD thành công!";
                return RedirectToAction("Index", "Home");
            }

            if (request.PaymentMethod == "VietQR")
            {
                // 👉 Tạo order trạng thái Pending
                var order = await _orderService.CreatePendingOrder(request);

                request.QRCodeUrl = await _vietQRService.GenerateVietQRAsync(
                    accountNo: "1026869227",
                    accountName: "NGUYEN TRUONG GIANG",
                    bank: BankEnum.Vietcombank,
                    amount: request.TotalAmount,
                    note: $"DH-{order.OrderCode}"
                );

                return View("Checkout", request);
            }

            return RedirectToAction("Checkout");
        }

        private void CalculateAmount(CheckoutDto request, List<CartItemDto> cart)
        {
            const decimal SHIPPING_FEE = 30000m;

            request.SubTotal = cart.Sum(x => (x.Price ?? 0m) * x.Quantity);
            request.ShippingFee = SHIPPING_FEE;
            request.TotalAmount = request.SubTotal + SHIPPING_FEE;
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
