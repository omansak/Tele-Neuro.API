using System.Collections.Generic;
using System.Threading.Tasks;
using PlayCore.Core.Model;
using TeleNeuro.Entities;
using TeleNeuro.Service.ExerciseService.Models;

namespace TeleNeuro.Service.ExerciseService
{
    public interface IExerciseService
    {
        Task<List<ExerciseInfo>> SearchExercises(string term);
        Task<List<ExerciseInfo>> ListExercises(PageInfo pageInfo = null);
        Task<int> CountExercises();
        Task<ExerciseInfo> GetExercise(int id);
        Task<ExerciseInfo> UpdateExercise(Exercise exercise);
        Task<bool> ToggleExerciseStatus(int categoryId);

    }
}
