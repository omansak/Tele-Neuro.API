using Microsoft.AspNetCore.Authentication;
using System;

namespace PlayCore.Core.Middleware.HMACAuthenticationHandler
{
    public class HMACAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string Schema { get; set; } = "HMAC";
        public string AuthorizationHeader { get; set; } = "Authorization";
        public string TimeStampHeader { get; set; } = "Timestamp";
        public TimeSpan AllowedDateDrift { get; set; } = TimeSpan.FromSeconds(30);
        public string Secret { get; set; }
        public char SplitChar { get; set; } = ':';
    }
}
