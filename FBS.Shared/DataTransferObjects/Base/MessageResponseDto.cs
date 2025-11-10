// <copyright file= MessageResponseDto.cs company=Tan Nguyen>
// Copyright (c) Tan Nguyen. All rights reserved.
// </copyright>

namespace FBS.Shared.DataTransferObjects.Base
{
    public class MessageResponseDto
    {
        public MessageResponseDto(string? message = null, string? attribute = null, bool isValid = false)
        {
            Message = message;
            Attribute = attribute;
            IsValid = isValid;
        }

        public bool IsValid { get; set; }

        public string? Message { get; set; }

        public string? Attribute { get; set; }
    }
}
