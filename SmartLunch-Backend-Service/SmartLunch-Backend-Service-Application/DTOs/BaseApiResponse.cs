using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLunch.Backend.Service.Application.DTOs
{
    public class BaseApiResponse
    {
        public Guid ResponseId { get; set; } = Guid.NewGuid();
        public DateTime ResponseTimestamp { get; set; } = DateTime.UtcNow;
        public string ResponseType { get; set; } = string.Empty;
        public string ResponseSource { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
        public string ResponseStatus { get; set; } = string.Empty;
        public string ResponseData { get; set; } = string.Empty;
        public string ResponseError { get; set; } = string.Empty;
        public string ResponseErrorDetails { get; set; } = string.Empty;
    }
}