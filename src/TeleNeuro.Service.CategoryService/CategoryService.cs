using Microsoft.EntityFrameworkCore;
using PlayCore.Core.CustomException;
using PlayCore.Core.Model;
using PlayCore.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TeleNeuro.Entities;
using TeleNeuro.Entity.Context;
using TeleNeuro.Service.CategoryService.Models;

namespace TeleNeuro.Service.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly IBaseRepository<Category, TeleNeuroDatabaseContext> _categoryRepository;
        private readonly IBaseRepository<Document, TeleNeuroDatabaseContext> _documentRepository;
        private readonly IBaseRepository<Program, TeleNeuroDatabaseContext> _programRepository;
        private readonly IBaseRepository<TeleNeuroDatabaseContext> _baseRepository;

        public CategoryService(IBaseRepository<Category, TeleNeuroDatabaseContext> categoryRepository, IBaseRepository<Document, TeleNeuroDatabaseContext> documentRepository, IBaseRepository<Program, TeleNeuroDatabaseContext> programRepository, IBaseRepository<TeleNeuroDatabaseContext> baseRepository)
        {
            _categoryRepository = categoryRepository;
            _documentRepository = documentRepository;
            _programRepository = programRepository;
            _baseRepository = baseRepository;
        }
        /// <summary>
        /// Returns Categories
        /// </summary>
        /// <returns></returns>
        public async Task<List<CategoryInfo>> ListCategories(PageInfo pageInfo = null, int? assignedUserId = null)
        {
            return await GetQueryableCategory(pageInfo: pageInfo, assignedUserId: assignedUserId)
                .ToListAsync();
        }
        /// <summary>
        /// Returns Active Categories
        /// </summary>
        /// <returns></returns>
        public async Task<List<CategoryInfo>> ListActiveCategories(PageInfo pageInfo = null, int? assignedUserId = null)
        {
            return await GetQueryableCategory(i => i.IsActive, pageInfo: pageInfo, assignedUserId: assignedUserId)
                .ToListAsync();
        }
        /// <summary>
        /// Returns Active Categories count
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountActiveCategories(int? assignedUserId = null)
        {
            return await GetQueryableCategory(i => i.IsActive, assignedUserId: assignedUserId)
                .CountAsync();
        }
        /// <summary>
        /// Returns Categories count
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountCategories(int? assignedUserId = null)
        {
            return await GetQueryableCategory(assignedUserId: assignedUserId)
                .CountAsync();
        }

        /// <summary>
        /// Returns CategoryInfo
        /// </summary>
        /// <param name="id">Category's Id</param>
        /// <param name="assignedUserId">User Id</param>
        /// <returns></returns>
        public async Task<CategoryInfo> GetCategory(int id, int? assignedUserId = null)
        {
            return await GetQueryableCategory(i => i.Id == id, assignedUserId: assignedUserId)
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
                var categoryRow = await _categoryRepository.FirstOrDefaultAsync(i => i.Id == category.Id);
                if (categoryRow != null)
                {
                    categoryRow.Name = category.Name;
                    categoryRow.Description = category.Description;
                    categoryRow.IsActive = category.IsActive;
                    categoryRow.CreatedDate = System.DateTime.Now;
                    categoryRow.CreatedUser = category.CreatedUser;
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
                        DocumentGuid = category.DocumentGuid,
                        CreatedUser = category.CreatedUser
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
            var categoryRow = await _categoryRepository.FirstOrDefaultAsync(i => i.Id == categoryId);
            if (categoryRow != null)
            {
                categoryRow.IsActive = !categoryRow.IsActive;
                await _categoryRepository.UpdateAsync(categoryRow);
                return true;
            }

            throw new UIException("Kategori bulunamadi");
        }

        /// <summary>
        /// Returns CategoryInfo Queryable
        /// </summary>
        /// <returns></returns>
        private IQueryable<CategoryInfo> GetQueryableCategory(Expression<Func<Category, bool>> expression = null, PageInfo pageInfo = null, int? assignedUserId = null)
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
                   Document = j,
                   ProgramCount = _programRepository
                       .GetQueryable()
                       .Where(k => k.IsActive)
                       .Where(k => k.IsPublic || assignedUserId == null || _baseRepository.GetQueryable<UserProgramRelation>().Where(l => l.UserId == 4).Select(l => l.ProgramId).Contains(k.Id))
                       .Count(k => k.CategoryId == i.Id)
               });

            if (pageInfo != null && pageInfo.Page > -1 && pageInfo.PageSize > 0)
            {
                queryableCategory = queryableCategory
                    .Skip((pageInfo.Page - 1) * pageInfo.PageSize)
                    .Take(pageInfo.PageSize);
            }
            return queryableCategory;
        }
    }
}
