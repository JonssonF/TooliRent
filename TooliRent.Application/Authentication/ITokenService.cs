using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Authentication;

public interface ITokenService
{
    Task<(string accessToken, DateTime expiresUtc)> CreateAccessTokenAsync(AuthUser user);

    string CreateRefreshToken();

}
