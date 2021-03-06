using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using PlayCore.Core.CustomException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace TeleNeuro.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MinimumRoleAuthorize : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _role;
        public MinimumRoleAuthorize(string role)
        {
            _role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                var roles = context.HttpContext.User.Claims.Where(i => i.Type == ClaimTypes.Role).Select(i => i.Value).ToList();
                if (CheckRoleAuthorization(roles))
                {
                    return;
                }
            }

            throw new UIException("Yetkisiz Erişim").SetResultCode(403);
        }

        public bool CheckRoleAuthorization(IEnumerable<string> userRoles)
        {
            if (userRoles.Any())
            {
                var userMaxRole = Startup.RoleDefinitions.Where(i => userRoles.Contains(i.Key)).OrderBy(i => i.Priority).FirstOrDefault();
                var requirementRole = Startup.RoleDefinitions.FirstOrDefault(i => i.Key == _role);
                if (requirementRole != null && userMaxRole != null && requirementRole.Priority >= userMaxRole.Priority)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
