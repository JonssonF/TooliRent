using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Authentication;

public interface ITokenService
{
    // Creates an access token for the specified authenticated user.
    Task<(string accessToken, DateTime expiresUtc)> CreateAccessTokenAsync(AuthUser user);
    // Generates a new refresh token.
    string CreateRefreshToken();

}
