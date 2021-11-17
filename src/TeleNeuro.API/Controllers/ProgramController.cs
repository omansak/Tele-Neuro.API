using System.Collections.Generic;
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
        private readonly IUserManagerService _userManagerService;
        public ProgramController(IProgramService programService, IUserManagerService userManagerService)
        {
            _programService = programService;
            _userManagerService = userManagerService;
        }
        [HttpGet("{programId}")]
        public async Task<BaseResponse<ProgramInfo>> ProgramInfo(int programId)
        {
            return new BaseResponse<ProgramInfo>()
                .SetResult(await _programService.GetProgram(programId));
        }
        [HttpPost]
        public async Task<BaseResponse<IEnumerable<ProgramInfo>>> ListPrograms(PageInfo pageInfo)
        {
            return new BaseResponse<IEnumerable<ProgramInfo>>()
                .SetResult(await _programService.ListPrograms(pageInfo))
                .SetTotalCount(await _programService.CountPrograms())
                .SetPage(pageInfo.Page)
                .SetPageSize(pageInfo.PageSize);
        }
        [HttpPost]
        public async Task<BaseResponse<ProgramInfo>> UpdateProgram(ProgramModel model)
        {
            return new BaseResponse<ProgramInfo>().SetResult(await _programService.UpdateProgram(new Entities.Program
            {
                Id = model.Id,
                CategoryId = model.CategoryId,
                Name = model.Name,
                Description = model.Description,
                IsActive = model.IsActive,
                IsPublic = model.IsPublic,
                CreatedUser = _userManagerService.UserId
            }));
        }
        [HttpPost]
        public async Task<BaseResponse<bool>> ToggleProgramStatus([FromBody] int id)
        {
            return new BaseResponse<bool>().SetResult(await _programService.ToggleProgramStatus(id));
        }
        [HttpPost]
        public async Task<BaseResponse<int>> AssignExercise(AssignExerciseModel model)
        {
            model.UserId = _userManagerService.UserId;
            return new BaseResponse<int>().SetResult(await _programService.AssignExercise(model));
        }
        [HttpPost]
        public async Task<BaseResponse<int>> AssignUser(AssignUserModel model)
        {
            model.AssignedUserId = _userManagerService.UserId;
            return new BaseResponse<int>().SetResult(await _programService.AssignUser(model));
        }
        [HttpPost]
        public async Task<BaseResponse<bool>> DeleteAssignedUser(AssignUserModel model)
        {
            model.AssignedUserId = _userManagerService.UserId;
            return new BaseResponse<bool>().SetResult(await _programService.DeleteAssignedUser(model));
        }
        [HttpPost]
        public async Task<BaseResponse<IEnumerable<ProgramAssignedExerciseInfo>>> AssignedExercises([FromBody] int programId)
        {
            return new BaseResponse<IEnumerable<ProgramAssignedExerciseInfo>>().SetResult(await _programService.AssignedExercises(programId));
        }
        [HttpPost]
        public async Task<BaseResponse<bool>> ChangeSequenceAssignedExercise(ChangeSequenceModel model)
        {
            return new BaseResponse<bool>().SetResult(await _programService.ChangeSequenceAssignedExercise(model.Id, model.Direction));
        }
        [HttpPost]
        public async Task<BaseResponse<bool>> DeleteAssignedExercise([FromBody] int relationId)
        {
            return new BaseResponse<bool>().SetResult(await _programService.DeleteAssignedExercise(relationId));
        }
        [HttpPost]
        public async Task<BaseResponse<List<AssignedProgramUserInfo>>> ListAssignedUsers(AssignedProgramUsersModel model)
        {
            var (result, count) = await _programService.ListAssignedUsers(model);
            return new BaseResponse<List<AssignedProgramUserInfo>>()
                .SetResult(result)
                .SetTotalCount(count)
                .SetPage(model.PageInfo.Page)
                .SetPageSize(model.PageInfo.PageSize);
        }
    }
}
