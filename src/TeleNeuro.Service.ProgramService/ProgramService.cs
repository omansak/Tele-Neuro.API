using Microsoft.EntityFrameworkCore;
using PlayCore.Core.CustomException;
using PlayCore.Core.Model;
using PlayCore.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

            int index = await _baseRepository.GetQueryable<ExerciseProgramRelation>()
                .Where(i => i.ProgramId == model.ProgramId)
                .Select(i => i.Sequence)
                .DefaultIfEmpty()
                .MaxAsync();

            var exerciseProgramRelational = await _baseRepository.InsertAsync(new ExerciseProgramRelation
            {
                ProgramId = model.ProgramId,
                ExerciseId = model.ExerciseId,
                CreatedDate = DateTime.Now,
                AutoSkip = model.AutoSkip,
                AutoSkipTime = model.AutoSkipTime,
                Sequence = index + 1,
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
        /// Return Exercises of Program
        /// </summary>
        public async Task<List<ProgramAssignedExerciseInfo>> AssignedExercises(int programId, bool? isActiveExercise = null)
        {
            var programRow = await _programRepository.FindOrDefaultAsync(i => i.Id == programId);
            if (programRow != null)
            {
                return _baseRepository.GetQueryable<ExerciseProgramRelation>()
                       .Where(i => i.ProgramId == programRow.Id)
                       .Join(_baseRepository.GetQueryable<Exercise>(), i => i.ExerciseId, j => j.Id, (i, j) => new
                       {
                           Exercise = j,
                           ProgramRelation = i
                       })
                       .GroupJoin(_baseRepository.GetQueryable<Document>(), i => i.Exercise.DocumentGuid, j => j.Guid, (i, j) => new
                       {
                           Exercise = i.Exercise,
                           ProgramRelation = i.ProgramRelation,
                           ExerciseDocument = j
                       })
                       .SelectMany(i => i.ExerciseDocument.DefaultIfEmpty(), (i, j) => new
                       {
                           Exercise = i.Exercise,
                           ProgramRelation = i.ProgramRelation,
                           ExerciseDocument = j
                       })
                       .Where(i => isActiveExercise == null || i.Exercise.IsActive == isActiveExercise)
                       .GroupJoin(_baseRepository.GetQueryable<ExerciseProgramRelationProperty>(),
                           i => i.ProgramRelation.Id,
                           j => j.ExerciseRelationId, (i, j) => new
                           {
                               Exercise = i.Exercise,
                               Relation = i.ProgramRelation,
                               ExerciseDocument = i.ExerciseDocument,
                               Properties = j
                           })
                       .SelectMany(i => i.Properties.DefaultIfEmpty(), (i, j) => new
                       {
                           Exercise = i.Exercise,
                           Relation = i.Relation,
                           ExerciseDocument = i.ExerciseDocument,
                           Property = j
                       })
                       .GroupJoin(_baseRepository.GetQueryable<ExercisePropertyDefinition>(),
                           i => i.Property.ExercisePropertyId, j => j.Id, (i, j) => new
                           {
                               Exercise = i.Exercise,
                               ExerciseDocument = i.ExerciseDocument,
                               Relation = i.Relation,
                               Property = i.Property,
                               PropertyDefinitions = j
                           })
                       .SelectMany(i => i.PropertyDefinitions.DefaultIfEmpty(), (i, j) => new
                       {
                           Exercise = i.Exercise,
                           ExerciseDocument = i.ExerciseDocument,
                           Relation = i.Relation,
                           Property = i.Property,
                           PropertyDefinition = j
                       })
                       .OrderBy(i => i.Relation.Sequence)
                       .ToListAsync()
                       .ConfigureAwait(false)
                       .GetAwaiter()
                       .GetResult()
                       .GroupBy(i => new { Relation = i.Relation, Exercise = i.Exercise, ExerciseDocument = i.ExerciseDocument })
                       .Select(i => new ProgramAssignedExerciseInfo
                       {
                           RelationId = i.Key.Relation.Id,
                           ProgramId = i.Key.Relation.ProgramId,
                           Sequence = i.Key.Relation.Sequence,
                           AutoSkip = i.Key.Relation.AutoSkip,
                           AutoSkipTime = i.Key.Relation.AutoSkipTime,
                           Exercise = i.Key.Exercise,
                           ExerciseDocument = i.Key.ExerciseDocument,
                           Properties = i.Where(j => j.Property != null && j.PropertyDefinition != null)
                               .Select(j => new ExerciseProperty
                               {
                                   Value = j.Property.Value,
                                   Definition = j.PropertyDefinition
                               })
                               .ToList()
                       })
                       .ToList();
            }
            throw new UIException("Program bulunamadı");
        }
        /// <summary>
        /// Change Sequence Assigned Exercise
        /// </summary>
        /// <param name="relationId"></param>
        /// <param name="direction">-1 : Up, 0 : No Change, 1 : Down</param>
        /// <returns></returns>
        public async Task<bool> ChangeSequenceAssignedExercise(int relationId, int direction)
        {
            if (direction == -1 || direction == 1)
            {
                var relationRow = await _baseRepository.FindOrDefaultAsync<ExerciseProgramRelation>(i => i.Id == relationId);
                if (relationRow != null)
                {
                    var relatedRelation = await _baseRepository
                        .FindOrDefaultAsync<ExerciseProgramRelation>(i => i.ProgramId == relationRow.ProgramId && i.Sequence == relationRow.Sequence + direction);

                    if (relatedRelation != null)
                    {
                        int temp = relatedRelation.Sequence;
                        relatedRelation.Sequence -= direction;
                        await _baseRepository.UpdateAsync(relatedRelation);

                        relationRow.Sequence += direction;
                        await _baseRepository.UpdateAsync(relationRow);
                        return true;
                    }
                    return false;
                }
                throw new UIException("Egzersiz bulunamadı");
            }

            throw new UIException("Yön geçersiz");
        }

        public async Task<bool> DeleteAssignedExercise(int relationId)
        {
            var relationRow = await _baseRepository.FindOrDefaultAsync<ExerciseProgramRelation>(i => i.Id == relationId);
            if (relationRow != null)
            {
                await _baseRepository.DeleteAsync(relationRow);
                int index = 1;
                var assignedExercise = await _baseRepository.ListAsync<ExerciseProgramRelation>(i => i.ProgramId == relationRow.ProgramId);
                if (assignedExercise != null)
                {
                    foreach (var item in assignedExercise)
                    {
                        item.Sequence = index++;
                    }
                    await _baseRepository.UpdateRangeAsync(assignedExercise);
                }

                return true;
            }
            throw new UIException("Egzersiz bulunamadı");
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
                .Join(_categoryRepository.GetQueryable(), i => i.CategoryId, j => j.Id, (i, j) => new
                {
                    Program = i,
                    Category = j
                })
                .GroupJoin(_baseRepository.GetQueryable<Document>(), i => i.Category.DocumentGuid, j => j.Guid, (i, j) => new
                {
                    Program = i.Program,
                    Category = i.Category,
                    CategoryDocument = j
                })
                .SelectMany(i => i.CategoryDocument.DefaultIfEmpty(), (i, j) => new ProgramInfo()
                {
                    Program = i.Program,
                    Category = i.Category,
                    CategoryDocument = j
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
