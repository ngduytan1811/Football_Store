using FBS.Shared.DataTranferObjects.Base;
using FBS.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.DataTranferObjects.Users
{
    public class UserDto : BaseDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        public bool? IsAdmin { get; set; }

        public string? FullName { get; set; }

        public string? Avatar { get; set; }

        public string? AvatarBase64 => FileUploadHelper.GetFileBase64(Avatar);
    }
}
