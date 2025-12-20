// <copyright file=  StatusEnum.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.Shared.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum StatusEnum
    {
        /// <summary>
        /// In active
        /// </summary>
        [Display(Name = "Chờ Xác Nhận", Order = 1)]
        Inactive = 0,

        /// <summary>
        /// Active
        /// </summary>
        [Display(Name = "Đang Xử Lý", Order = 2)]
        Active = 1,

        /// <summary>
        /// In Handler
        /// </summary>
        [Display(Name = "Đang Giao", Order = 3)]
        InHandler = 2,

        /// <summary>
        /// Cancel
        /// </summary>
        [Display(Name = "Giao Thành Công", Order = 4)]
        WaitingApproval = 3,

        /// <summary>
        /// Wợi xác nhận
        /// </summary>
        [Display(Name = "Đã Hủy", Order = 5)]
         Cancel = 4,

        /// <summary>
        /// Not seen
        /// </summary>
        [Display(Name = "Trả Hàng", Order = 6)]
        NotSeen = 5,

        /// <summary>
        /// Watched
        /// </summary>
        [Display(Name = "Thất Bại", Order = 7)]
        Watched = 6,
    }
}
