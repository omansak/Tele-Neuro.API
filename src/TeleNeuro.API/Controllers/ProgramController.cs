﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.Extension;
using PlayCore.Core.Model;
using TeleNeuro.API.Models;
using TeleNeuro.Service.ProgramService;
using TeleNeuro.Service.ProgramService.Models;

namespace TeleNeuro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
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
        public async Task<BaseResponse> ToggleProgramStatus(ProgramModel model)
        {
            return new BaseResponse().SetResult(await _programService.ToggleProgramStatus(model.Id));
        }
        [HttpPost]
        public async Task<BaseResponse> AssignExercise(AssignExerciseModel model)
        {
            return new BaseResponse().SetResult(await _programService.AssignExercise(model));
        }
    }
}
