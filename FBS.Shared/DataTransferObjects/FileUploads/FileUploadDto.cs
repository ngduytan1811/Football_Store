// <copyright file=  FileUploadDto.cs company= Tan Nguyen>
// Copyright (c) Tan Nguyen. All rights reserved.
// </copyright>

namespace FBS.Shared.DataTranferObjects.FileUploads
{
    public class FileUploadDto
    {
        public string? FileName { get; set; }

        public string? Name { get; set; }

        public string? OriginalFileName { get; set; }

        public string? ContentType { get; set; }

        public string? SubFolder { get; set; }

        public long Length { get; set; }
    }
}
