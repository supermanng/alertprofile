using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Common.Models
{
    public class BaseMessageResponse
    {
        [JsonIgnore]
        public bool IsSuccessResponse { get; set; }
        public int ResponseCode { get; set; }
        public string Message { get; set; }
    }
    public class MessageResponse<T> : BaseMessageResponse
    {
        public T Result { get; set; }
    }
    public enum ResponseDetail
    {
        Successful = 1,
        Failed
    }
}
