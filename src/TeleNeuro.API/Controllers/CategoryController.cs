using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.Extension;
using PlayCore.Core.Model;
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

        [HttpPost]
        public async Task<BaseResponse> GetActiveCategories()
        {
            return new BaseResponse().SetResult(await _categoryService.GetActiveCategories());
        }

        [HttpPost]
        public async Task<BaseResponse> InsertCategory(CategoryModel model)
        {
            return new BaseResponse().SetResult(await _categoryService.InsertCategory(new Category
            {
                Name = model.Name,
                Description = model.Description
            }));
        }
    }
}
