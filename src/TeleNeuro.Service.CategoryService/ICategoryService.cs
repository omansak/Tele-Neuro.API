using System.Collections.Generic;
using System.Threading.Tasks;
using PlayCore.Core.Model;
using TeleNeuro.Entities;
using TeleNeuro.Service.CategoryService.Models;

namespace TeleNeuro.Service.CategoryService
{
    public interface ICategoryService
    {
        /// <summary>
        /// Returns Categories
        /// </summary>
        /// <returns></returns>
        Task<List<CategoryInfo>> ListCategories(PageInfo pageInfo = null, int? assignedUserId = null);

        /// <summary>
        /// Returns Active Categories
        /// </summary>
        /// <returns></returns>
        Task<List<CategoryInfo>> ListActiveCategories(PageInfo pageInfo = null, int? assignedUserId = null);

        /// <summary>
        /// Returns Active Categories count
        /// </summary>
        /// <returns></returns>
        Task<int> CountActiveCategories(int? assignedUserId = null);

        /// <summary>
        /// Returns Categories count
        /// </summary>
        /// <returns></returns>
        Task<int> CountCategories(int? assignedUserId = null);

        /// <summary>
        /// Returns CategoryInfo
        /// </summary>
        /// <param name="id">Category's Id</param>
        /// <returns></returns>
        Task<CategoryInfo> GetCategory(int id, int? assignedUserId = null);

        /// <summary>
        /// Insert or update Category (CreatedDate can not modify)
        /// </summary>
        /// <param name="category">Model</param>
        /// <returns>Category Id</returns>
        Task<CategoryInfo> UpdateCategory(Category category);

        /// <summary>
        /// Toggle Category IsActive Status
        /// </summary>
        /// <param name="categoryId">Category's Id</param>
        /// <returns></returns>
        Task<bool> ToggleCategoryStatus(int categoryId);
    }
}