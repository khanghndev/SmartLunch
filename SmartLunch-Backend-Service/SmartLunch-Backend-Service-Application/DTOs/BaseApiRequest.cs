using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.DTOs
{
    public class BaseApiRequest
    {
        public Guid RequestId { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } = VietnamTime.Now;
        public string Type { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
    }
}