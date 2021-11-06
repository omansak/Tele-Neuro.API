using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.Extension;
using PlayCore.Core.Model;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        public ContentController(IProgramService programService, IExerciseService exerciseService)
        {
            _programService = programService;
            _exerciseService = exerciseService;
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
    }
}
