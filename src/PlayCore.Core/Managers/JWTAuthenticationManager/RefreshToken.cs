using System;

namespace PlayCore.Core.Managers.JWTAuthenticationManager
{
    public class RefreshToken
    {
        public string Guid { get; set; } 
        public string TokenString { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
