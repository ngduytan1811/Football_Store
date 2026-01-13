using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FBS.Application.DataTranferObjects.Cart
{
    public class CheckoutDto
    {
       

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [MinLength(3, ErrorMessage = "Họ tên quá ngắn, vui lòng nhập lại")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^(0|\+84)[0-9]{9}$",
                 ErrorMessage = "Số điện thoại không hợp lệ, vui lòng nhập lại")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [MinLength(10, ErrorMessage = "Địa chỉ quá ngắn, vui lòng nhập lại")]
        public string? Address { get; set; }

        public string? Note { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        public string PaymentMethod { get; set; }
        public string? QRCodeUrl { get; set; }

        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }

        public List<CartItemDto> CartItems { get; set; } = new();
    }

}
