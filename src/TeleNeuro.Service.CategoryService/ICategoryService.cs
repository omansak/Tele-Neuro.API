using System.Collections.Generic;
using System.Threading.Tasks;
using TeleNeuro.Entities;
using TeleNeuro.Service.CategoryService.Models;

namespace TeleNeuro.Service.CategoryService
{
    public interface ICategoryService
    {
        Task<List<CategoryInfo>> ListCategories();
        Task<CategoryInfo> GetCategory(int id);
        Task<CategoryInfo> UpdateCategory(Category category);
        Task<bool> ToggleCategoryStatus(int categoryId);

    }
}
