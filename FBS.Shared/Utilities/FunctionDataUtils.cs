// <copyright file=  FunctionDataUtils.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.Shared.Utilities
{
    using System.Reflection;

    public static class FunctionDataUtils
    {
        public static string? LookupResource(IReflect resourceManagerProvider, string resourceKey)
        {
            foreach (var staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (staticProperty.PropertyType != typeof(System.Resources.ResourceManager))
                {
                    continue;
                }

                var resourceManager = staticProperty?.GetValue(null, null) as System.Resources.ResourceManager;

                return resourceManager?.GetString(resourceKey);
            }

            return resourceKey; // Fallback with the key name
        }

        public static string ExcelRange(int lastColumnIndex, int rowIndex)
        {
            return FunctionDataUtils.ExcelRange(1, lastColumnIndex, rowIndex, rowIndex);
        }

        public static string ExcelRange(int firstColumnIndex, int lastColumnIndex, int rowIndex)
        {
            return FunctionDataUtils.ExcelRange(firstColumnIndex, lastColumnIndex, rowIndex, rowIndex);
        }

        public static string ExcelRange(int firstColumnIndex, int lastColumnIndex, int firstRowIndex, int lastRowIndex)
        {
            var firstRow = firstRowIndex.ToString();
            var lastRow = lastRowIndex.ToString();

            var firstColumn = char.ConvertFromUtf32(firstColumnIndex + 64);
            if (firstColumnIndex > 26)
            {
                var leftChar = char.ConvertFromUtf32((firstColumnIndex / 26) + 64);
                var rightChar = char.ConvertFromUtf32((firstColumnIndex % 26) + 64);
                firstColumn = leftChar + rightChar;
            }

            var lastColumn = char.ConvertFromUtf32(lastColumnIndex + 64);
            if (lastColumnIndex > 26)
            {
                var leftChar = char.ConvertFromUtf32((lastColumnIndex / 26) + 64);
                var rightChar = char.ConvertFromUtf32((lastColumnIndex % 26) + 64);
                lastColumn = leftChar + rightChar;
            }

            return $"{firstColumn}{firstRow}:{lastColumn}{lastRow}";
        }
    }
}
