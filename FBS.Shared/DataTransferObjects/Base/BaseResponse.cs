// <copyright file=  BaseResponse.cs company= Tan Nguyen>
// Copyright (c) Tan Nguyen. All rights reserved.
// </copyright>

namespace FBS.Shared.DataTranferObjects.Base
{
    using System.Runtime.Serialization;
    using FBS.Shared.Constants;

    [DataContract]
    public class BaseResponse<T>
    {
        [DataMember(Name = "Type")]
        public string Type { get; set; } = GlobalConstants.ResponseType.Success;

        [DataMember(Name = "Message")]
        public string Message { get; set; }

        [DataMember(Name = "Data")]
        public T? Data { get; set; }
    }
}
