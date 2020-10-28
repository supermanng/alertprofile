using Common.Models;
using System;

namespace Common
{
    public class Util
    {
        public static MessageResponse<T> GetResponse<T>(bool response, string message, T result = default) where T : class
        {
            var responseMessage = new MessageResponse<T>();
            responseMessage.IsSuccessResponse = response;
            var responseCode = response ? ResponseDetail.Successful : ResponseDetail.Failed;
            responseMessage.ResponseCode = (int)responseCode;
            responseMessage.Message = message;
            responseMessage.Result = result;
            return responseMessage;
        }
        public static BaseMessageResponse GetSimpleResponse(bool response, string message)
        {
            var responseMessage = new BaseMessageResponse();
            responseMessage.IsSuccessResponse = response;
            var responseCode = response ? ResponseDetail.Successful : ResponseDetail.Failed;
            responseMessage.ResponseCode = (int)responseCode;
            responseMessage.Message = message;
            return responseMessage;
        }
    }
}
