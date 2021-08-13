using PlayCore.Core.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeleNeuro.Entities;
using TeleNeuro.Entity.Context;

namespace TeleNeuro.Service.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly IBaseRepository<Category, TeleNeuroDatabaseContext> _categoryRepository;

        public CategoryService(IBaseRepository<Category, TeleNeuroDatabaseContext> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Category>> GetActiveCategories()
        {
            return await _categoryRepository.ListAsync(i => i.IsActive == true);
        }

        public async Task<int> InsertCategory(Category category)
        {
            var result = await _categoryRepository.InsertAsync(category);
            return result.Id;
        }
    }
}
