using System.Net;

namespace ConsultaVehiculosForms.DTOs
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public int StatusCodeNumber => (int)StatusCode;
        public T? Data { get; set; }
        public string RawJson { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public ApiProblemDetailsDto? ProblemDetails { get; set; }
        public string? LocationHeader { get; set; }

        public static ApiResponse<T> Success(HttpStatusCode status, T data, string rawJson, string? location = null)
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,
                StatusCode = status,
                Data = data,
                RawJson = rawJson,
                LocationHeader = location
            };
        }

        public static ApiResponse<T> Fail(HttpStatusCode status, string error, string rawJson, ApiProblemDetailsDto? details = null)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                StatusCode = status,
                ErrorMessage = error,
                RawJson = rawJson,
                ProblemDetails = details
            };
        }
    }
}
