using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLunch.Backend.Service.Application.DTOs
{
    public class BaseApiResponse<T>
    {
        public bool Success { get; init; } = true;
        public DateTime ResponseTimestamp { get; init; } = DateTime.UtcNow;
        public string ResponseType { get; init; } = string.Empty;
        public string ResponseSource { get; init; } = string.Empty;
        public string ResponseMessage { get; init; } = string.Empty;
        public string ResponseStatus { get; init; } = string.Empty;
        public T ResponseData { get; init; } = default!;
        public string ResponseError { get; init; } = string.Empty;
        public string ResponseErrorDetails { get; init; } = string.Empty;

        // Additional properties for middleware compatibility
        public string RequestId { get; init; } = string.Empty;
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

        /// <summary>
        /// Creates an error response with the specified message, errors, and status code.
        /// </summary>
        public static BaseApiResponse<T> ErrorResult(string message, IEnumerable<string> errors) =>
            new BaseApiResponse<T>
            {
                Success = false,
                ResponseStatus = "Error",
                ResponseMessage = message,
                ResponseError = message,
                ResponseErrorDetails = errors != null ? string.Join("; ", errors) : string.Empty,
                Errors = errors != null ? errors.ToList().AsReadOnly() : Array.Empty<string>(),
                Timestamp = DateTime.UtcNow,
                ResponseTimestamp = DateTime.UtcNow
            };

        /// <summary>
        /// Creates a not found response with the specified message.
        /// </summary>
        public static BaseApiResponse<T> NotFoundResult(string message) =>
            new BaseApiResponse<T>
            {
                Success = false,
                ResponseStatus = "Error",
                ResponseMessage = message,
                ResponseError = message,
                ResponseErrorDetails = message,
                Errors = new List<string> { message }.AsReadOnly(),
                Timestamp = DateTime.UtcNow,
                ResponseTimestamp = DateTime.UtcNow
            };

        /// <summary>
        /// Creates a successful response with the specified data and message.
        /// </summary>
        public static BaseApiResponse<T> SuccessResult(T data, string message) =>
            new BaseApiResponse<T>
            {
                Success = true,
                ResponseStatus = "Success",
                ResponseMessage = message,
                ResponseData = data,
                Timestamp = DateTime.UtcNow,
                ResponseTimestamp = DateTime.UtcNow
            };

        /// <summary>
        /// Creates a successful response with the specified data and default message.
        /// </summary>
        public static BaseApiResponse<T> SuccessResult(T data) =>
            SuccessResult(data, "Operation completed successfully");
    }
}