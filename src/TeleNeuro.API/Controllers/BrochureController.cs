using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.CustomException;
using PlayCore.Core.Model;
using Service.Document.DocumentServiceSelector;
using Service.Document.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TeleNeuro.API.Attributes;
using TeleNeuro.API.Models;
using TeleNeuro.API.Services;
using TeleNeuro.Entities;
using TeleNeuro.Service.BrochureService;
using TeleNeuro.Service.BrochureService.Models;


namespace TeleNeuro.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    [MinimumRoleAuthorize(UserRoleDefinition.Contributor)]
    public class BrochureController
    {
        private readonly IBrochureService _brochureService;
        private readonly IUserManagerService _userManagerService;
        private readonly IDocumentServiceSelector _documentServiceSelector;
        public BrochureController(IBrochureService brochureService, IUserManagerService userManagerService, IDocumentServiceSelector documentServiceSelector)
        {
            _brochureService = brochureService;
            _userManagerService = userManagerService;
            _documentServiceSelector = documentServiceSelector;
        }

        [HttpGet("{brochureId}")]
        public async Task<BaseResponse<BrochureInfo>> BrochureInfo(int brochureId)
        {
            return new BaseResponse<BrochureInfo>()
                .SetResult(await _brochureService.GetBrochure(brochureId));
        }

        [HttpPost]
        public async Task<BaseResponse<IEnumerable<BrochureInfo>>> ListBrochures(PageInfo pageInfo)
        {
            var (result, count) = await _brochureService.ListExercises(pageInfo);
            return new BaseResponse<IEnumerable<BrochureInfo>>()
                .SetResult(result)
                .SetTotalCount(count)
                .SetPage(pageInfo.Page)
                .SetPageSize(pageInfo.PageSize);
        }

        [HttpPost]
        [MinimumRoleAuthorize(UserRoleDefinition.Editor)]
        public async Task<BaseResponse<BrochureInfo>> UpdateBrochure([FromForm] BrochureModel model)
        {
            DocumentResult documentResult = null;
            if (model.Id == 0 && (model.File == null || model.File?.Length == 0))
            {
                throw new UIException("İçerik bulunamadı.");
            }
            else
            {
                if (model.File != null && model.File.Length != 0)
                {
                    MemoryStream memoryStream = new MemoryStream();
                    await model.File.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    documentResult = await _documentServiceSelector
                        .GetService(model.File, new[] { DocumentType.Pdf })
                        .SaveAsync(memoryStream, model.File.FileName, model.File.ContentType);
                    memoryStream.Close();
                }
            }

            var brochureInfo = await _brochureService.UpdateBrochure(new Brochure
            {
                Id = model.Id,
                Name = model.Name,
                CreatedUser = _userManagerService.UserId,
                IsActive = model.IsActive,
                DocumentGuid = documentResult?.Guid
            });


            return new BaseResponse<BrochureInfo>().SetResult(brochureInfo);
        }

        [HttpPost]
        [MinimumRoleAuthorize(UserRoleDefinition.Editor)]
        public async Task<BaseResponse<bool>> ToggleBrochureStatus(ExerciseModel model)
        {
            return new BaseResponse<bool>().SetResult(await _brochureService.ToggleExerciseStatus(model.Id));
        }

        [HttpPost]
        public async Task<BaseResponse<int>> AssignUser(AssignBrochureUserModel model)
        {
            model.AssignedUserId = _userManagerService.UserId;
            return new BaseResponse<int>().SetResult(await _brochureService.AssignUser(model));
        }

        [HttpPost]
        public async Task<BaseResponse<bool>> DeleteAssignedUser(AssignBrochureUserModel model)
        {
            model.AssignedUserId = _userManagerService.UserId;
            return new BaseResponse<bool>().SetResult(await _brochureService.DeleteAssignedUser(model));
        }

        [HttpPost]
        public async Task<BaseResponse<List<AssignedBrochureUserInfo>>> ListAssignedUsers(AssignedBrochureUserModel model)
        {
            var (result, count) = await _brochureService.ListAssignedUsers(model);
            return new BaseResponse<List<AssignedBrochureUserInfo>>()
                .SetResult(result)
                .SetTotalCount(count)
                .SetPage(model.PageInfo.Page)
                .SetPageSize(model.PageInfo.PageSize);
        }
    }
}
