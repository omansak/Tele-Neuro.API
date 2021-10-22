using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.Extension;
using PlayCore.Core.Model;
using System.Threading.Tasks;
using TeleNeuro.Service.ExerciseService;
using TeleNeuro.Service.ProgramService;

namespace TeleNeuro.API.Controllers
{
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
        public async Task<BaseResponse> ProgramInfo(int programId)
        {
            return new BaseResponse()
                .SetResult(await _programService.GetProgram(programId));
        }
        [HttpGet("{programId}")]
        public async Task<BaseResponse> AssignedExercises(int programId)
        {
            return new BaseResponse()
                .SetResult(await _programService.AssignedExercises(programId, true));
        }
        [HttpGet("{exerciseId}")]
        public async Task<BaseResponse> GetActiveExercise(int exerciseId)
        {
            return new BaseResponse()
                .SetResult(await _exerciseService.GetActiveExercise(exerciseId));
        }
    }
}
