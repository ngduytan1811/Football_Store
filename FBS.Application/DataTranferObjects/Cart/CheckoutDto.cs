using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.DataTranferObjects.Cart
{
    public class CheckoutDto
    {
        [Required(ErrorMessage ="Họ tên là bắt buộc")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        public string? Address { get; set; }

        public string? Note { get; set; }

        public List<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();
    }
}
