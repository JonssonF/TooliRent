using Microsoft.AspNetCore.Identity;

namespace TooliRent.Infrastructure.Identity
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
