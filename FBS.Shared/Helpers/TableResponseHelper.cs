// <copyright file=  TableResponseHelper.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.Shared.Helpers
{
    public static class TableResponseHelper
    {
        public static (List<T> Items, int TotalPage) MakeToList<T>(
        IQueryable<T> query,
        int total,
        int start,
        int pageSize)
        {
            if (pageSize <= 0)
            {
                pageSize = 10;
            }

            if (start < 0)
            {
                start = 0;
            }

            var totalPage = total > 0
                ? (int)Math.Ceiling(total / (double)pageSize)
                : 0;

            var items = query.Skip(start).Take(pageSize).ToList();

            return (items, totalPage);
        }
    }
}
