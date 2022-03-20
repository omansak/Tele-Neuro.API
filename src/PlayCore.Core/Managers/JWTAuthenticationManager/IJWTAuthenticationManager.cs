using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PlayCore.Core.Managers.JWTAuthenticationManager
{
    public interface IJwtAuthenticationManager
    {
        void RemoveExpiredRefreshTokens(DateTime now);
        void RemoveRefreshToken(string guid);
        JwtTokenResult Generate(string guid, IEnumerable<Claim> claims);
        JwtTokenResult Refresh(string refreshToken, string accessToken, string guid);
        (ClaimsPrincipal, JwtSecurityToken) Decode(string token);
    }
}