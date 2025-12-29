namespace FBS.Shared.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using FBS.Shared.Constants;
    using FBS.Shared.Utilities;
    using FBS.Shared.DataTranferObjects.FileUploads;

    public static class FileUploadHelper
    {
        public static FileUploadDto UploadFile(IFormFile file, string folderPath, bool createThumbnail)
        {
            var extension = Path.GetExtension(file.FileName);
            var name = file.FileName.Replace(extension, string.Empty);
            var renamedFile = file.FileName.FormatFileName();

            var uploadedFile = new FileUploadDto
            {
                Name = name,
                FileName = renamedFile,
                OriginalFileName = $"{name}{extension}",
                ContentType = file.ContentType,
                Length = file.Length,
                SubFolder = $"{folderPath}/{DateTime.Now.Year}_{DateTime.Now.Month}",
            };

            var fullPath = $"{PathConstants.Folder.Upload}/{uploadedFile.SubFolder}";
            if (!Directory.Exists(fullPath))
            {
                Utils.CreateFolder(fullPath);
            }

            var filePath = $"{fullPath}/{uploadedFile.FileName}";
            var thumbPath = $"{fullPath}/{PathConstants.Extension.Thumbnail}";

            if (!Directory.Exists(thumbPath))
            {
                Directory.CreateDirectory(thumbPath);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return uploadedFile;
        }

        public static FileUploadDto UploadFile(IFormFile file, string folderPath)
        {
            return UploadFile(file, folderPath, false);
        }

        public static void DeleteFile(FileUploadDto file)
        {
            if (file == null)
            {
                return;
            }

            var fileName = $"{PathConstants.Folder.Upload}/{file.SubFolder}/{file.FileName}";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        public static void DeleteFiles(List<FileUploadDto> files)
        {
            if (files == null || !files.Any())
            {
                return;
            }

            foreach (var file in files)
            {
                DeleteFile(file);
            }
        }

        public static byte[] DownloadFile(FileUploadDto uploadDto, string folderPath)
        {
            if (uploadDto == null)
            {
                return Array.Empty<byte>();
            }

            var filePath = $"{PathConstants.Folder.Root}/{folderPath}/{uploadDto.SubFolder}/{uploadDto.FileName}";
            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                stream.CopyTo(memory);
            }

            memory.Position = 0;
            return memory.ToArray();
        }

        public static byte[] DownloadFileByPath(string path)
        {
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                stream.CopyTo(memory);
            }

            memory.Position = 0;
            return memory.ToArray();
        }

        public static string? GetFileBase64(string? fileName, bool isUploadFile = true)
        {
            var defaultImage = $"{PathConstants.Folder.Root}/{PathConstants.Default.Image}";
            if (string.IsNullOrEmpty(fileName))
            {
                return Utils.ConvertToBase64(defaultImage);
            }

            try
            {
                var uploadedFile = JsonConvert.DeserializeObject<FileUploadDto>(fileName);
                defaultImage = $"{(isUploadFile ? PathConstants.Folder.Upload : PathConstants.Folder.Root)}/{uploadedFile?.SubFolder}/{uploadedFile?.FileName}";
                return Utils.ConvertToBase64(defaultImage);
            }
            catch (Exception)
            {
                return Utils.ConvertToBase64(defaultImage);
            }
        }
    }
}
