using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.Model;
using Service.Document.DocumentServiceSelector;
using Service.Document.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeleNeuro.API.Attributes;
using TeleNeuro.API.Models;
using TeleNeuro.API.Services;
using TeleNeuro.Entities;
using TeleNeuro.Service.CategoryService;
using TeleNeuro.Service.CategoryService.Models;

namespace TeleNeuro.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CategoryController
    {
        private readonly ICategoryService _categoryService;
        private readonly IDocumentServiceSelector _documentServiceSelector;
        private readonly IUserManagerService _userManagerService;
        public CategoryController(ICategoryService categoryService, IDocumentServiceSelector documentServiceSelector, IUserManagerService userManagerService)
        {
            _categoryService = categoryService;
            _documentServiceSelector = documentServiceSelector;
            _userManagerService = userManagerService;
        }
        [HttpPost]
        [MinimumRoleAuthorize(UserRoleDefinition.Editor)]
        public async Task<BaseResponse<IEnumerable<CategoryInfo>>> ListCategories(PageInfo pageInfo)
        {
            return new BaseResponse<IEnumerable<CategoryInfo>>()
                .SetResult(await _categoryService.ListCategories(pageInfo))
                .SetTotalCount(await _categoryService.CountCategories())
                .SetPage(pageInfo.Page)
                .SetPageSize(pageInfo.PageSize);
        }
        [HttpPost]
        [MinimumRoleAuthorize(UserRoleDefinition.Subscriber)]
        public async Task<BaseResponse<IEnumerable<CategoryInfo>>> ListActiveCategories(PageInfo pageInfo)
        {
            return new BaseResponse<IEnumerable<CategoryInfo>>()
                .SetResult(await _categoryService.ListActiveCategories(pageInfo))
                .SetTotalCount(await _categoryService.CountActiveCategories())
                .SetPage(pageInfo.Page)
                .SetPageSize(pageInfo.PageSize);
        }
        [HttpPost]
        [MinimumRoleAuthorize(UserRoleDefinition.Editor)]
        public async Task<BaseResponse<CategoryInfo>> UpdateCategory([FromForm] CategoryModel model)
        {
            DocumentResult documentResult = null;
            if (model.Image != null)
            {
                documentResult = await _documentServiceSelector.GetService(model.Image, new[] { DocumentType.Image }).SaveAsync(model.Image.OpenReadStream(), model.Image.FileName, model.Image.ContentType);
            }

            return new BaseResponse<CategoryInfo>().SetResult(await _categoryService.UpdateCategory(new Category
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                IsActive = model.IsActive,
                DocumentGuid = documentResult?.Guid,
                CreatedUser = _userManagerService.UserId
            }));
        }
        [HttpPost]
        [MinimumRoleAuthorize(UserRoleDefinition.Editor)]
        public async Task<BaseResponse<bool>> ToggleCategoryStatus(CategoryModel model)
        {
            return new BaseResponse<bool>().SetResult(await _categoryService.ToggleCategoryStatus(model.Id));
        }

    }
}
