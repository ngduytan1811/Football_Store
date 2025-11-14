using FBS.Shared.DataTranferObjects.Base;
using System.ComponentModel.DataAnnotations;

namespace FBS.Application.DataTranferObjects.Users
{
    public class UserSaveDto : BaseSaveDto
    {
        [Required(ErrorMessage ="Tên tài khoản là bắt buộc")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Địa chỉ email là bắt buộc")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Họ là bắt buộc")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Tên là bắt buộc")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
    }
}
