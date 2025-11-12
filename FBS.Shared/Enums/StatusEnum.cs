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
        [Display(Name = "Không kích hoạt", Order = 1)]
        Inactive = 0,

        /// <summary>
        /// Active
        /// </summary>
        [Display(Name = "Hoạt động", Order = 2)]
        Active = 1,

        /// <summary>
        /// In Handler
        /// </summary>
        [Display(Name = "Đang xử lý", Order = 3)]
        InHandler = 2,

        /// <summary>
        /// Cancel
        /// </summary>
        [Display(Name = "Hủy bỏ", Order = 4)]
        Cancel = 3,

        /// <summary>
        /// Waiting Approval
        /// </summary>
        [Display(Name = "Đợi xác nhận", Order = 5)]
        WaitingApproval = 4,

        /// <summary>
        /// Not seen
        /// </summary>
        [Display(Name = "Chưa xem", Order = 6)]
        NotSeen = 5,

        /// <summary>
        /// Watched
        /// </summary>
        [Display(Name = "Đã xem", Order = 7)]
        Watched = 6,
    }
}
