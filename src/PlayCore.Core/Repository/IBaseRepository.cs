using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlayCore.Core.Model;

namespace PlayCore.Core.Repository
{
    public interface IBaseRepository<TContext>
    {
        DbSet<TEntity> GetQueryable<TEntity>() where TEntity : class;
        // Find
        Task<TEntity> FindByIdAsync<TEntity>(int id) where TEntity : class;
        Task<TEntity> FindOrDefaultAsync<TEntity>(ISpecification<TEntity> filter) where TEntity : class;
        Task<TEntity> FindOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;

        //List
        Task<List<TEntity>> ListAllAsync<TEntity>() where TEntity : class;
        Task<List<TEntity>> ListAsync<TEntity>(ISpecification<TEntity> filter) where TEntity : class;
        Task<List<TEntity>> ListAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;

        //CRUD
        Task<TEntity> InsertAsync<TEntity>(TEntity entity) where TEntity : class;
        Task<IEnumerable<TEntity>> InsertRangeAsync<TEntity>(IEnumerable<TEntity> entity) where TEntity : class;
        Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : class;
        Task<IEnumerable<TEntity>> UpdateRangeAsync<TEntity>(IEnumerable<TEntity> entity) where TEntity : class;
        Task<TEntity> DeleteAsync<TEntity>(TEntity entity) where TEntity : class;
        Task<IEnumerable<TEntity>> DeleteRangeAsync<TEntity>(IEnumerable<TEntity> entity) where TEntity : class;

        //Aggregate
        Task<int> CountAsync<TEntity>() where TEntity : class;
        Task<int> CountAsync<TEntity>(ISpecification<TEntity> spec) where TEntity : class;
        Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        Task<bool> AnyAsync<TEntity>(ISpecification<TEntity> spec) where TEntity : class;
        Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;

        //BaseFilter
        Task<IEnumerable<TEntity>> ListFilterAsync<TEntity>(BaseFilterModel baseFilterModel) where TEntity : class;
        Task<int> CountFilterAsync<TEntity>(BaseFilterModel baseFilterModel, bool includeFilters = true, bool includePaging = true) where TEntity : class;

        //Executable
        Task<IEnumerable<TEntity>> ExecuteQueryListAsync<TEntity>(string raw, params object[] parameters) where TEntity : class;
        Task<IEnumerable<TEntity>> ExecuteQueryListAsync<TEntity>(IExecutableQuery command) where TEntity : class;
        Task<TEntity> ExecuteQueryAsync<TEntity>(string raw, params object[] parameters) where TEntity : class;
        Task<TEntity> ExecuteQueryAsync<TEntity>(IExecutableQuery command) where TEntity : class;
        Task<TResult> ExecuteTextScalarAsync<TResult>(string raw, params object[] parameters);
        Task<TResult> ExecuteProcedureScalarAsync<TResult>(string raw, params object[] parameters);
        Task<int> ExecuteTextNonQueryAsync(string raw, params object[] parameters);
        Task<int> ExecuteProcedureNonQueryAsync(string raw, params object[] parameters);
        Task<TResult> ExecuteScalarAsync<TResult>(IExecutableQuery command);
        Task<int> ExecuteNonQueryAsync(IExecutableQuery command);
    }
}



