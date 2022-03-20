namespace PlayCore.Core.Managers.JWTAuthenticationManager
{
    public class JwtTokenConfig
    {
        public string Secret { get; set; }
        public string Issuer { get; set; } = null;
        public string Audience { get; set; } = null;
        public int AccessTokenExpirationMinute { get; set; } = 60;
        public bool UseRefreshToken { get; set; } = true;
        public int RefreshTokenExpirationMinute { get; set; } = 60 * 24;
    }
}
