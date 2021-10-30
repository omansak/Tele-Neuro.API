using Microsoft.AspNetCore.Http;
using PlayCore.Core.CustomException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

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
                throw new UIException("Kullanıcı Id bulunamadı");
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
                throw new UIException("Kullanıcı Role bulunamadı");
            }
        }

    }
}
