
using System;
using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.Extension;
using PlayCore.Core.Model;
using Service.Document.Image.ImageSharp;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PlayCore.Core.CustomException;
using PlayCore.Core.HostedService;
using PlayCore.Core.Logger;
using Service.Document.DocumentServiceSelector;
using Service.Document.Model;
using TeleNeuro.API.Models;
using TeleNeuro.Entities;
using TeleNeuro.Service.CategoryService;

namespace TeleNeuro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CategoryController
    {
        private readonly ICategoryService _categoryService;
        private readonly IDocumentServiceSelector _documentServiceSelector;

        public CategoryController(ICategoryService categoryService, IDocumentServiceSelector documentServiceSelector)
        {
            _categoryService = categoryService;
            _documentServiceSelector = documentServiceSelector;
        }
        [HttpPost]
        public async Task<BaseResponse> ListCategories(PageInfo pageInfo)
        {
            return new BaseResponse()
                .SetResult(await _categoryService.ListCategories(pageInfo))
                .SetTotalCount(await _categoryService.CountCategories())
                .SetPage(pageInfo.Page)
                .SetPageSize(pageInfo.PageSize);
        }
        [HttpPost]
        public async Task<BaseResponse> UpdateCategory([FromForm] CategoryModel model)
        {
            DocumentResult documentResult = null;
            if (model.Image != null)
            {
                documentResult = await _documentServiceSelector.GetService(model.Image, new[] { DocumentType.Image }).SaveAsync(model.Image.OpenReadStream(), model.Image.FileName, model.Image.ContentType);
            }

            return new BaseResponse().SetResult(await _categoryService.UpdateCategory(new Category
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                IsActive = model.IsActive,
                DocumentGuid = documentResult?.Guid
            }));
        }
        [HttpPost]
        public async Task<BaseResponse> ToggleCategoryStatus(CategoryModel model)
        {
            return new BaseResponse().SetResult(await _categoryService.ToggleCategoryStatus(model.Id));
        }

    }
}
