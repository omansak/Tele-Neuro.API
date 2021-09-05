using PlayCore.Core.CustomException;
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
        public async Task<List<Category>> ListActiveCategories()
        {
            return await _categoryRepository.ListAsync(i => i.IsActive == true);
        }
        public async Task<List<Category>> ListCategories()
        {
            return await _categoryRepository.ListAllAsync();
        }

        public async Task<int> UpdateCategory(Category category)
        {
            if (category.Id > 0)
            {
                var categoryRow = await _categoryRepository.FindOrDefaultAsync(i => i.Id == category.Id);
                if (categoryRow != null)
                {
                    categoryRow.Name = category.Name;
                    categoryRow.Description = category.Description;
                    ///categoryRow.ImagePath = category.ImagePath;
                    categoryRow.CreatedDate = System.DateTime.Now;
                    //categoryRow.CreatedUser = category.CreatedUser;
                    var result = await _categoryRepository.UpdateAsync(categoryRow);
                    return result.Id;
                }
                throw new UIException("Kategori bulunamadi");
            }
            else
            {

                category.CreatedDate = System.DateTime.Now;
                var result = await _categoryRepository.InsertAsync(new Category
                {
                    Name = category.Name,
                    Description = category.Description,
                    IsActive = true,
                    CreatedDate = System.DateTime.Now

                });
                return result.Id;
            }
        }

        public async Task<bool> ToggleCategoryStatus(int categoryId)
        {
            var categoryRow = await _categoryRepository.FindOrDefaultAsync(i => i.Id == categoryId);
            if (categoryRow != null)
            {
                categoryRow.IsActive = !categoryRow.IsActive;
                await _categoryRepository.UpdateAsync(categoryRow);
                return true;
            }

            throw new UIException("Kategori bulunamadi");
        }
    }
}
