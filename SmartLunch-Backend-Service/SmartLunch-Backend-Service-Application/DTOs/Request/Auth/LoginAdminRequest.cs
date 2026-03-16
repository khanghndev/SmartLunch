using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLunch.Backend.Service.Application.DTOs.Request.Auth
{
    public class LoginAdminRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}