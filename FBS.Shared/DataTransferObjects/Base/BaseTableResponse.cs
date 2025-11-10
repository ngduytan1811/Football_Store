// <copyright file=  BaseTableResponse.cs company= Tan Nguyen>
// Copyright (c) Tan Nguyen. All rights reserved.
// </copyright>

namespace FBS.API.Responses.Base
{
    using System.Collections.Generic;

    public class BaseTableResponse<T>
    {
        public List<T>? Items { get; set; }

        public int Total { get; set; }

        public int TotalPage { get; set; }

        public string? Type { get; set; }

        public string Message { get; set; }
    }
}
