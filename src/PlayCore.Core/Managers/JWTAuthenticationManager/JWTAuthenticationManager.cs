using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PlayCore.Core.Managers.JWTAuthenticationManager
{
    /// <summary>
    /// JWT Auth. Manager (You can use singleton)
    /// Don't forget clear Refresh token cache with 'JwtManagerRefreshCache' by 'HostedService', If you are using UseRefreshToken = 'true'
    /// </summary>
    public class JwtAuthenticationManager : IJwtAuthenticationManager
    {
        private readonly ConcurrentDictionary<string, RefreshToken> _refreshTokens;
        private readonly JwtTokenConfig _jwtTokenConfig;
        private readonly byte[] _secret;

        public JwtAuthenticationManager(JwtTokenConfig jwtTokenConfig)
        {
            _jwtTokenConfig = jwtTokenConfig;
            _refreshTokens = new ConcurrentDictionary<string, RefreshToken>();
            _secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
        }

        public void RemoveExpiredRefreshTokens(DateTime dateTime)
        {
            if (_jwtTokenConfig.UseRefreshToken)
            {
                var expiredTokens = _refreshTokens.Where(x => x.Value.ExpireAt < dateTime).ToList();
                foreach (var expiredToken in expiredTokens)
                {
                    _refreshTokens.TryRemove(expiredToken.Key, out _);
                }
            }
        }

        public void RemoveRefreshToken(string guid)
        {
            if (_jwtTokenConfig.UseRefreshToken)
            {
                var refreshTokens = _refreshTokens.Where(x => x.Value.Guid == guid).ToList();
                foreach (var refreshToken in refreshTokens)
                {
                    _refreshTokens.TryRemove(refreshToken.Key, out _);
                }
            }
        }

        public JwtTokenResult Generate(string guid, IEnumerable<Claim> claims)
        {
            var jwtToken = new JwtSecurityToken(
                _jwtTokenConfig.Issuer,
                string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value) ? _jwtTokenConfig.Audience : string.Empty,
                claims,
                expires: DateTime.Now.AddMinutes(_jwtTokenConfig.AccessTokenExpirationMinute),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            if (_jwtTokenConfig.UseRefreshToken)
            {
                var refreshToken = new RefreshToken
                {
                    Guid = guid,
                    TokenString = GenerateRefreshTokenString(),
                    ExpireAt = DateTime.Now.AddMinutes(_jwtTokenConfig.RefreshTokenExpirationMinute)
                };
                _refreshTokens.AddOrUpdate(refreshToken.TokenString, refreshToken, (i, j) => refreshToken);

                return new JwtTokenResult
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
            }

            return new JwtTokenResult
            {
                AccessToken = accessToken
            };
        }

        public JwtTokenResult Refresh(string refreshToken, string accessToken, string guid)
        {
            var (principal, jwtToken) = Decode(accessToken);
            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
            {
                throw new SecurityTokenException("Invalid token");
            }
            if (!_refreshTokens.TryGetValue(refreshToken, out var existingRefreshToken))
            {
                throw new SecurityTokenException("Invalid token");
            }
            if (existingRefreshToken.Guid != guid || existingRefreshToken.ExpireAt < DateTime.Now)
            {
                throw new SecurityTokenException("Invalid token");
            }

            return Generate(guid, principal.Claims.ToArray());
        }

        public (ClaimsPrincipal, JwtSecurityToken) Decode(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token");
            }
            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token,
                    new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(_secret),
                        ValidIssuer = _jwtTokenConfig.Issuer,
                        ValidAudience = _jwtTokenConfig.Audience,
                        ValidateIssuer = !string.IsNullOrWhiteSpace(_jwtTokenConfig.Issuer),
                        ValidateAudience = !string.IsNullOrWhiteSpace(_jwtTokenConfig.Audience),
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    },
                    out var validatedToken);
            return (principal, validatedToken as JwtSecurityToken);
        }

        private static string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
