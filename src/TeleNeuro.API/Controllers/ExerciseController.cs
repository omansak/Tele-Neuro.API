
using System.IO;
using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.Extension;
using PlayCore.Core.Model;
using Service.Document.DocumentServiceSelector;
using Service.Document.Model;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using PlayCore.Core.CustomException;
using PlayCore.Core.QueuedHostedService;
using TeleNeuro.API.Attributes;
using TeleNeuro.API.Models;
using TeleNeuro.API.Services;
using TeleNeuro.Entities;
using TeleNeuro.Service.ExerciseService;

namespace TeleNeuro.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    [MinimumRoleAuthorize(UserRoleDefinition.Contributor)]
    public class ExerciseController
    {
        private readonly IExerciseService _exerciseService;
        private readonly IBackgroundTaskQueue _queue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IUserManagerService _userManagerService;
        public ExerciseController(IExerciseService exerciseService, IBackgroundTaskQueue queue, IServiceScopeFactory serviceScopeFactory, IUserManagerService userManagerService)
        {
            _exerciseService = exerciseService;
            _queue = queue;
            _serviceScopeFactory = serviceScopeFactory;
            _userManagerService = userManagerService;
        }
        [HttpPost]
        public async Task<BaseResponse> SearchExercises(SearchTermModel model)
        {
            return new BaseResponse()
                .SetResult(await _exerciseService.SearchExercises(model.SearchTerm));
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
        [MinimumRoleAuthorize(UserRoleDefinition.Editor)]
        public async Task<BaseResponse> UpdateExercise([FromForm] ExerciseModel model)
        {
            if (model.Id == 0 && model.File?.Length == 0)
            {
                throw new UIException("Ýçerik bulunamadý.");
            }

            var exerciseInfo = await _exerciseService.UpdateExercise(new Exercise()
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                CreatedUser = _userManagerService.UserId,
                IsActive = model.IsActive
            });

            if (model.File?.Length > 0 && exerciseInfo != null)
            {
                MemoryStream memoryStream = new MemoryStream();
                await model.File.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                _queue.Queue(async i =>
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDocumentServiceSelector documentServiceSelector = scope.ServiceProvider.GetRequiredService<IDocumentServiceSelector>();
                        IExerciseService exerciseService = scope.ServiceProvider.GetRequiredService<IExerciseService>();
                        DocumentResult documentResult = null;
                        if (model.File != null)
                        {
                            documentResult = await documentServiceSelector
                                .GetService(model.File, new[] { DocumentType.Image, DocumentType.Video })
                                .SaveAsync(memoryStream, model.File.FileName, model.File.ContentType);
                            memoryStream.Close();
                        }

                        if (documentResult?.Guid != null)
                        {
                            exerciseInfo.Exercise.DocumentGuid = documentResult.Guid;
                            exerciseInfo.Exercise.CreatedUser = _userManagerService.UserId;
                            await exerciseService.UpdateExercise(exerciseInfo.Exercise);
                        }
                    }
                });
            }
            return new BaseResponse().SetResult(exerciseInfo);
        }
        [HttpPost]
        [MinimumRoleAuthorize(UserRoleDefinition.Editor)]
        public async Task<BaseResponse> ToggleExerciseStatus(ExerciseModel model)
        {
            return new BaseResponse().SetResult(await _exerciseService.ToggleExerciseStatus(model.Id));
        }

    }
}
