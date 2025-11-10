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
        [Display(Name = "Status.Inactive", Order = 1)]
        Inactive = 0,

        /// <summary>
        /// Active
        /// </summary>
        [Display(Name = "Status.Active", Order = 2)]
        Active = 1,

        /// <summary>
        /// In Handler
        /// </summary>
        [Display(Name = "Status.Inprogress", Order = 3)]
        InHandler = 2,

        /// <summary>
        /// Cancel
        /// </summary>
        [Display(Name = "Status.Cancel", Order = 4)]
        Cancel = 3,

        /// <summary>
        /// Waiting Approval
        /// </summary>
        [Display(Name = "Status.Waiting_Approval", Order = 5)]
        WaitingApproval = 4,

        /// <summary>
        /// Not seen
        /// </summary>
        [Display(Name = "Status.NotSeen", Order = 6)]
        NotSeen = 5,

        /// <summary>
        /// Watched
        /// </summary>
        [Display(Name = "Status.Watched", Order = 7)]
        Watched = 6,
    }
}
