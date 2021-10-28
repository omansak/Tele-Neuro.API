namespace PlayCore.Core.Managers.JWTAuthenticationManager
{
    public class JWTTokenResult
    {
        public string AccessToken { get; set; }

        public RefreshToken RefreshToken { get; set; }
    }
}
