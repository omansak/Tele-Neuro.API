using Microsoft.AspNetCore.Http;
using PlayCore.Core.CustomException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TeleNeuro.API.Services
{
    public class UserRoleDefinition
    {
        public const string Administrator = "ADMIN";
        public const string Editor = "EDITOR";
        public const string Contributor = "CONTRIBUTOR";
        public const string Subscriber = "SUBSCRIBER";
    }
    public class UserManagerService : IUserManagerService
    {
        private readonly ClaimsPrincipal _user;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserManagerService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _user = httpContextAccessor.HttpContext!.User;
        }

        public bool IsAuthenticated => _user.Identity?.IsAuthenticated == true;

        public int UserId
        {
            get
            {
                if (IsAuthenticated)
                {
                    var claim = _user.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier);
                    if (!string.IsNullOrWhiteSpace(claim?.Value))
                    {
                        var val = int.Parse(claim.Value);
                        if (val > 0)
                        {
                            return val;
                        }
                    }
                }
                throw new UIException("Kullanıcı Id bulunamadı").SetResultCode(401);
            }
        }

        public string Token
        {
            get
            {
                if (IsAuthenticated)
                {
                    var token = _httpContextAccessor.HttpContext!.GetTokenAsync(JwtBearerDefaults.AuthenticationScheme, "access_token").GetAwaiter().GetResult();
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        return token;
                    }
                }
                throw new UIException("Kullanıcı Token bulunamadı").SetResultCode(401);
            }
        }

        public IEnumerable<string> Roles
        {
            get
            {
                if (IsAuthenticated)
                {
                    return _user.Claims.Where(i => i.Type == ClaimTypes.Role).Select(i => i.Value);
                }
                throw new UIException("Kullanıcı Role bulunamadı").SetResultCode(401);
            }
        }

        public bool CheckMinimumRole(string role)
        {
            if (Roles.Any())
            {
                var userMaxRole = Startup.RoleDefinitions.Where(i => Roles.Contains(i.Key)).OrderBy(i => i.Priority).FirstOrDefault();
                var requirementRole = Startup.RoleDefinitions.FirstOrDefault(i => i.Key == role);
                if (requirementRole != null && userMaxRole != null && requirementRole.Priority >= userMaxRole.Priority)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
