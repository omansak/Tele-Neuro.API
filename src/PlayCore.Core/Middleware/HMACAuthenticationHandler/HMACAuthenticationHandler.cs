using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace PlayCore.Core.Middleware.HMACAuthenticationHandler
{
    /*
     * Client Usage
     * HTTP METHOD
     * Header Value :
     *  Authorization (AuthorizationHeader) : {Client GUID}:{HTTP Method}:{Full URL}:{Timestamp}
     *  Timestamp (TimeStampHeader) : {UTC Timestamp}
     *
     * Return ClientId Claim on User 
     */
    public class HMACAuthenticationHandler : AuthenticationHandler<HMACAuthenticationOptions>
    {
        public HMACAuthenticationHandler(IOptionsMonitor<HMACAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {

        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(Options.AuthorizationHeader))
            {
                return await Task.Run(() => AuthenticateResult.Fail("Missing authorization."));
            }

            if (!Request.Headers.ContainsKey(Options.TimeStampHeader))
            {
                return await Task.Run(() => AuthenticateResult.Fail("Missing timestamp."));
            }

            DateTime timestamp = ConvertUTCFromTimeStamp(Request.Headers[Options.TimeStampHeader]);

            if (!PassesThresholdCheck(timestamp))
            {
                return AuthenticateResult.Fail("Date is drifted more than allowed.");
            }

            var hash = SplitAuthenticationHeader(Request.Headers[Options.AuthorizationHeader]);
            if (hash == null)
            {
                return AuthenticateResult.Fail("Hash header is not valid.");
            }

            if (!ComputeHash(Options.Secret, hash.Value.Signature, hash.Value.ClientId, timestamp))
            {
                return await Task.Run(() => AuthenticateResult.Fail("Client authentication failed."));
            }


            Claim[] claims = { new(ClaimTypes.Name, hash.Value.ClientId) };
            ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);
            return await Task.Run(() => AuthenticateResult.Success(ticket));
        }

        private DateTime ConvertUTCFromTimeStamp(string timeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(double.Parse(timeStamp));
        }

        private double ConvertTimeStampFromUTC(DateTime utc)
        {
            return utc.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        private bool PassesThresholdCheck(DateTime timestamp)
        {
            return Options.AllowedDateDrift.Ticks <= 0 || timestamp.Add(Options.AllowedDateDrift) > DateTime.UtcNow;
        }

        private (string ClientId, string Signature)? SplitAuthenticationHeader(string header)
        {
            if (header == null)
                return null;

            string[] splitHeader = header.Split(Options.SplitChar);
            if (splitHeader.Length != 2)
                return null;

            return (splitHeader[0], splitHeader[1]);
        }

        private bool ComputeHash(string sharedSecret, string clientHash, string clientId, DateTime date)
        {
            string hashString;
            byte[] key = Encoding.UTF8.GetBytes(sharedSecret);
            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                hashString = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(Generate(clientId, Request.Method, Request.GetDisplayUrl(), ConvertTimeStampFromUTC(date)))));
            }
            return hashString.Equals(clientHash);
        }

        private string Generate(string id, string method, string url, double timeStamp)
        {
            return $"{id}{Options.SplitChar}{method}{Options.SplitChar}{url}{Options.SplitChar}{timeStamp}";
        }
    }
}
