// <copyright file=  ExtensionMethodUtils.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.Shared.Utilities
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using Microsoft.AspNetCore.Http;
    using FBS.Shared.Constants;
    using FBS.Shared.Enums;
    using Microsoft.AspNetCore.Http.Internal;

    public static class ExtensionMethodUtils
    {
        /// <summary>
        /// Convert datetime to string.
        /// </summary>
        /// <param name="dateTime">dateTime.</param>
        /// <returns>String value width format: dd/MM/yyyy.</returns>
        public static string ConvertToString(this DateTime dateTime)
        {
            return dateTime.ToString(DateConstants.DateFormat);
        }

        /// <summary>
        /// Convert datetime to string.
        /// </summary>
        /// <param name="dateTime">dateTime.</param>
        /// <param name="def">default: dd/MM/yyyy HH:mm.</param>
        /// <returns>String value width format: dd/MM/yyyy HH:mm.</returns>
        public static string ConvertToTime(this DateTime dateTime, string def = DateConstants.DateTimeFormat)
        {
            return dateTime.ToString(def);
        }

        /// <summary>
        /// Convert string to datetime.
        /// </summary>
        /// <param name="input">input.</param>
        /// <param name="format">format default: dd/MM/yyyy.</param>
        /// <returns>Datetime value.</returns>
        public static DateTime ConvertToDateTime(this string input, string format = DateConstants.DateFormat)
        {
            try
            {
                return DateTime.ParseExact(input, format, CultureInfo.InvariantCulture);
            }
            catch
            {
                return default;
            }
        }

        public static string GetDayOfWeek(this string dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DateConstants.Monday: return DateConstants.MondayVi;
                case DateConstants.Tuesday: return DateConstants.TuesdayVi;
                case DateConstants.Wednesday: return DateConstants.WednesdayVi;
                case DateConstants.Thursday: return DateConstants.ThursdayVi;
                case DateConstants.Friday: return DateConstants.FridayVi;
                case DateConstants.Saturday: return DateConstants.SaturdayVi;
                case DateConstants.Sunday: return DateConstants.SundayVi;
                default: return string.Empty;
            }
        }

        public static string GetDayOfWeek(this DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday: return DateConstants.MondayVi;
                case DayOfWeek.Tuesday: return DateConstants.TuesdayVi;
                case DayOfWeek.Wednesday: return DateConstants.WednesdayVi;
                case DayOfWeek.Thursday: return DateConstants.ThursdayVi;
                case DayOfWeek.Friday: return DateConstants.FridayVi;
                case DayOfWeek.Saturday: return DateConstants.SaturdayVi;
                case DayOfWeek.Sunday: return DateConstants.SundayVi;
                default: return string.Empty;
            }
        }

        public static string ToTime24Format(int totalMinutes)
        {
            var hours = totalMinutes / 60;
            var minutes = totalMinutes % 60;
            var hoursAsString = hours < 10 ? $"0{hours}" : $"{hours}";
            var minutesAsString = minutes < 10 ? $"0{minutes}" : $"{minutes}";
            return $"{hoursAsString}:{minutesAsString}";
        }

        public static string FormatTimeSent(this DateTime dateTime)
        {
            var dateNow = DateTime.Now;
            if (dateNow.Year == dateTime.Year)
            {
                if (dateNow.Date == dateTime.Date)
                {
                    return dateTime.ToString("HH:mm");
                }

                return dateTime.ToString("dd/MM HH:mm");
            }

            return dateTime.ToString("dd/MM/yyyy HH:mm");
        }

        /// <summary>
        /// Convert number to Roman Number. Only from 1 to 3999.
        /// </summary>
        /// <param name="number">number.</param>
        /// <returns>String value present Roman Numerals.
        public static string ToRoman(this int number)
        {
            if ((number < 0) || (number > 3999))
            {
                throw new ArgumentOutOfRangeException("Insert value betwheen 1 and 3999");
            }

            if (number < 1)
            {
                return string.Empty;
            }

            if (number >= 1000)
            {
                return "M" + ToRoman(number - 1000);
            }

            if (number >= 900)
            {
                return "CM" + ToRoman(number - 900);
            }

            if (number >= 500)
            {
                return "D" + ToRoman(number - 500);
            }

            if (number >= 400)
            {
                return "CD" + ToRoman(number - 400);
            }

            if (number >= 100)
            {
                return "C" + ToRoman(number - 100);
            }

            if (number >= 90)
            {
                return "XC" + ToRoman(number - 90);
            }

            if (number >= 50)
            {
                return "L" + ToRoman(number - 50);
            }

            if (number >= 40)
            {
                return "XL" + ToRoman(number - 40);
            }

            if (number >= 10)
            {
                return "X" + ToRoman(number - 10);
            }

            if (number >= 9)
            {
                return "IX" + ToRoman(number - 9);
            }

            if (number >= 5)
            {
                return "V" + ToRoman(number - 5);
            }

            if (number >= 4)
            {
                return "IV" + ToRoman(number - 4);
            }

            if (number >= 1)
            {
                return "I" + ToRoman(number - 1);
            }

            throw new ArgumentOutOfRangeException("something bad happened");
        }

        /// <summary>
        /// Convert number to Excel Column Name. Example: 1 to A, 2 to B, 27 to AA.
        /// </summary>
        /// <param name="number">number.</param>
        /// <returns>String value present Excel Column Name.
        public static string ToExcelColumnName(this int number)
        {
            var dividend = number;
            var columnName = string.Empty;
            while (dividend > 0)
            {
                var modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        /// <summary>
        /// Remove delicate space characters.
        /// </summary>
        /// <param name="input">string value.</param>
        /// <returns>String value has been removed space characters.</returns>
        public static string RemoveDuplicateSpaceCharacters(this string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return Regex.Replace(input, @"\s+", " ");
            }

            return string.Empty;
        }

        public static string RemoveSignedCharacters(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            List<string> signedCharacters = new List<string>
            {
                "à", "á", "ạ", "ả", "ã", "â", "ầ", "ấ", "ậ", "ẩ",
                "ẫ", "ă", "ằ", "ắ", "ặ", "ẳ", "ẵ", "è", "é", "ẹ", "ẻ", "ẽ", "ê",
                "ề", "ế", "ệ", "ể", "ễ", "ì", "í", "ị", "ỉ", "ĩ", "ò", "ó", "ọ",
                "ỏ", "õ", "ô", "ồ", "ố", "ộ", "ổ", "ỗ", "ơ", "ờ", "ớ", "ợ", "ở",
                "ỡ", "ù", "ú", "ụ", "ủ", "ũ", "ư", "ừ", "ứ", "ự", "ử", "ữ", "ỳ",
                "ý", "ỵ", "ỷ", "ỹ", "đ", "À", "Á", "Ạ", "Ả", "Ã", "Â", "Ầ", "Ấ",
                "Ậ", "Ẩ", "Ẫ", "Ă", "Ằ", "Ắ", "Ặ", "Ẳ", "Ẵ", "È", "É", "Ẹ", "Ẻ",
                "Ẽ", "Ê", "Ề", "Ế", "Ệ", "Ể", "Ễ", "Ì", "Í", "Ị", "Ỉ", "Ĩ", "Ò",
                "Ó", "Ọ", "Ỏ", "Õ", "Ô", "Ồ", "Ố", "Ộ", "Ổ", "Ỗ", "Ơ", "Ờ", "Ớ",
                "Ợ", "Ở", "Ỡ", "Ù", "Ú", "Ụ", "Ủ", "Ũ", "Ư", "Ừ", "Ứ", "Ự", "Ử",
                "Ữ", "Ỳ", "Ý", "Ỵ", "Ỷ", "Ỹ", "Đ", "ê", "ù", "à",

                "á", "ạ", "ả", "ã", "â", "ầ", "ấ", "ậ", "ẩ", "ẫ", "ă",
                "ằ", "ắ", "ặ", "ẳ", "ẵ", "è", "é", "ẹ", "ẻ", "ẽ", "ê",
                "ề", "ế", "ệ", "ể", "ễ", "ì", "í", "ị", "ỉ", "ĩ", "ò",
                "ó", "ọ", "ỏ", "õ", "ô", "ồ", "ố", "ộ", "ổ", "ỗ", "ơ",
                "ờ", "ớ", "ợ", "ở", "ỡ", "ù", "ú", "ụ", "ủ", "ũ", "ư",
                "ừ", "ứ", "ự", "ử", "ữ", "ỳ", "ý", "ỵ", "ỷ", "ỹ", "đ",
                "À", "Á", "Ạ", "Ả", "Ã", "Â", "Ầ", "Ấ", "Ậ", "Ẩ", "Ẫ",
                "Ă", "Ằ", "Ắ", "Ặ", "Ẳ", "Ẵ", "È", "É", "Ẹ", "Ẻ", "Ẽ",
                "Ê", "Ề", "Ế", "Ệ", "Ể", "Ễ", "Ì", "Í", "Ị", "Ỉ", "Ĩ",
                "Ò", "Ó", "Ọ", "Ỏ", "Õ", "Ô", "Ồ", "Ố", "Ộ", "Ổ", "Ỗ",
                "Ơ", "Ờ", "Ớ", "Ợ", "Ở", "Ỡ", "Ù", "Ú", "Ụ", "Ủ", "Ũ",
                "Ư", "Ừ", "Ứ", "Ự", "Ử", "Ữ", "Ỳ", "Ý", "Ỵ", "Ỷ", "Ỹ", "Đ",
            };

            List<string> unSignedCharacters = new List<string>
            {
                "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                "a", "a", "a", "a", "a", "a", "a", "e", "e", "e", "e", "e", "e",
                "e", "e", "e", "e", "e", "i", "i", "i", "i", "i", "o", "o", "o",
                "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o",
                "o", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "y",
                "y", "y", "y", "y", "d", "A", "A", "A", "A", "A", "A", "A", "A",
                "A", "A", "A", "A", "A", "A", "A", "A", "A", "E", "E", "E", "E",
                "E", "E", "E", "E", "E", "E", "E", "I", "I", "I", "I", "I", "O",
                "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O",
                "O", "O", "O", "U", "U", "U", "U", "U", "U", "U", "U", "U", "U",
                "U", "Y", "Y", "Y", "Y", "Y", "D", "e", "u", "a",

                "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                "a", "a", "a", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e",
                "e", "i", "i", "i", "i", "i", "o", "o", "o", "o", "o", "o", "o",
                "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "u", "u", "u",
                "u", "u", "u", "u", "u", "u", "u", "u", "y", "y", "y", "y", "y",
                "d", "A", "A", "A", "A", "A", "A", "A", "A", "A", "A", "A", "A",
                "A", "A", "A", "A", "A", "E", "E", "E", "E", "E", "E", "E", "E",
                "E", "E", "E", "I", "I", "I", "I", "I", "O", "O", "O", "O", "O",
                "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "O", "U",
                "U", "U", "U", "U", "U", "U", "U", "U", "U", "U", "Y", "Y", "Y", "Y", "Y", "D",
            };

            for (int i = 0; i < signedCharacters.Count; i++)
            {
                input = input.Replace(signedCharacters[i], unSignedCharacters[i]);
            }

            return input;
        }

        public static string RemoveSpecialCharacters(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var pattern = new Regex("[:\\-!@#,=$%^&*()}{|\":?><\\[\\]\\;/.~+]");
            input = pattern.Replace(input, string.Empty);
            input = Regex.Replace(input, @"\s+", string.Empty);
            input = Regex.Replace(input, @"'", string.Empty);
            input = Regex.Replace(input, @"`", string.Empty);
            return input;
        }

        public static string RemoveNonPrintableCharacters(this string text)
        {
            return string.IsNullOrEmpty(text) ? text : Regex.Replace(text, @"\p{C}+", string.Empty);
        }

        public static string RemoveWhiteSpaces(this string text)
        {
            return string.IsNullOrEmpty(text) ? text : Regex.Replace(text, @"\s+", string.Empty);
        }

        public static string MergeMultipleWhiteSpaces(this string text)
        {
            return string.IsNullOrEmpty(text) ? text : Regex.Replace(text, @"\s+", " ");
        }

        public static string CapitalizeFirstLetter(this string text)
        {
            return string.IsNullOrEmpty(text) ? text : $"{text.First().ToString().ToUpper()}{text.Substring(1).ToLower()}";
        }

        public static string FormatFileName(this string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            int place = originalFileName.LastIndexOf(extension);
            var fileName = originalFileName.Remove(place, extension.Length).Insert(place, string.Empty);

            fileName = fileName.RemoveSpecialCharacters().RemoveSignedCharacters().Trim();
            fileName = $"{fileName}_{DateTime.Now:ddMMyyyyhhmmsss}{extension}";

            return fileName;
        }

        public static byte[] Encrypting(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                var aes = Aes.Create();

                byte[] key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
                byte[] iv = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

                var cryptStream = new CryptoStream(stream, aes.CreateEncryptor(key, iv), CryptoStreamMode.Write);

                var sWriter = new StreamWriter(cryptStream);

                byte[] result;
                using (MemoryStream ms = new MemoryStream())
                {
                    sWriter.BaseStream.CopyTo(ms);
                    result = ms.ToArray();
                }

                sWriter.Close();
                cryptStream.Close();
                stream.Close();

                return result;
            }
        }

        public static byte[] Decryption(byte[] byteArray)
        {
            var stream = new MemoryStream(byteArray);

            var aes = Aes.Create();
            byte[] key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
            byte[] iv = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

            CryptoStream cryptStream = new CryptoStream(stream, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read);
            StreamReader sReader = new StreamReader(cryptStream);

            byte[] result;
            using (MemoryStream ms = new MemoryStream())
            {
                sReader.BaseStream.CopyTo(ms);
                result = ms.ToArray();
            }

            cryptStream.Close();
            stream.Close();
            sReader.Close();

            return result;
        }

        public static string ConvertPhoneNumberToPrefix84(this string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return string.Empty;
            }

            if (phoneNumber.StartsWith("0"))
            {
                return $"84{phoneNumber.Substring(1)}";
            }

            return phoneNumber.StartsWith("84") ? phoneNumber : string.Empty;
        }

        public static string FindAndReplaceAtIndex(this string text, string findText, string replaceText, int atCount)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            if (atCount < 0)
            {
                return text;
            }

            int index = text.IndexOfNth(findText, atCount);

            if (index < 0)
            {
                return text;
            }

            return text.Substring(0, index) + replaceText + text.Substring(index + 1);
        }

        public static int IndexOfNth(this string str, string value, int nth = 0)
        {
            if (nth < 0)
            {
                return -1;
            }

            int offset = str.IndexOf(value);
            for (int i = 0; i < nth; i++)
            {
                if (offset == -1)
                {
                    return -1;
                }

                offset = str.IndexOf(value, offset + 1);
            }

            return offset;
        }

        public static bool IsNumber(this string text)
        {
            var isNumber = new Regex("^[0-9]+$");
            return string.IsNullOrEmpty(text) ? false : isNumber.IsMatch(text);
        }

        public static bool IsAlphaNumeric(this string text)
        {
            Regex isAlphaNumeric = new Regex(RegexConstants.AlphaNumericFormat);
            return string.IsNullOrEmpty(text) ? false : isAlphaNumeric.IsMatch(text);
        }

        public static bool IsAlphaNumericOrSpecialCharacters(this string text)
        {
            Regex pattern = new Regex(RegexConstants.AlphaNumericOrSpecialCharactersFormat);
            return !string.IsNullOrEmpty(text) && pattern.IsMatch(text);
        }

        public static bool IsEmailFormat(this string text)
        {
            var isEmailFormat = new Regex(RegexConstants.EmailFormat);
            return string.IsNullOrEmpty(text) ? false : isEmailFormat.IsMatch(text);
        }

        public static bool HasWhiteSpace(this string text)
        {
            var hasWhiteSpace = new Regex(@"\s+");
            return string.IsNullOrEmpty(text) ? false : hasWhiteSpace.IsMatch(text);
        }

        public static byte[] GetAsByteArray(this IFormFile file)
        {
            using var fileStream = file.OpenReadStream();
            byte[] bytes = new byte[file.Length];
            fileStream.Read(bytes, 0, (int)file.Length);
            return bytes;
        }

        public static bool CompareText(this string a, string b)
        {
            return a.ToLower().Where(ac => !char.IsWhiteSpace(ac)).SequenceEqual(b.ToLower().Where(bc => !char.IsWhiteSpace(bc)));
        }

        public static int AsInt(this Enum value)
        {
            return (int)(object)value;
        }

        public static string GenerateSku(this string phrase)
        {
            string str = phrase.RemoveSignedCharacters().ToLower();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", string.Empty);
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-");
            return str;
        }

        public static string? ToCurrency(this decimal? value)
        {
            var info = CultureInfo.GetCultureInfo("vi-VN");
            return string.Format(info, "{0:C}", value ?? 0);
        }

        public static IFormFile Base64ToFile(this string base64, string fileName, int fileType)
        {
            if (fileType == MimeTypeEnum.XLSX.AsInt())
            {
                base64 = base64.Replace(MimeTypeBase64.XLSX, string.Empty);
            }

            byte[] bytes = Convert.FromBase64String(base64);
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                IFormFile formFile = new FormFile(memoryStream, 0, bytes.Length, "file", fileName);

                return formFile;
            }
        }

        public static string GetMenuKeyLang(this string key)
        {
            return $"Menu.{key}";
        }

        public static string GetAttributeKeyLang(this string key)
        {
            return $"Attribute.{key}";
        }
    }
}
