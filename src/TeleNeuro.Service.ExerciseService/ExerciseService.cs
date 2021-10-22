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
using TeleNeuro.Service.ExerciseService.Models;

namespace TeleNeuro.Service.ExerciseService
{
    public class ExerciseService : IExerciseService
    {
        private readonly IBaseRepository<Exercise, TeleNeuroDatabaseContext> _exerciseRepository;
        private readonly IBaseRepository<Document, TeleNeuroDatabaseContext> _documentRepository;

        public ExerciseService(IBaseRepository<Exercise, TeleNeuroDatabaseContext> exerciseRepository, IBaseRepository<Document, TeleNeuroDatabaseContext> documentRepository)
        {
            _exerciseRepository = exerciseRepository;
            _documentRepository = documentRepository;
        }
        /// <summary>
        /// Search Exercises
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public async Task<List<ExerciseInfo>> SearchExercises(string term)
        {
            return await GetQueryableExercise(i => i.Name.Contains(term) || i.Description.Contains(term))
                .ToListAsync();
        }
        /// <summary>
        /// Returns Exercises
        /// </summary>
        /// <returns></returns>
        public async Task<List<ExerciseInfo>> ListExercises(PageInfo pageInfo = null)
        {
            return await GetQueryableExercise(pageInfo: pageInfo)
                .ToListAsync();
        }
        /// <summary>
        /// Returns Exercises count
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountExercises()
        {
            return await GetQueryableExercise()
                .CountAsync();
        }
        /// <summary>
        /// Return Exercise
        /// </summary>
        /// <param name="id">Exercise's Id</param>
        /// <returns></returns>
        public async Task<ExerciseInfo> GetExercise(int id)
        {
            return await GetQueryableExercise(i => i.Id == id)
                .SingleOrDefaultAsync();
        }
        /// <summary>
        /// Return Exercise
        /// </summary>
        /// <param name="id">Exercise's Id</param>
        /// <returns></returns>
        public async Task<ExerciseInfo> GetActiveExercise(int id)
        {
            return await GetQueryableExercise(i => i.IsActive && i.Id == id)
                .SingleOrDefaultAsync();
        }
        /// <summary>
        /// Insert or update Exercise (CreatedDate can not modify)
        /// </summary>
        /// <param name="exercise">Model</param>
        /// <returns>Exercise Id</returns>
        public async Task<ExerciseInfo> UpdateExercise(Exercise exercise)
        {
            if (exercise.Id > 0)
            {
                var exerciseRow = await _exerciseRepository.FindOrDefaultAsync(i => i.Id == exercise.Id);
                if (exerciseRow != null)
                {
                    exerciseRow.Name = exercise.Name;
                    exerciseRow.Description = exercise.Description;
                    exerciseRow.IsActive = exercise.IsActive;
                    exerciseRow.CreatedDate = System.DateTime.Now;
                    if (!string.IsNullOrWhiteSpace(exercise.DocumentGuid))
                        exerciseRow.DocumentGuid = exercise.DocumentGuid;

                    var result = await _exerciseRepository.UpdateAsync(exerciseRow);
                    return await GetExercise(result.Id);
                }
                throw new UIException("Egzersiz bulunamadi");
            }
            else
            {
                exercise.CreatedDate = System.DateTime.Now;
                var result = await _exerciseRepository.InsertAsync(new Exercise
                {
                    Name = exercise.Name,
                    Description = exercise.Description,
                    IsActive = true,
                    CreatedDate = System.DateTime.Now,
                    DocumentGuid = exercise.DocumentGuid
                });
                return await GetExercise(result.Id);
            }
        }
        /// <summary>
        /// Toggle Exercise IsActive Status
        /// </summary>
        /// <param name="exerciseId">Category's Id</param>
        /// <returns></returns>
        public async Task<bool> ToggleExerciseStatus(int exerciseId)
        {
            var exerciseRow = await _exerciseRepository.FindOrDefaultAsync(i => i.Id == exerciseId);
            if (exerciseRow != null)
            {
                exerciseRow.IsActive = !exerciseRow.IsActive;
                await _exerciseRepository.UpdateAsync(exerciseRow);
                return true;
            }

            throw new UIException("Kategori bulunamadi");
        }

        /// <summary>
        /// Return ExerciseInfo Queryable
        /// </summary>
        private IQueryable<ExerciseInfo> GetQueryableExercise(Expression<Func<Exercise, bool>> expression = null, PageInfo pageInfo = null)
        {
            var query = _exerciseRepository.GetQueryable().AsQueryable();
            if (expression != null)
            {
                query = query.Where(expression);
            }

            var queryableCategory = query
                .OrderByDescending(i => i.IsActive)
                .ThenByDescending(i => i.CreatedDate)
                .GroupJoin(_documentRepository.GetQueryable(), i => i.DocumentGuid, j => j.Guid, (i, j) => new
                {
                    Exercise = i,
                    Document = j
                })
                .SelectMany(i => i.Document.DefaultIfEmpty(), (i, j) => new ExerciseInfo
                {
                    Exercise = i.Exercise,
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
