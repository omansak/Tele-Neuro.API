using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using PlayCore.Core.CustomException;
using System;
using System.Linq;
using System.Security.Claims;

namespace TeleNeuro.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MinimumRoleAuthorize : AuthorizeAttribute, IAuthorizationFilter
    {
        private string _role;
        public MinimumRoleAuthorize(string role)
        {
            _role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                var roles = context.HttpContext.User.Claims.Where(i => i.Type == ClaimTypes.Role).Select(i => i.Value).ToList();
                if (roles.Any())
                {
                    var userMaxRole = Startup.RoleDefinitions.Where(i => roles.Contains(i.Key)).OrderBy(i => i.Priority).FirstOrDefault();
                    var requirementRole = Startup.RoleDefinitions.FirstOrDefault(i => i.Key == _role);
                    if (requirementRole != null && userMaxRole != null && requirementRole.Priority >= userMaxRole.Priority)
                    {
                        return;
                    }
                }
            }

            throw new UIException("Yetkisiz Erişim");
        }
    }
}
