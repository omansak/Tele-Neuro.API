using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlayCore.Core.CustomException;
using PlayCore.Core.Model;
using PlayCore.Core.Repository;
using TeleNeuro.Entities;
using TeleNeuro.Entity.Context;
using TeleNeuro.Service.ProgramService.Models;

namespace TeleNeuro.Service.ProgramService
{
    public class ProgramService : IProgramService
    {
        private readonly IBaseRepository<Program, TeleNeuroDatabaseContext> _programRepository;
        private readonly IBaseRepository<Category, TeleNeuroDatabaseContext> _categoryRepository;

        public ProgramService(IBaseRepository<Program, TeleNeuroDatabaseContext> programRepository, IBaseRepository<Category, TeleNeuroDatabaseContext> categoryRepository)
        {
            _programRepository = programRepository;
            _categoryRepository = categoryRepository;
        }
        /// <summary>
        /// Returns ProgramInfo
        /// </summary>
        /// <param name="pageInfo"></param>
        /// <returns></returns>
        public async Task<List<ProgramInfo>> ListPrograms(PageInfo pageInfo = null)
        {
            return await GetQueryableProgram(pageInfo: pageInfo)
                .ToListAsync();
        }
        /// <summary>
        /// Returns ProgramInfo counts
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountPrograms()
        {
            return await GetQueryableProgram()
                .CountAsync();
        }
        /// <summary>
        /// Returns ProgramInfo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ProgramInfo> GetProgram(int id)
        {
            return await GetQueryableProgram(i => i.Id == id)
                .SingleOrDefaultAsync();
        }
        /// <summary>
        /// Insert or update Program (CreatedDate can not modify)
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public async Task<ProgramInfo> UpdateProgram(Program program)
        {
            if (program.Id > 0)
            {
                var programRow = await _programRepository.FindOrDefaultAsync(i => i.Id == program.Id);
                if (programRow != null)
                {
                    programRow.CategoryId = program.CategoryId;
                    programRow.Name = program.Name;
                    programRow.Description = program.Description;
                    programRow.IsActive = program.IsActive;
                    programRow.IsPublic = program.IsPublic;
                    programRow.CreatedDate = System.DateTime.Now;
                    var result = await _programRepository.UpdateAsync(programRow);
                    return await GetProgram(result.Id);
                }
                throw new UIException("Program bulunamadi");
            }
            else
            {
                var result = await _programRepository.InsertAsync(new Program
                {
                    CategoryId = program.CategoryId,
                    Name = program.Name,
                    Description = program.Description,
                    IsActive = true,
                    IsPublic = program.IsPublic,
                    CreatedDate = System.DateTime.Now
                });
                return await GetProgram(result.Id);
            }
        }
        /// <summary>
        /// Toggle Program IsActive Status
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        public async Task<bool> ToggleProgramStatus(int programId)
        {
            var programRow = await _programRepository.FindOrDefaultAsync(i => i.Id == programId);
            if (programRow != null)
            {
                programRow.IsActive = !programRow.IsActive;
                await _programRepository.UpdateAsync(programRow);
                return true;
            }

            throw new UIException("Program bulunamadi");
        }
        /// <summary>
        /// Return ProgramInfo Queryable
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="pageInfo"></param>
        /// <returns></returns>
        private IQueryable<ProgramInfo> GetQueryableProgram(Expression<Func<Program, bool>> expression = null, PageInfo pageInfo = null)
        {
            var query = _programRepository.GetQueryable().AsQueryable();
            if (expression != null)
            {
                query = query.Where(expression);
            }

            var queryableProgram = query
                .OrderByDescending(i => i.IsActive)
                .ThenByDescending(i => i.CreatedDate)
                .Join(_categoryRepository.GetQueryable(), i => i.CategoryId, j => j.Id, (i, j) => new ProgramInfo()
                {
                    Program = i,
                    Category = j
                });

            if (pageInfo != null)
            {
                queryableProgram = queryableProgram
                    .Skip((pageInfo.Page - 1) * pageInfo.PageSize)
                    .Take(pageInfo.PageSize);
            }
            return queryableProgram;
        }
    }
}
