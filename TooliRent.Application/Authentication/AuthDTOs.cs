using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Authentication
{
    public record RegisterRequest(string Email, string Password, string? FullName);
    public record LoginRequest(string Email, string Password);
    public record RefreshRequest(string RefreshToken);
    public record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresUtc);
    public record PromoteUserRequest
    {
        public string UserId { get; init; } = default!;
        public bool RemoveMemberRole { get; init; } = false;
    }

    public record LogoutRequest
    {
        public string? RefreshToken { get; init; }
        public bool AllDevices { get; init; } = false;
    }
}
