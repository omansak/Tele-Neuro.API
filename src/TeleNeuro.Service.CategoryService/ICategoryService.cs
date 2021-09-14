using System.Collections.Generic;
using System.Threading.Tasks;
using PlayCore.Core.Model;
using TeleNeuro.Entities;
using TeleNeuro.Service.CategoryService.Models;

namespace TeleNeuro.Service.CategoryService
{
    public interface ICategoryService
    {
        Task<List<CategoryInfo>> ListCategories(PageInfo pageInfo = null);
        Task<int> CountCategories();
        Task<CategoryInfo> GetCategory(int id);
        Task<CategoryInfo> UpdateCategory(Category category);
        Task<bool> ToggleCategoryStatus(int categoryId);

    }
}
