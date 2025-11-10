// <copyright file=  MimeTypes.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.Shared.Constants
{
    public static class MimeTypes
    {
        public const string XLSX = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public const string XLS = "application/vnd.ms-excel";
        public const string PNG = "image/png";
        public const string JPEG = "image/jpeg";
        public const string PDF = "application/pdf";
        public const string DOCX = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        public const string DOC = "application/msword";
        public const string PPTX = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
        public const string PPT = "application/vnd.ms-powerpoint";
    }

    public static class MimeTypeBase64
    {
        public const string XLSX = "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,";
        public const string ImagePNG = "data:image/png;base64,{0}";
        public const string ImageJPG = "data:image/jpg;base64,{0}";
    }
}
