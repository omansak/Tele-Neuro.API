using System.Collections.Generic;
using System.Threading.Tasks;
using TeleNeuro.Entities;
using TeleNeuro.Service.UtilityService.Models;

namespace TeleNeuro.Service.UtilityService
{
    public interface IUtilityService
    {
        /// <summary>
        /// List Property Definitions of Exercixe
        /// </summary>
        Task<List<ExercisePropertyDefinition>> ListExercisePropertyDefinitions();

        /// <summary>
        /// Insert stat log
        /// </summary>
        /// <param name="model"></param>
        Task InsertRelationStatLog(RelationStatLogModel model);

        /// <summary>
        /// Return Completed Exercise Ids  Of Program
        /// </summary>
        Task<List<int?>> CompletedExercisesOfProgram(int[] programIds, int userId);

        /// <summary>
        /// Return user's stats
        /// </summary>
        Task<UserWorkProcessStats> UserStats(int userId);
    }
}