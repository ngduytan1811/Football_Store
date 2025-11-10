// <copyright file=  SelectListItemDto.cs company= Tan Nguyen>
// Copyright (c) Tan Nguyen. All rights reserved.
// </copyright>

namespace FBS.Shared.DataTranferObjects.Base
{
    public class SelectListItemDto
    {
        public bool Disabled { get; set; }

        public bool Selected { get; set; }

        public string? Text { get; set; }

        public string? Value { get; set; }
    }
}
