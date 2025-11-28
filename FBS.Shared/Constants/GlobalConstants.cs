// <copyright file=  GlobalConstants.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.Shared.Constants
{
    public static class GlobalConstants
    {
        public const string PasswordDefault = "G123@";
        public const int DefaultPageSize = 20;

        public static class ResponseType
        {
            public const string Error = "error";
            public const string Success = "success";
        }

        public static class ValidatorErrorCode
        {
            public const string Required = "Required";
            public const string Invalid = "Invalid";
            public const string IsCodeValid = "IsCodeValid";
        }

        public static class PrefixCode
        {
            public const string Category = "CATE";
            public const string Project = "PRO";
        }

        public static readonly List<string> Brands = new List<string>
        {
            "Adidas",
            "Nike",
            "Puma",
            "Mizuno",
            "Wika",
            "Kamito",
            "Joma"
        };

        public static readonly List<string> Color = new List<string>
        {
            "Đen",
            "Đỏ",
            "Xanh",
            "Trắng",
            "Tím",
            "Vàng",
            

        };
    }
}
