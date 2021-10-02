using PlayCore.Core.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeleNeuro.Entities;
using TeleNeuro.Entity.Context;

namespace TeleNeuro.Service.UtilityService
{
    public class UtilityService : IUtilityService
    {
        private readonly IBaseRepository<TeleNeuroDatabaseContext> _baseRepository;

        public UtilityService(IBaseRepository<TeleNeuroDatabaseContext> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<List<ExercisePropertyDefinition>> ListExercisePropertyDefinitions()
        {
            return await _baseRepository.ListAllAsync<ExercisePropertyDefinition>();
        }
    }
}
