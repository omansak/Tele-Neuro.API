using Microsoft.EntityFrameworkCore;
using PlayCore.Core.CustomException;
using PlayCore.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PlayCore.Core.Model;
using TeleNeuro.Entities;
using TeleNeuro.Entity.Context;
using TeleNeuro.Service.CategoryService.Models;

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
        /// <summary>
        /// Returns Categories
        /// </summary>
        /// <returns></returns>
        public async Task<List<CategoryInfo>> ListCategories(PageInfo pageInfo = null)
        {
            return await GetQueryableCategory(pageInfo: pageInfo)
                .ToListAsync();
        }
        /// <summary>
        /// Returns Categories count
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountCategories()
        {
            return await GetQueryableCategory()
                .CountAsync();
        }
        /// <summary>
        /// Return Category
        /// </summary>
        /// <param name="id">Category's Id</param>
        /// <returns></returns>
        public async Task<CategoryInfo> GetCategory(int id)
        {
            return await GetQueryableCategory(i => i.Id == id)
                .SingleOrDefaultAsync();
        }
        /// <summary>
        /// Insert or update Category (CreatedDate can not modify)
        /// </summary>
        /// <param name="category">Model</param>
        /// <returns>Category Id</returns>
        public async Task<CategoryInfo> UpdateCategory(Category category)
        {
            if (category.Id > 0)
            {
                var categoryRow = await _categoryRepository.FindOrDefaultAsync(i => i.Id == category.Id);
                if (categoryRow != null)
                {
                    categoryRow.Name = category.Name;
                    categoryRow.Description = category.Description;
                    categoryRow.IsActive = category.IsActive;
                    categoryRow.CreatedDate = System.DateTime.Now;
                    if (!string.IsNullOrWhiteSpace(category.DocumentGuid))
                        categoryRow.DocumentGuid = category.DocumentGuid;

                    var result = await _categoryRepository.UpdateAsync(categoryRow);
                    return await GetCategory(result.Id);
                }
                throw new UIException("Kategori bulunamadi");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(category.DocumentGuid))
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
                throw new UIException("Kategori resmi bulunamadı.");
            }
        }
        /// <summary>
        /// Toggle Category IsActive Status
        /// </summary>
        /// <param name="categoryId">Category's Id</param>
        /// <returns></returns>
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
        /// <summary>
        /// Return CategoryInfo Queryable
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private IQueryable<CategoryInfo> GetQueryableCategory(Expression<Func<Category, bool>> expression = null, PageInfo pageInfo = null)
        {
            var query = _categoryRepository.GetQueryable().AsQueryable();
            if (expression != null)
            {
                query = query.Where(expression);
            }

            var queryableCategory = query
               .OrderByDescending(i => i.IsActive)
               .ThenByDescending(i => i.CreatedDate)
               .Join(_documentRepository.GetQueryable(), i => i.DocumentGuid, j => j.Guid, (i, j) => new CategoryInfo
               {
                   Category = i,
                   Document = j
               });

            if (pageInfo != null)
            {
                queryableCategory = queryableCategory
                    .Skip((pageInfo.Page - 1) * pageInfo.PageSize)
                    .Take(pageInfo.PageSize);
            }
            return queryableCategory;
        }
    }
}
