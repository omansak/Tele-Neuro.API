using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.Extension;
using PlayCore.Core.Model;
using System.Threading.Tasks;
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

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<BaseResponse> ListActiveCategories()
        {
            return new BaseResponse().SetResult(await _categoryService.ListActiveCategories());
        }
        [HttpGet]
        public async Task<BaseResponse> ListCategories()
        {
            return new BaseResponse().SetResult(await _categoryService.ListCategories());
        }

        [HttpPost]
        public async Task<BaseResponse> UpdateCategory(CategoryModel model)
        {
            return new BaseResponse().SetResult(await _categoryService.UpdateCategory(new Category
            {
                Name = model.Name,
                Description = model.Description,
                Id = model.Id
            }));
        }

        [HttpPost]
        public async Task<BaseResponse> ToggleCategoryStatus(CategoryModel model)
        {
            return new BaseResponse().SetResult(await _categoryService.ToggleCategoryStatus(model.Id));
        }

    }
}
