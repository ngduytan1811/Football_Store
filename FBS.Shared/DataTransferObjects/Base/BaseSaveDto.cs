// <copyright file=  BaseSaveDto.cs company= Tan Nguyen>
// Copyright (c) Tan Nguyen. All rights reserved.
// </copyright>

namespace FBS.Shared.DataTranferObjects.Base
{
    using FBS.Shared.Enums;

    public class BaseSaveDto
    {
        public StatusEnum? Status { get; set; } = StatusEnum.Active;
    }
}
