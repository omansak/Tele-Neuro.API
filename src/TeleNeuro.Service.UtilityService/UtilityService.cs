using Microsoft.EntityFrameworkCore;
using PlayCore.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeleNeuro.Entities;
using TeleNeuro.Entity.Context;
using TeleNeuro.Service.UtilityService.Models;

namespace TeleNeuro.Service.UtilityService
{
    public class UtilityService : IUtilityService
    {
        private readonly IBaseRepository<TeleNeuroDatabaseContext> _baseRepository;

        public UtilityService(IBaseRepository<TeleNeuroDatabaseContext> baseRepository)
        {
            _baseRepository = baseRepository;
        }
        /// <summary>
        /// List Property Definitions of Exercixe
        /// </summary>
        public async Task<List<ExercisePropertyDefinition>> ListExercisePropertyDefinitions()
        {
            return await _baseRepository.ListAllAsync<ExercisePropertyDefinition>();
        }
        /// <summary>
        /// Insert stat log
        /// </summary>
        /// <param name="model"></param>
        public async Task InsertRelationStatLog(RelationStatLogModel model)
        {
            _ = await _baseRepository.InsertAsync(new RelationStatLog
            {
                ProgramId = model.ProgramId,
                UserId = model.UserId,
                CreatedTime = DateTime.Now,
                ActionKey = model.ActionKey,
                ActionArgument = model.ActionArgument,
                ExerciseId = model.ExerciseId,
                ExerciseProgramRelationId = model.ExerciseProgramRelationId,
                UserProgramRelationId = model.UserProgramRelationId,
            });
        }
        /// <summary>
        /// Return Completed Exercise Program Relation Ids  Of Program
        /// </summary>
        public Task<List<int?>> CompletedExercisesOfProgram(int[] programIds, int userId)
        {
            return _baseRepository.GetQueryable<RelationStatLog>()
                .Where(i => programIds.Length > 0 && programIds.Contains(i.ProgramId.Value) && i.UserId == userId)
                .Where(i => new[] { "EXERCISE_ENDED", "EXERCISE_OPENED" }.Contains(i.ActionKey))
                .Select(i => i.ExerciseProgramRelationId)
                .Distinct()
                .ToListAsync();
        }
        /// <summary>
        /// Return user's stats
        /// </summary>
        public Task<UserWorkProcessStats> UserStats(int userId)
        {
            int? totalTimeAs5 = _baseRepository
                .GetQueryable<RelationStatLog>()
                .Where(j => j.UserId == userId && j.ActionArgument != null &&
                            new[] { "EXERCISE_NON_VIDEO_TIME_5", "EXERCISE_VIDEO_TIME_5" }.Contains(j.ActionKey))
                .ToList()
                .Sum(j => int.Parse(j.ActionArgument));
            return _baseRepository
                .GetQueryable<User>()
                .Where(i => i.Id == userId)
                .Select(i => new UserWorkProcessStats
                {
                    TotalPrograms = _baseRepository.GetQueryable<UserProgramRelation>().Count(j => j.UserId == i.Id),
                    TotalCompletedExercises = _baseRepository
                        .GetQueryable<RelationStatLog>()
                        .Where(j => j.UserId == i.Id && new[] { "EXERCISE_ENDED", "EXERCISE_OPENED" }.Contains(j.ActionKey))
                        .Select(j => j.ExerciseProgramRelationId)
                        .Distinct()
                        .Count(),
                    TotalTimeAs5 = totalTimeAs5
                })
                .SingleOrDefaultAsync();
        }
    }
}
