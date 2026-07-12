using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace IMBP.App.Domain.Models
{
    public class ServiceResponse(int statusCode = StatusCodes.Status200OK)
    {
        [JsonIgnore]
        public int StatusCode { get; set; } = statusCode;

        [JsonIgnore]
        public string? ErrorCode { get; set; }

        public void SetError(int statusCode, string errorCode)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        public IActionResult ToControllerResponse()
        {
            return new ObjectResult(string.IsNullOrWhiteSpace(ErrorCode) ? this : new ServiceError(ErrorCode))
            {
                StatusCode = StatusCode,
            };
        }
    }
    public static class ServiceResponseExtensions
    {
        public static async Task<IActionResult> ToControllerResponse<T>(this Task<T> response)
            where T : ServiceResponse
        {
            return (await response).ToControllerResponse();
        }
    }
}
