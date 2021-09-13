
using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.Extension;
using PlayCore.Core.Model;
using Service.Document.Image.ImageSharp;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PlayCore.Core.CustomException;
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
        private readonly IDocumentImageService _documentImageService;

        public CategoryController(ICategoryService categoryService, IDocumentImageService documentImageService)
        {
            _categoryService = categoryService;
            _documentImageService = documentImageService;
        }
        [HttpGet]
        public async Task<BaseResponse> ListCategories()
        {
            return new BaseResponse().SetResult(await _categoryService.ListCategories());
        }
        [HttpPost]
        public async Task<BaseResponse> UpdateCategory([FromForm] CategoryModel model)
        {
            return new BaseResponse().SetResult(await _categoryService.UpdateCategory(new Category
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                IsActive = model.IsActive,
                DocumentGuid = model.Image != null ? (await _documentImageService.SaveAsync(model.Image.OpenReadStream())).Guid : null
            }));
        }
        [HttpPost]
        public async Task<BaseResponse> ToggleCategoryStatus(CategoryModel model)
        {
            return new BaseResponse().SetResult(await _categoryService.ToggleCategoryStatus(model.Id));
        }

    }
}
