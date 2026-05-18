using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.DTOs
{
    public class BaseApiResponse<T>
    {
        public bool Success { get; init; } = true;
        public DateTime Timestamp { get; init; } = VietnamTime.Now;
        public string Type { get; init; } = string.Empty;
        public string Source { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public T Data { get; init; } = default!;
        public string Error { get; init; } = string.Empty;
        public string ErrorDetails { get; init; } = string.Empty;

        // Additional properties for middleware compatibility
        public string RequestId { get; init; } = string.Empty;
        public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

        /// <summary>
        /// Creates an error  with the specified message, errors, and status code.
        /// </summary>
        public static BaseApiResponse<T> ErrorResult(string message, IEnumerable<string> errors) =>
            new BaseApiResponse<T>
            {
                Success = false,
                Status = "Error",
                Message = message,
                Error = message,
                ErrorDetails = errors != null ? string.Join("; ", errors) : string.Empty,
                Errors = errors != null ? errors.ToList().AsReadOnly() : Array.Empty<string>(),
                Timestamp = VietnamTime.Now,
            };

        /// <summary>
        /// Creates a not found  with the specified message.
        /// </summary>
        public static BaseApiResponse<T> NotFoundResult(string message) =>
            new BaseApiResponse<T>
            {
                Success = false,
                Status = "Error",
                Message = message,
                Error = message,
                ErrorDetails = message,
                Errors = new List<string> { message }.AsReadOnly(),
                Timestamp = VietnamTime.Now,
            };

        /// <summary>
        /// Creates a successful  with the specified data and message.
        /// </summary>
        public static BaseApiResponse<T> SuccessResult(T data, string message) =>
            new BaseApiResponse<T>
            {
                Success = true,
                Status = "Success",
                Message = message,
                Data = data,
                Timestamp = VietnamTime.Now,
            };

        /// <summary>
        /// Creates a successful  with the specified data and default message.
        /// </summary>
        public static BaseApiResponse<T> SuccessResult(T data) =>
            SuccessResult(data, "Operation completed successfully");
    }
}