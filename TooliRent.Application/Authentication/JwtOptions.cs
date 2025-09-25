using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Authentication
{
    // Configuration options for JWT (JSON Web Token) authentication.
    public sealed class JwtOptions
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenMinutes { get; set; } = 60;
    }
}
