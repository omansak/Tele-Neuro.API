using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PlayCore.Core.Managers.JWTAuthenticationManager
{
    public interface IJWTAuthenticationManager
    {
        void RemoveExpiredRefreshTokens(DateTime now);
        void RemoveRefreshToken(string guid);
        JWTTokenResult Generate(string guid, IEnumerable<Claim> claims);
        JWTTokenResult Refresh(string refreshToken, string accessToken, string guid);
        (ClaimsPrincipal, JwtSecurityToken) Decode(string token);
    }
}