using PlayCore.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PlayCore.Core.Repository
{
    public interface IBaseRepository<TEntity, TContext> where TEntity : class
    {
        DbSet<TEntity> GetQueryable();
        IQueryable<TEntity> GetQueryable(ISpecification<TEntity> filter);

        // Find
        Task<TEntity> FindByIdAsync(int id);
        Task<TEntity> FirstOrDefaultAsync(ISpecification<TEntity> filter);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter);
        Task<TEntity> SingleOrDefaultAsync(ISpecification<TEntity> filter);
        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> filter);

        // List
        Task<List<TEntity>> ListAllAsync();
        Task<List<TEntity>> ListAsync(ISpecification<TEntity> filter);
        Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>> filter);

        // CRUD
        Task<TEntity> InsertAsync(TEntity entity);
        Task<IEnumerable<TEntity>> InsertRangeAsync(IEnumerable<TEntity> entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entity);
        Task<TEntity> DeleteAsync(TEntity entity);
        Task<IEnumerable<TEntity>> DeleteRangeAsync(IEnumerable<TEntity> entity);

        // Aggregate
        Task<int> CountAsync();
        Task<int> CountAsync(ISpecification<TEntity> spec);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> filter);

        Task<bool> AnyAsync(ISpecification<TEntity> spec);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter);
        IEnumerable<IGrouping<TKey, TEntity>> GroupBy<TKey>(Func<TEntity, TKey> selector);

        // Filter
        Task<IEnumerable<TEntity>> ListFilterAsync(BaseFilterModel baseFilterModel);
        Task<int> CountFilterAsync(BaseFilterModel baseFilterModel, bool includeFilters = true, bool includePaging = true);

        // FromSql
        Task<IEnumerable<TEntity>> FromSqlListAsync(string raw, params object[] parameters);
        Task<TEntity> FromSqlAsync(string raw, params object[] parameters);
        Task<TEntity> ExecuteTextScalarAsync(string raw, params object[] parameters);
        Task<TEntity> ExecuteTableScalarAsync(string raw, params object[] parameters);
        Task<TEntity> ExecuteProcedureScalarAsync(string raw, params object[] parameters);
        Task<int> ExecuteTextNonQueryAsync(string raw, params object[] parameters);
        Task<int> ExecuteTableNonQueryAsync(string raw, params object[] parameters);
        Task<int> ExecuteProcedureNonQueryAsync(string raw, params object[] parameters);
        Task<int> ExecuteSqlRawAsync(string raw, params object[] parameters);

    }

}
