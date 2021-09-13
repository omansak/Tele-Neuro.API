using System;
using Microsoft.EntityFrameworkCore;
using PlayCore.Core.CustomException;
using PlayCore.Core.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using TeleNeuro.Entities;
using TeleNeuro.Entity.Context;

namespace TeleNeuro.Service.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly IBaseRepository<Category, TeleNeuroDatabaseContext> _categoryRepository;
        private readonly IBaseRepository<Document, TeleNeuroDatabaseContext> _documentRepository;

        public CategoryService(IBaseRepository<Category, TeleNeuroDatabaseContext> categoryRepository, IBaseRepository<Document, TeleNeuroDatabaseContext> documentRepository)
        {
            _categoryRepository = categoryRepository;
            _documentRepository = documentRepository;
        }
        public async Task<List<Category>> ListCategories()
        {
            return await GetQueryableCategory()
                .ToListAsync();
        }
        public async Task<Category> GetCategory(int id)
        {
            return await GetQueryableCategory(i => i.Id == id)
                .SingleOrDefaultAsync();
        }
        /// <summary>
        /// Insert or update Category (CreatedDate can not modify)
        /// </summary>
        /// <param name="category">Model</param>
        /// <returns>Category Id</returns>
        public async Task<Category> UpdateCategory(Category category)
        {
            if (category.Id > 0)
            {
                var categoryRow = await _categoryRepository.FindOrDefaultAsync(i => i.Id == category.Id);
                if (categoryRow != null)
                {
                    categoryRow.Name = category.Name;
                    categoryRow.Description = category.Description;
                    categoryRow.IsActive = category.IsActive;
                    categoryRow.DocumentGuid = category.DocumentGuid;
                    categoryRow.CreatedDate = System.DateTime.Now;
                    var result = await _categoryRepository.UpdateAsync(categoryRow);
                    return await GetCategory(result.Id);
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
                    CreatedDate = System.DateTime.Now,
                    DocumentGuid = category.DocumentGuid
                });
                return await GetCategory(result.Id);
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

        private IQueryable<Category> GetQueryableCategory(Expression<Func<Category, bool>> expression = null)
        {
            var query = _categoryRepository
                .GetQueryable()
                .GroupJoin(_documentRepository.GetQueryable(), i => i.DocumentGuid, j => j.Guid, (i, j) => new
                {
                    Category = i,
                    Document = j
                })
                .SelectMany(i => i.Document.DefaultIfEmpty(), (i, j) => new Category
                {
                    Id = i.Category.Id,
                    IsActive = i.Category.IsActive,
                    Name = i.Category.Name,
                    DocumentGuid = i.Category.DocumentGuid,
                    Document = j,
                    CreatedDate = i.Category.CreatedDate,
                    CreatedUser = i.Category.CreatedUser,
                    Description = i.Category.Description,
                });

            if (expression != null)
            {
                query = query.Where(expression);
            }

            return query.OrderByDescending(i => i.IsActive)
            .ThenByDescending(i => i.CreatedDate);
        }
    }
}
