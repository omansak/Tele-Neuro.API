using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeleNeuro.API.Attributes;
using TeleNeuro.API.Services;
using TeleNeuro.Service.UserService;
using TeleNeuro.Service.UtilityService;
using TeleNeuro.Service.UtilityService.Models;

namespace TeleNeuro.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    [MinimumRoleAuthorize(UserRoleDefinition.Subscriber)]
    public class StatController
    {
        private readonly IUtilityService _utilityService;
        private readonly IUserService _userService;
        private readonly IUserManagerService _userManagerService;

        public StatController(IUtilityService utilityService, IUserService userService, IUserManagerService userManagerService)
        {
            _utilityService = utilityService;
            _userService = userService;
            _userManagerService = userManagerService;
        }

        [HttpPost]
        public async Task<BaseResponse<bool>> InsertRelationStatLog(RelationStatLogModel model)
        {
            model.UserId = _userManagerService.UserId;
            await _utilityService.InsertRelationStatLog(model);
            return new BaseResponse<bool>().SetResult(true);
        }

        [HttpPost]
        public async Task<BaseResponse<List<int?>>> CompletedExercisesOfProgram(int[] programIds)
        {
            return new BaseResponse<List<int?>>().SetResult(await _utilityService.CompletedExercisesOfProgram(programIds, _userManagerService.UserId));
        }

        [HttpGet]
        public async Task<BaseResponse<UserWorkProcessStats>> UserStats()
        {
            return new BaseResponse<UserWorkProcessStats>().SetResult(await _utilityService.UserStats(_userManagerService.UserId));
        }
    }
}
