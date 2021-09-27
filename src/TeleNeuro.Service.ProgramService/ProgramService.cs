using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlayCore.Core.CustomException;
using PlayCore.Core.Extension;
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
        private readonly IBaseRepository<TeleNeuroDatabaseContext> _baseRepository;

        public ProgramService(IBaseRepository<Program, TeleNeuroDatabaseContext> programRepository, IBaseRepository<Category, TeleNeuroDatabaseContext> categoryRepository, IBaseRepository<TeleNeuroDatabaseContext> baseRepository)
        {
            _programRepository = programRepository;
            _categoryRepository = categoryRepository;
            _baseRepository = baseRepository;
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
            if (program.CategoryId <= 0)
                throw new UIException("Kategori bulunamadi");

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

            throw new UIException("Program bulunamadı");
        }
        /// <summary>
        /// Assign Exercise to Program
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Assign's Id</returns>
        public async Task<int> AssignExercise(AssignExerciseModel model)
        {
            if (!await _programRepository.AnyAsync(i => i.IsActive && i.Id == model.ProgramId))
                throw new UIException("Program bulunamadı");

            if (!await _baseRepository.AnyAsync<Exercise>(i => i.IsActive && i.Id == model.ExerciseId))
                throw new UIException("Egzersiz bulunamadı");

            var exerciseProgramRelational = await _baseRepository.InsertAsync(new ExerciseProgramRelation
            {
                ProgramId = model.ProgramId,
                ExerciseId = model.ExerciseId,
                CreatedDate = DateTime.Now,
                AutoSkip = model.AutoSkip,
                AutoSkipTime = model.AutoSkipTime,
                CreatedUser = model.UserId
            });

            if (exerciseProgramRelational?.Id > 0)
            {
                if (model.Properties?.Count > 0)
                {
                    foreach (var item in model.Properties)
                    {
                        if (item.Id > 0 & !string.IsNullOrWhiteSpace(item.Value))
                        {
                            await _baseRepository.InsertAsync(
                                new ExerciseProgramRelationProperty
                                {
                                    ExerciseRelationId = exerciseProgramRelational.Id,
                                    ExercisePropertyId = item.Id,
                                    Value = item.Value,
                                    CreatedDate = DateTime.Now,
                                    CreatedUser = model.UserId
                                });
                        }
                    }
                }

                return exerciseProgramRelational.Id;
            }
            throw new UIException("Egzersiz atanamadı");
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
