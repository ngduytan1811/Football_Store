// <copyright file=  CurrentUserDto.cs company= Tan Nguyen>
// Copyright (c) Tan Nguyen. All rights reserved.
// </copyright>

namespace FBS.Shared.DataTranferObjects.Auth
{
    using FBS.Shared.DataTranferObjects.Base;
    using FBS.Shared.DataTransferObjects.Roles;

    public class CurrentUserDto : BaseDto
    {
        public string? FullName { get; set; }

        public bool IsAdmin { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string Name { get; set; }

        public string? Avatar { get; set; }

        public List<PermissionMenuDto>? PermissionMenus { get; set; }
    }
}
