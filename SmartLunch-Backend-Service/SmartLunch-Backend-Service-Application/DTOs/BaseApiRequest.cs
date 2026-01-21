using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLunch.Backend.Service.Application.DTOs
{
    public class BaseApiRequest
    {
        public Guid RequestId { get; set; } = Guid.NewGuid();
        public DateTime RequestTimestamp { get; set; } = DateTime.UtcNow;
        public string RequestType { get; set; } = string.Empty;
        public string RequestSource { get; set; } = string.Empty;
    }
}