using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.Extension;
using PlayCore.Core.Model;
using TeleNeuro.API.Attributes;
using TeleNeuro.API.Models;
using TeleNeuro.API.Services;
using TeleNeuro.Service.ProgramService;
using TeleNeuro.Service.ProgramService.Models;

namespace TeleNeuro.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    [MinimumRoleAuthorize(UserRoleDefinition.Contributor)]
    public class ProgramController
    {
        private readonly IProgramService _programService;

        public ProgramController(IProgramService programService)
        {
            _programService = programService;
        }
        [HttpPost]
        public async Task<BaseResponse> ListPrograms(PageInfo pageInfo)
        {
            return new BaseResponse()
                .SetResult(await _programService.ListPrograms(pageInfo))
                .SetTotalCount(await _programService.CountPrograms())
                .SetPage(pageInfo.Page)
                .SetPageSize(pageInfo.PageSize);
        }
        [HttpPost]
        public async Task<BaseResponse> UpdateProgram(ProgramModel model)
        {
            return new BaseResponse().SetResult(await _programService.UpdateProgram(new Entities.Program
            {
                Id = model.Id,
                CategoryId = model.CategoryId,
                Name = model.Name,
                Description = model.Description,
                IsActive = model.IsActive,
                IsPublic = model.IsPublic
            }));
        }
        [HttpPost]
        public async Task<BaseResponse> ToggleProgramStatus([FromBody] int id)
        {
            return new BaseResponse().SetResult(await _programService.ToggleProgramStatus(id));
        }
        [HttpPost]
        public async Task<BaseResponse> AssignExercise(AssignExerciseModel model)
        {
            return new BaseResponse().SetResult(await _programService.AssignExercise(model));
        }
        [HttpPost]
        public async Task<BaseResponse> AssignedExercises([FromBody] int programId)
        {
            return new BaseResponse().SetResult(await _programService.AssignedExercises(programId));
        }
        [HttpPost]
        public async Task<BaseResponse> ChangeSequenceAssignedExercise(ChangeSequenceModel model)
        {
            return new BaseResponse().SetResult(await _programService.ChangeSequenceAssignedExercise(model.Id, model.Direction));
        }
        [HttpPost]
        public async Task<BaseResponse> DeleteAssignedExercise([FromBody] int relationId)
        {
            return new BaseResponse().SetResult(await _programService.DeleteAssignedExercise(relationId));
        }
    }
}
