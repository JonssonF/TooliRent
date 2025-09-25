using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Authentication.DTOs
{

    // Data Transfer Objects (DTOs) for Authentication operations
    public record RegisterRequest(string Email, string Password, string? FullName);
    public record LoginRequest(string Email, string Password);
    public record RefreshRequest(string RefreshToken);
    public record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresUtc);
    public record LogoutRequest(string RefreshToken);
    
}
