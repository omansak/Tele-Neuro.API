namespace PlayCore.Core.Managers.JWTAuthenticationManager
{
    public class JwtTokenResult
    {
        public string AccessToken { get; set; }
        public RefreshToken RefreshToken { get; set; }
    }
}
