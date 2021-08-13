using System.Collections.Generic;
using System.Threading.Tasks;
using TeleNeuro.Entities;

namespace TeleNeuro.Service.CategoryService
{
    public interface ICategoryService
    {
        Task<List<Category>> GetActiveCategories();
        Task<int> InsertCategory(Category category);
    }
}
