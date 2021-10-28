using Microsoft.EntityFrameworkCore;
using PlayCore.Core.Extension;
using PlayCore.Core.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace PlayCore.Core.Repository
{
    public class BaseRepository<TContext> : IBaseRepository<TContext> where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly CancellationToken _cancellationToken;

        public BaseRepository(TContext context)
        {
            _context = context;
            _cancellationToken = new CancellationToken();
        }
        public BaseRepository(TContext context, CancellationToken cancellationToken)
        {
            _context = context;
            _cancellationToken = cancellationToken;
        }
        public DbSet<TEntity> GetQueryable<TEntity>() where TEntity : class
        {
            return _context.Set<TEntity>();
        }
        public async Task<TEntity> FindByIdAsync<TEntity>(int id) where TEntity : class
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }
        public async Task<TEntity> FirstOrDefaultAsync<TEntity>(ISpecification<TEntity> filter) where TEntity : class
        {
            return await ApplySpecification(filter).FirstOrDefaultAsync(cancellationToken: _cancellationToken);
        }
        public async Task<TEntity> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            return await _context.Set<TEntity>().FirstOrDefaultAsync(filter, cancellationToken: _cancellationToken);
        }
        public async Task<TEntity> SingleOrDefaultAsync<TEntity>(ISpecification<TEntity> filter) where TEntity : class
        {
            return await ApplySpecification(filter).FirstOrDefaultAsync(cancellationToken: _cancellationToken);
        }
        public async Task<TEntity> SingleOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            return await _context.Set<TEntity>().FirstOrDefaultAsync(filter, cancellationToken: _cancellationToken);
        }
        public async Task<List<TEntity>> ListAllAsync<TEntity>() where TEntity : class
        {
            return await _context.Set<TEntity>().ToListAsync(cancellationToken: _cancellationToken);
        }
        public async Task<List<TEntity>> ListAsync<TEntity>(ISpecification<TEntity> filter) where TEntity : class
        {
            return await ApplySpecification(filter).ToListAsync(cancellationToken: _cancellationToken);
        }
        public async Task<List<TEntity>> ListAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            return await _context.Set<TEntity>().Where(filter).ToListAsync(_cancellationToken);
        }
        public async Task<TEntity> InsertAsync<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync(_cancellationToken);
            return entity;
        }
        public async Task<IEnumerable<TEntity>> InsertRangeAsync<TEntity>(IEnumerable<TEntity> entity) where TEntity : class
        {
            _context.Set<TEntity>().AddRange(entity);
            await _context.SaveChangesAsync(_cancellationToken);
            return entity;
        }
        public async Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync(_cancellationToken);
            return entity;
        }
        public async Task<IEnumerable<TEntity>> UpdateRangeAsync<TEntity>(IEnumerable<TEntity> entity) where TEntity : class
        {
            _context.Set<TEntity>().UpdateRange(entity);
            await _context.SaveChangesAsync(_cancellationToken);
            return entity;
        }
        public async Task<TEntity> DeleteAsync<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync(_cancellationToken);
            return entity;
        }
        public async Task<IEnumerable<TEntity>> DeleteRangeAsync<TEntity>(IEnumerable<TEntity> entity) where TEntity : class
        {
            _context.Set<TEntity>().RemoveRange(entity);
            await _context.SaveChangesAsync(_cancellationToken);
            return entity;
        }
        public async Task<int> CountAsync<TEntity>() where TEntity : class
        {
            return await _context.Set<TEntity>().CountAsync(_cancellationToken);
        }
        public async Task<int> CountAsync<TEntity>(ISpecification<TEntity> spec) where TEntity : class
        {
            return await _context.Set<TEntity>().CountAsync(_cancellationToken);
        }
        public async Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            return await _context.Set<TEntity>().CountAsync(filter, _cancellationToken);
        }
        public async Task<bool> AnyAsync<TEntity>(ISpecification<TEntity> spec) where TEntity : class
        {
            return await ApplySpecification(spec).AnyAsync(_cancellationToken);
        }
        public async Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            return await _context.Set<TEntity>().AnyAsync(filter, _cancellationToken);
        }
        public async Task<IEnumerable<TEntity>> ListFilterAsync<TEntity>(BaseFilterModel baseFilterModel) where TEntity : class
        {
            return await _context.Set<TEntity>().AsQueryable().ToQueryableFromBaseFilter(baseFilterModel).ToListAsync(cancellationToken: _cancellationToken);
        }
        public async Task<int> CountFilterAsync<TEntity>(BaseFilterModel baseFilterModel, bool includeFilters = true, bool includePaging = true) where TEntity : class
        {
            return await _context.Set<TEntity>().AsQueryable().ToQueryableFromBaseFilter(baseFilterModel, includeFilters, includePaging).CountAsync(cancellationToken: _cancellationToken);
        }
        public async Task<IEnumerable<TEntity>> ExecuteQueryListAsync<TEntity>(string raw, params object[] parameters) where TEntity : class
        {
            return await _context.Set<TEntity>().FromSqlRaw(raw, parameters).ToListAsync(cancellationToken: _cancellationToken);
        }
        public async Task<IEnumerable<TEntity>> ExecuteQueryListAsync<TEntity>(IExecutableQuery command) where TEntity : class
        {
            return await _context.Set<TEntity>().FromSqlRaw(command.GetPreparedCommandText(), command.GetParameters()).ToListAsync(cancellationToken: _cancellationToken);
        }
        public async Task<TEntity> ExecuteQueryAsync<TEntity>(string raw, params object[] parameters) where TEntity : class
        {
            return await _context.Set<TEntity>().FromSqlRaw(raw, parameters).FirstOrDefaultAsync(cancellationToken: _cancellationToken);
        }
        public async Task<TEntity> ExecuteQueryAsync<TEntity>(IExecutableQuery command) where TEntity : class
        {
            return await _context.Set<TEntity>().FromSqlRaw(command.GetPreparedCommandText(), command.GetParameters()).FirstOrDefaultAsync(cancellationToken: _cancellationToken);
        }
        public async Task<TResult> ExecuteTextScalarAsync<TResult>(string raw, params object[] parameters)
        {
            return await ExecuteScalarAsync<TResult>(raw, CommandType.Text, parameters);
        }
        public async Task<TResult> ExecuteScalarAsync<TResult>(IExecutableQuery command)
        {
            return await ExecuteScalarAsync<TResult>(command.GetPreparedCommandText(), command.GetCommandType(), command.GetParameters());
        }
        public async Task<int> ExecuteNonQueryAsync(IExecutableQuery command)
        {
            return await ExecuteNonQueryAsync(command.GetCommandText(), command.GetCommandType(), command.GetParameters());
        }
        public async Task<TResult> ExecuteProcedureScalarAsync<TResult>(string raw, params object[] parameters)
        {
            return await ExecuteScalarAsync<TResult>(raw, CommandType.StoredProcedure, parameters);
        }
        public async Task<int> ExecuteTextNonQueryAsync(string raw, params object[] parameters)
        {
            return await ExecuteNonQueryAsync(raw, CommandType.Text, parameters);
        }
        public async Task<int> ExecuteProcedureNonQueryAsync(string raw, params object[] parameters)
        {
            return await ExecuteNonQueryAsync(raw, CommandType.StoredProcedure, parameters);
        }
        private async Task<TResult> ExecuteScalarAsync<TResult>(string raw, CommandType commandType, params object[] parameters)
        {
            DbConnection connection = _context.Database.GetDbConnection();
            await using DbCommand command = connection.CreateCommand();
            command.CommandType = commandType;
            command.CommandText = raw;
            command.Parameters.AddRange(parameters);
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(_cancellationToken);
            object result = await command.ExecuteScalarAsync(_cancellationToken);
            return result == null ? default : (TResult)result;
        }
        private async Task<int> ExecuteNonQueryAsync(string raw, CommandType commandType, params object[] parameters)
        {
            DbConnection connection = _context.Database.GetDbConnection();
            await using DbCommand command = connection.CreateCommand();
            command.CommandType = commandType;
            command.CommandText = raw;
            command.Parameters.AddRange(parameters);
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(_cancellationToken);
            return await command.ExecuteNonQueryAsync(_cancellationToken);
        }
        private IQueryable<TEntity> ApplySpecification<TEntity>(ISpecification<TEntity> spec) where TEntity : class
        {
            return SpecificationEvaluator<TEntity>.GetQuery(_context.Set<TEntity>().AsQueryable(), spec);
        }
    }
}
