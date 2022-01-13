using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeleNeuro.API.Services;
using TeleNeuro.Service.ExerciseService;
using TeleNeuro.Service.ExerciseService.Models;
using TeleNeuro.Service.ProgramService;
using TeleNeuro.Service.ProgramService.Models;

namespace TeleNeuro.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ContentController
    {
        private readonly IProgramService _programService;
        private readonly IExerciseService _exerciseService;
        private readonly IUserManagerService _userManagerService;

        public ContentController(IProgramService programService, IExerciseService exerciseService, IUserManagerService userManagerService)
        {
            _programService = programService;
            _exerciseService = exerciseService;
            _userManagerService = userManagerService;
        }

        [HttpGet("{programId}")]
        public async Task<BaseResponse<ProgramInfo>> ProgramInfo(int programId)
        {
            return new BaseResponse<ProgramInfo>()
                .SetResult(await _programService.GetProgram(programId));
        }

        [HttpGet("{programId}")]
        public async Task<BaseResponse<IEnumerable<ProgramAssignedExerciseInfo>>> AssignedExercises(int programId)
        {
            return new BaseResponse<IEnumerable<ProgramAssignedExerciseInfo>>()
                .SetResult(await _programService.AssignedExercises(programId, true));
        }

        [HttpGet("{exerciseId}")]
        public async Task<BaseResponse<ExerciseInfo>> GetActiveExercise(int exerciseId)
        {
            return new BaseResponse<ExerciseInfo>()
                .SetResult(await _exerciseService.GetActiveExercise(exerciseId));
        }

        [HttpPost]
        public async Task<BaseResponse<List<AssignedProgramOfUserInfo>>> SelfAssignedPrograms(PageInfo pageInfo)
        {
            var (result, count) = await _programService.ListAssignedPrograms(new AssignedProgramOfUserModel
            {
                PageInfo = pageInfo,
                UserId = _userManagerService.UserId
            });
            return new BaseResponse<List<AssignedProgramOfUserInfo>>()
                .SetResult(result)
                .SetTotalCount(count)
                .SetPage(pageInfo.Page)
                .SetPageSize(pageInfo.PageSize);
        }
    }
}
