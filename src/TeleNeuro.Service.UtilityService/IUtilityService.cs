using System.Collections.Generic;
using System.Threading.Tasks;
using TeleNeuro.Entities;

namespace TeleNeuro.Service.UtilityService
{
    public interface IUtilityService
    {
        Task<List<ExercisePropertyDefinition>> ListExercisePropertyDefinitions();
    }
}