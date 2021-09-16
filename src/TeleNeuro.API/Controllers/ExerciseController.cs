
using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.Extension;
using PlayCore.Core.Model;
using Service.Document.DocumentServiceSelector;
using Service.Document.Model;
using System.Threading.Tasks;
using TeleNeuro.API.Models;
using TeleNeuro.Entities;
using TeleNeuro.Service.ExerciseService;

namespace TeleNeuro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ExerciseController
    {
        private readonly IExerciseService _exerciseService;
        private readonly IDocumentServiceSelector _documentServiceSelector;

        public ExerciseController(IExerciseService exerciseService, IDocumentServiceSelector documentServiceSelector)
        {
            _exerciseService = exerciseService;
            _documentServiceSelector = documentServiceSelector;
        }
        [HttpPost]
        public async Task<BaseResponse> ListExercises(PageInfo pageInfo)
        {
            return new BaseResponse()
                .SetResult(await _exerciseService.ListExercises(pageInfo))
                .SetTotalCount(await _exerciseService.CountExercises())
                .SetPage(pageInfo.Page)
                .SetPageSize(pageInfo.PageSize);
        }
        [HttpPost]
        public async Task<BaseResponse> UpdateExercise([FromForm] ExerciseModel model)
        {
            DocumentResult documentResult = null;
            if (model.File != null)
            {
                documentResult = await _documentServiceSelector.GetService(model.File, new[] { DocumentType.Image, DocumentType.Video }).SaveAsync(model.File.OpenReadStream(), model.File.FileName, model.File.ContentType);
            }

            return new BaseResponse().SetResult(await _exerciseService.UpdateExercise(new Exercise()
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                IsActive = model.IsActive,
                DocumentGuid = documentResult?.Guid
            }));
        }
        [HttpPost]
        public async Task<BaseResponse> ToggleExerciseStatus(ExerciseModel model)
        {
            return new BaseResponse().SetResult(await _exerciseService.ToggleExerciseStatus(model.Id));
        }

    }
}
