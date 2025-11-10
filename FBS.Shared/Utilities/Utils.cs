// <copyright file=  Utils.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.Shared.Utilities
{
    using System;
    using System.IO;
    using System.Security.AccessControl;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Localization;
    using FBS.Shared.Constants;

    public static class Utils
    {
        /// <summary>
        /// IsOnTime.
        /// </summary>
        /// <param name="startTimeAsString">Start time with format: HH:mm.</param>
        /// <param name="endTimeAsString">End time with format: HH:mm.</param>
        /// <param name="checkInTime">Time checkin before start time (minutes).</param>
        /// <returns>True/False.</returns>
        public static bool IsOnTime(string startTimeAsString, string endTimeAsString, int checkInTime)
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            TimeSpan startTime = TimeSpan.Parse(startTimeAsString);
            TimeSpan endTime = TimeSpan.Parse(endTimeAsString);

            startTime = startTime.Add(TimeSpan.FromMinutes(-checkInTime));

            if (TimeSpan.Compare(now, startTime) > 0 && TimeSpan.Compare(now, endTime) < 0)
            {
                return true;
            }

            return false;
        }

        public static string Encrypting(string filePath, out string key1, out string key2)
        {
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            var number = 16;
            var randomKey1 = Utils.RandomString(number);
            var randomKey2 = Utils.RandomString(number);
            char[] cArray1 = randomKey1.ToCharArray();
            char[] cArray2 = randomKey2.ToCharArray();

            var newKey1 = new StringBuilder();
            var newKey2 = new StringBuilder();
            for (var i = 0; i < number; i++)
            {
                newKey1.Append(cArray1[i]);
                newKey1.Append(cArray2[number - (i + 1)]);

                newKey2.Append(cArray2[i]);
                newKey2.Append(cArray1[number - (i + 1)]);
            }

            key1 = randomKey1;
            key2 = randomKey2;
            var dataFile = Convert.ToBase64String(memoryStream.ToArray());

            var random = new Random();
            var index = random.Next(1, dataFile.Length / 2);
            var data1 = dataFile.Substring(0, index);
            var data2 = dataFile.Substring(index, dataFile.Length - index);

            return $"{newKey1}{data1}{newKey2}{data2}";

            /*using (Aes aes = new AesCryptoServiceProvider())
            {
                var byteKey = Encoding.ASCII.GetBytes(key);
                var byteIv = Encoding.ASCII.GetBytes(iv);

                aes.Key = byteKey;
                aes.IV = byteIv;

                var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                memoryStream.Position = 0;

                byte[] encryptedData;
                using (var cryptStream = new CryptoStream(memoryStream, aes.CreateEncryptor(byteKey, byteIv), CryptoStreamMode.Write))
                {
                    var buffer = new byte[1024];
                    var read = memoryStream.Read(buffer, 0, buffer.Length);
                    while (read > 0)
                    {
                        cryptStream.Write(buffer, 0, read);
                        read = memoryStream.Read(buffer, 0, buffer.Length);
                    }

                    cryptStream.FlushFinalBlock();
                    encryptedData = memoryStream.ToArray();
                }

                return Convert.ToBase64String(encryptedData);
            }*/
        }

        public static string RandomString(int size)
        {
            var builder = new StringBuilder(size);

            Random random = new Random();
            char offset = 'a';
            const int lettersOffset = 26;

            for (var i = 0; i < size; i++)
            {
                var temp = random.Next(0, size);
                var @char = (char)random.Next(offset, offset + lettersOffset);
                if (temp % 3 == 0 && i % 3 == 0)
                {
                    builder.Append(@char.ToString().ToUpper());
                }
                else
                {
                    builder.Append(@char);
                }
            }

            return builder.ToString();
        }

        public static string ConvertToUnSign(string input)
        {
            input = input.Trim();
            for (var i = 0x20; i < 0x30; i++)
            {
                input = input.Replace(((char)i).ToString(), " ");
            }

            var regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            var str = input.Normalize(NormalizationForm.FormD);
            var str2 = regex.Replace(str, string.Empty).Replace('đ', 'd').Replace('Đ', 'D');
            while (str2.IndexOf("?", StringComparison.Ordinal) >= 0)
            {
                str2 = str2.Remove(str2.IndexOf("?", StringComparison.Ordinal), 1);
            }

            return str2;
        }

        public static byte[]? ToBypeArray(this Stream source)
        {
            if (source == null)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                source.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static void ConvertBase64ToFile(string base64String, string outputFilePath)
        {
            var base64Data = base64String.Contains(",") ? base64String.Split(',')[1] : base64String;

            byte[] fileBytes = Convert.FromBase64String(base64Data);

            File.WriteAllBytes(outputFilePath, fileBytes);
        }

        public static string ConvertToBase64(this Stream stream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            return Convert.ToBase64String(bytes);
        }

        public static string? ConvertToBase64(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            if (!File.Exists(filePath))
            {
                filePath = $"{PathConstants.Folder.Root}/{PathConstants.Default.Image}";
            }

            byte[] bytes = File.ReadAllBytes(filePath);
            return string.Format(MimeTypeBase64.ImagePNG, Convert.ToBase64String(bytes));
        }

        public static void CreateFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                DirectoryInfo dir = Directory.CreateDirectory(folderPath);

                DirectorySecurity security = dir.GetAccessControl();
                security.AddAccessRule(new FileSystemAccessRule(
                    "Everyone",
                    FileSystemRights.FullControl,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow));

                dir.SetAccessControl(security);
            }
        }

        public static string ConvertServerFileNameToFileName(string svFilename)
        {
            if (string.IsNullOrEmpty(svFilename))
            {
                return string.Empty;
            }

            if (svFilename.Length < 37)
            {
                return svFilename;
            }

            var fileExtension = svFilename.Substring(svFilename.LastIndexOf("."));
            var filename = svFilename.Substring(0, svFilename.LastIndexOf(".") - 37);
            return filename + fileExtension;
        }

        public static async Task<byte[]> GetBytes(this IFormFile formFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static byte[]? ToByteArrayIFormFile(this IFormFile source)
        {
            if (source == null)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                source.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static string GenerateCode(string? prefix = null)
        {
            return prefix + DateTime.Now.ToString("yyMMddHHmmss");
        }

        public static string GetMessageLocalizer(IStringLocalizer localizer, string key)
        {
            return localizer.GetString(key);
        }
    }
}
