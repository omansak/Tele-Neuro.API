using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.CustomException;
using PlayCore.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TeleNeuro.API.Attributes;
using TeleNeuro.API.Models;
using TeleNeuro.API.Services;
using TeleNeuro.Service.UserService;
using TeleNeuro.Service.UserService.Models;

namespace TeleNeuro.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController
    {
        private readonly IUserService _userService;
        private readonly IUserManagerService _userManagerService;

        public UserController(IUserService userService, IUserManagerService userManagerService)
        {
            _userService = userService;
            _userManagerService = userManagerService;
        }

        [HttpPost]
        [MinimumRoleAuthorize(UserRoleDefinition.Contributor)]
        public async Task<BaseResponse<int>> AddUser(UserRegisterModel model)
        {
            if (Startup
                .RoleDefinitions
                .Where(i => _userManagerService.Roles.Contains(i.Key))
                .Any(i => Startup.RoleDefinitions.Where(j => model.RoleKey.Contains(j.Key)).Any(j => j.Priority > i.Priority)))
            {
                return new BaseResponse<int>().SetResult(await _userService.UpdateUser(model));
            }
            throw new UIException("Yetkisiz Erişim").SetResultCode(403);
        }
        [HttpGet]
        [MinimumRoleAuthorize(UserRoleDefinition.Contributor)]
        public async Task<BaseResponse<bool>> ToggleUserStatus(int id)
        {
            return new BaseResponse<bool>().SetResult(await _userService.ToggleUserStatus(id));
        }
        /// <remarks>BaseFilterModel FilterClause : <see cref="UserInfoQueryable"/></remarks>
        [HttpPost]
        public async Task<BaseResponse<IEnumerable<UserInfo>>> ListFilterUsers(BaseFilterModel baseFilter)
        {
            return new BaseResponse<IEnumerable<UserInfo>>()
                .SetResult(await _userService.ListFilterUsers(baseFilter))
                .SetTotalCount(await _userService.CountFilterUsers(baseFilter))
                .SetPage(baseFilter.PagingBy?.IsValid == true ? (baseFilter.PagingBy.Skip / baseFilter.PagingBy.Take) + 1 : 0)
                .SetPageSize(baseFilter.PagingBy?.IsValid == true ? baseFilter.PagingBy.Take : 0);
        }
    }
}
