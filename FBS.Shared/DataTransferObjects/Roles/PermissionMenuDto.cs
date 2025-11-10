// <copyright file= PermissionMenuDto.cs company=Tan Nguyen>
// Copyright (c) Tan Nguyen. All rights reserved.
// </copyright>

namespace FBS.Shared.DataTransferObjects.Roles
{
    using FBS.Shared.DataTranferObjects.Base;

    public class PermissionMenuDto
    {
        public PermissionMenuDto()
        {
            MenuItems = new List<PermissionMenuDto>();
            Permission = new PermissionDto();
        }

        public string? MenuKey { get; set; }

        public PermissionDto Permission { get; set; }

        public List<PermissionMenuDto>? MenuItems { get; set; }
    }
}
