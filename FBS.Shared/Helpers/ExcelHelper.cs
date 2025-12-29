namespace FBS.Shared.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using Microsoft.AspNetCore.Http;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    public static class ExcelHelper
    {
        public static void HandleComboboxExcel(this ExcelWorksheet sheet, string columnName, int startRow, int totalRow, List<string> listValue)
        {
            if (startRow > 0 && listValue.Count > 0)
            {
                for (int i = 0; i <= totalRow; i++)
                {
                    var val = sheet.DataValidations.AddListValidation(columnName + (startRow + i));
                    for (int j = 0; j < listValue.Count; j++)
                    {
                        val.Formula.Values.Add(listValue[j]);
                    }
                }
            }
        }

        public static void HandleComboboxExcel(this ExcelWorksheet sheet, string columnName, int startRow, int totalRow, string formulaRangeData)
        {
            for (int i = 0; i <= totalRow; i++)
            {
                var val = sheet.DataValidations.AddListValidation(columnName + (startRow + i));
                val.Formula.ExcelFormula = formulaRangeData;
            }
        }

        public static void RenderBorderAll(this ExcelWorksheet sheet, int startRow, int startColumn, int endRow, int endColumn, ExcelBorderStyle style)
        {
            sheet.Cells[startRow, startColumn, endRow, endColumn].Style.Border.Top.Style = style;
            sheet.Cells[startRow, startColumn, endRow, endColumn].Style.Border.Left.Style = style;
            sheet.Cells[startRow, startColumn, endRow, endColumn].Style.Border.Right.Style = style;
            sheet.Cells[startRow, startColumn, endRow, endColumn].Style.Border.Bottom.Style = style;
        }

        public static void RenderBorderTop(this ExcelWorksheet sheet, int startRow, int startColumn, int endRow, int endColumn, ExcelBorderStyle style)
        {
            sheet.Cells[startRow, startColumn, endRow, endColumn].Style.Border.Top.Style = style;
        }

        public static void RenderBorderBottom(this ExcelWorksheet sheet, int startRow, int startColumn, int endRow, int endColumn, ExcelBorderStyle style)
        {
            sheet.Cells[startRow, startColumn, endRow, endColumn].Style.Border.Bottom.Style = style;
        }

        public static void RenderBorderLeft(this ExcelWorksheet sheet, int startRow, int startColumn, int endRow, int endColumn, ExcelBorderStyle style)
        {
            sheet.Cells[startRow, startColumn, endRow, endColumn].Style.Border.Left.Style = style;
        }

        public static void RenderBorderRight(this ExcelWorksheet sheet, int startRow, int startColumn, int endRow, int endColumn, ExcelBorderStyle style)
        {
            sheet.Cells[startRow, startColumn, endRow, endColumn].Style.Border.Right.Style = style;
        }

        public static Stream? ToStream(this ExcelPackage excelPackage, FileInfo newFile)
        {
            if (excelPackage != null)
            {
                excelPackage.SaveAs(newFile);
                string fullPath = newFile.FullName;
                MemoryStream memoryStream;
                using (FileStream fileStream = File.OpenRead(fullPath))
                {
                    memoryStream = new MemoryStream();
                    memoryStream.SetLength(fileStream.Length);
                    fileStream.Read(memoryStream.GetBuffer(), 0, (int)fileStream.Length);
                }

                FileInfo fi = new FileInfo(fullPath);
                fi.Delete();
                return memoryStream;
            }

            return null;
        }

        public static Stream? GetAsStream(this ExcelPackage excelPackage, FileInfo file)
        {
            if (excelPackage != null)
            {
                string fullPath = file.FullName;
                MemoryStream memoryStream;
                using (FileStream fileStream = File.OpenRead(fullPath))
                {
                    memoryStream = new MemoryStream();
                    memoryStream.SetLength(fileStream.Length);
                    fileStream.Read(memoryStream.GetBuffer(), 0, (int)fileStream.Length);
                }

                return memoryStream;
            }

            return null;
        }

        public static bool IsCellNullOrWhiteSpace(this ExcelWorksheet sheet, int row, int column)
        {
            return sheet.GetValue(row, column) == null || string.IsNullOrWhiteSpace(sheet.GetValue(row, column)?.ToString());
        }

        public static void MarkErrorCell(this ExcelWorksheet sheet, int row, int column)
        {
            sheet.Cells[row, column].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Red);
        }

        public static string GetColumnName(int orderColumn)
        {
            return ((char)(orderColumn + 65)).ToString();
        }

        public static void HighlightCells(this ExcelWorksheet sheet, int startRow, int startColumn, int endRow, int endColumn, ExcelFillStyle fillStyle, Color fillColor)
        {
            sheet.Cells[startRow, startColumn, endRow, endColumn].Style.Fill.PatternType = fillStyle;
            sheet.Cells[startRow, startColumn, endRow, endColumn].Style.Fill.BackgroundColor.SetColor(fillColor);
        }

        public static void Merge(this ExcelWorksheet sheet, int row, int col, int row2, int col2)
        {
            sheet.SelectedRange[row, col, row2, col2].Merge = true;
        }

        public static bool HasNoData(this IFormFile file, int firstDataRowIndex = 2)
        {
            using (var pck = new ExcelPackage(file.OpenReadStream()))
            {
                var sheet = pck.Workbook.Worksheets[0];

                return sheet.IsEndFile(firstDataRowIndex);
            }
        }

        public static int ToColumnIndex(this string columnName)
        {
            var index = 0;
            var power = 0;
            for (int i = columnName.Length - 1; i >= 0; i--)
            {
                index += (int)((columnName[i] - 64) * Math.Pow(26, power));
                power++;
            }

            return index;
        }

        public static bool IsEndFile(this ExcelWorksheet sheet, int currentRow)
        {
            for (int i = currentRow; i < currentRow + 10; i++)
            {
                if (sheet.GetValue(i, 1) != null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
