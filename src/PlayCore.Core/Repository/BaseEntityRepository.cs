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
    public class BaseRepository<TEntity, TContext> : IBaseRepository<TEntity, TContext>
        where TEntity : class, new()
        where TContext : DbContext
    {

        private readonly TContext _context;
        private readonly DbSet<TEntity> _entities;
        private readonly CancellationToken _cancellationToken;

        public BaseRepository(TContext context)
        {
            _context = context;
            _entities = context.Set<TEntity>();
            _cancellationToken = new CancellationToken();
        }
        public BaseRepository(TContext context, CancellationToken cancellationToken)
        {
            _context = context;
            _entities = context.Set<TEntity>();
            _cancellationToken = cancellationToken;
        }
        public DbSet<TEntity> GetQueryable()
        {
            return _entities;
        }
        public IQueryable<TEntity> GetQueryable(ISpecification<TEntity> filter)
        {
            return ApplySpecification(filter);
        }
        public async Task<TEntity> FindByIdAsync(int id)
        {
            return await _entities.FindAsync(id);
        }
        public async Task<TEntity> FirstOrDefaultAsync(ISpecification<TEntity> filter)
        {
            return await ApplySpecification(filter).FirstOrDefaultAsync(cancellationToken: _cancellationToken);
        }
        public async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _entities.SingleOrDefaultAsync(filter, cancellationToken: _cancellationToken);
        }
        public async Task<TEntity> SingleOrDefaultAsync(ISpecification<TEntity> filter)
        {
            return await ApplySpecification(filter).SingleOrDefaultAsync(cancellationToken: _cancellationToken);
        }
        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _entities.FirstOrDefaultAsync(filter, cancellationToken: _cancellationToken);
        }
        public async Task<List<TEntity>> ListAllAsync()
        {
            return await _entities.ToListAsync(cancellationToken: _cancellationToken);
        }
        public async Task<List<TEntity>> ListAsync(ISpecification<TEntity> filter)
        {
            return await ApplySpecification(filter).ToListAsync(cancellationToken: _cancellationToken);
        }
        public async Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _entities.Where(filter).ToListAsync(_cancellationToken);
        }
        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            _entities.Add(entity);
            await _context.SaveChangesAsync(_cancellationToken);
            return entity;
        }
        public async Task<IEnumerable<TEntity>> InsertRangeAsync(IEnumerable<TEntity> entity)
        {
            _entities.AddRange(entity);
            await _context.SaveChangesAsync(_cancellationToken);
            return entity;
        }
        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _entities.Update(entity);
            await _context.SaveChangesAsync(_cancellationToken);
            return entity;
        }
        public async Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entity)
        {
            _entities.UpdateRange(entity);
            await _context.SaveChangesAsync(_cancellationToken);
            return entity;
        }
        public async Task<TEntity> DeleteAsync(TEntity entity)
        {
            _entities.Remove(entity);
            await _context.SaveChangesAsync(_cancellationToken);
            return entity;
        }
        public async Task<IEnumerable<TEntity>> DeleteRangeAsync(IEnumerable<TEntity> entity)
        {
            _entities.RemoveRange(entity);
            await _context.SaveChangesAsync(_cancellationToken);
            return entity;
        }
        public async Task<int> CountAsync()
        {
            return await _entities.CountAsync(_cancellationToken);
        }
        public async Task<int> CountAsync(ISpecification<TEntity> spec)
        {
            return await ApplySpecification(spec).CountAsync(cancellationToken: _cancellationToken);
        }
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _entities.CountAsync(filter, _cancellationToken);
        }
        public async Task<bool> AnyAsync(ISpecification<TEntity> spec)
        {
            return await ApplySpecification(spec).AnyAsync(_cancellationToken);
        }
        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _entities.AnyAsync(filter, _cancellationToken);
        }
        public async Task<IEnumerable<TEntity>> ListFilterAsync(BaseFilterModel baseFilterModel)
        {
            return await _entities.AsQueryable().ToQueryableFromBaseFilter(baseFilterModel).ToListAsync(cancellationToken: _cancellationToken);
        }
        public async Task<int> CountFilterAsync(BaseFilterModel baseFilterModel, bool includeFilters = true, bool includePaging = true)
        {
            return await _entities.AsQueryable().ToQueryableFromBaseFilter(baseFilterModel, includeFilters, includePaging).CountAsync(cancellationToken: _cancellationToken);
        }
        public IEnumerable<IGrouping<TKey, TEntity>> GroupBy<TKey>(Func<TEntity, TKey> selector)
        {
            return _entities.GroupBy(selector);
        }
        public async Task<IEnumerable<TEntity>> FromSqlListAsync(string raw, params object[] parameters)
        {
            return await _context.Set<TEntity>().FromSqlRaw(raw, parameters).ToListAsync(cancellationToken: _cancellationToken);
        }
        public async Task<TEntity> FromSqlAsync(string raw, params object[] parameters)
        {
            return await _context.Set<TEntity>().FromSqlRaw(raw, parameters).FirstOrDefaultAsync(cancellationToken: _cancellationToken);
        }
        public async Task<TEntity> ExecuteTextScalarAsync(string raw, params object[] parameters)
        {
            return await ExecuteScalarAsync<TEntity>(raw, CommandType.Text, parameters);
        }
        public async Task<TEntity> ExecuteTableScalarAsync(string raw, params object[] parameters)
        {
            return await ExecuteScalarAsync<TEntity>(raw, CommandType.TableDirect, parameters);
        }
        public async Task<TEntity> ExecuteProcedureScalarAsync(string raw, params object[] parameters)
        {
            return await ExecuteScalarAsync<TEntity>(raw, CommandType.StoredProcedure, parameters);
        }
        public async Task<int> ExecuteTextNonQueryAsync(string raw, params object[] parameters)
        {
            return await ExecuteNonQueryAsync(raw, CommandType.Text, parameters);
        }
        public async Task<int> ExecuteTableNonQueryAsync(string raw, params object[] parameters)
        {
            return await ExecuteNonQueryAsync(raw, CommandType.TableDirect, parameters);
        }
        public async Task<int> ExecuteProcedureNonQueryAsync(string raw, params object[] parameters)
        {
            return await ExecuteNonQueryAsync(raw, CommandType.StoredProcedure, parameters);
        }
        private async Task<TResult> ExecuteScalarAsync<TResult>(string raw, CommandType commandType, params object[] parameters)
        {
            using (DbConnection connection = _context.Database.GetDbConnection())
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandType = commandType;
                    command.CommandText = raw;
                    command.Parameters.AddRange(parameters);
                    return (TResult)await command.ExecuteScalarAsync(_cancellationToken);
                }
            }
        }
        private async Task<int> ExecuteNonQueryAsync(string raw, CommandType commandType, params object[] parameters)
        {
            using (DbConnection connection = _context.Database.GetDbConnection())
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandType = commandType;
                    command.CommandText = raw;
                    command.Parameters.AddRange(parameters);
                    return await command.ExecuteNonQueryAsync(_cancellationToken);
                }
            }
        }
        private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
        {
            return SpecificationEvaluator<TEntity>.GetQuery(_entities.AsQueryable(), spec);
        }
    }
}