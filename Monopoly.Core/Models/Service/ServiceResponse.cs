using System.Net;

namespace Monopoly.Core.Models.Service
{
    public class ServiceResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } 
        public HttpStatusCode StatusCode { get; set; }
        public T? Data { get; set; }

        public ServiceResponse(bool success, string message, HttpStatusCode statusCode, T? data) 
        {
            Success = success;
            Message = message;
            StatusCode = statusCode;
            Data = data;
        }
        public ServiceResponse() { }
    }
}
