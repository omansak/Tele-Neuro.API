using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.Extension;
using PlayCore.Core.Model;
using TeleNeuro.Entities;
using TeleNeuro.Service.UserService;
using TeleNeuro.Service.UtilityService;

namespace TeleNeuro.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UtilityController
    {
        private readonly IUtilityService _utilityService;
        private readonly IUserService _userService;

        public UtilityController(IUtilityService utilityService, IUserService userService)
        {
            _utilityService = utilityService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<BaseResponse<IEnumerable<ExercisePropertyDefinition>>> ListExercisePropertyDefinitions()
        {
            return new BaseResponse<IEnumerable<ExercisePropertyDefinition>>().SetResult(await _utilityService.ListExercisePropertyDefinitions());
        }

        [HttpGet]
        public BaseResponse<IEnumerable<Role>> ListRoleDefinitions()
        {
            return new BaseResponse<IEnumerable<Role>>().SetResult(_userService.RoleDefinition.ToList());
        }
    }
}
