using System.Collections.Generic;
using System.Threading.Tasks;
using TeleNeuro.Entities;

namespace TeleNeuro.Service.CategoryService
{
    public interface ICategoryService
    {
        Task<List<Category>> ListActiveCategories();
        Task<List<Category>> ListCategories();
        Task<bool> ToggleCategoryStatus(int categoryId);
        Task<int> UpdateCategory(Category category);
    }
}
