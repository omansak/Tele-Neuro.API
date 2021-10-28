namespace PlayCore.Core.Managers.JWTAuthenticationManager
{
    public class JWTTokenConfig
    {
        public string Secret { get; set; }
        public string Issuer { get; set; } = null;
        public string Audience { get; set; } = null;
        public int AccessTokenExpirationMinute { get; set; } = 60;
        public int RefreshTokenExpirationMinute { get; set; } = 60 * 24;
    }
}
