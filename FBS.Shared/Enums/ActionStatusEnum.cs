// <copyright file=  ActionStatusEnum.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.Shared.Enums
{
    public enum ActionStatusEnum
    {
        /// <summary>
        /// The success
        /// </summary>
        Success = 0,

        /// <summary>
        /// The not found
        /// </summary>
        NotFound = 1,

        /// <summary>
        /// The has history
        /// </summary>
        HasHistory = 2,

        /// <summary>
        /// The fail
        /// </summary>
        Fail = 3,

        /// <summary>
        /// The exists
        /// </summary>
        Exists = 4,

        /// <summary>
        /// The user name exists
        /// </summary>
        UserNameExists = 5,

        /// <summary>
        /// The email exists
        /// </summary>
        EmailExists = 6,

        /// <summary>
        /// The phone exists
        /// </summary>
        PhoneExists = 7,

        /// <summary>
        /// The code exists
        /// </summary>
        CodeExists = 8,

        /// <summary>
        /// The password incorrect
        /// </summary>
        PasswordIncorrect = 9,

        /// <summary>
        /// The password less
        /// </summary>
        PasswordLess = 10,
    }
}
