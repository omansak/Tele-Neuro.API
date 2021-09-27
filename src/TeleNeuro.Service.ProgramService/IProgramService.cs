using System.Collections.Generic;
using System.Threading.Tasks;
using PlayCore.Core.Model;
using TeleNeuro.Entities;
using TeleNeuro.Service.ProgramService.Models;

namespace TeleNeuro.Service.ProgramService
{
    public interface IProgramService
    {
        Task<List<ProgramInfo>> ListPrograms(PageInfo pageInfo = null);
        Task<int> CountPrograms();
        Task<ProgramInfo> GetProgram(int id);
        Task<ProgramInfo> UpdateProgram(Program program);
        Task<bool> ToggleProgramStatus(int programId);
        Task<int> AssignExercise(AssignExerciseModel model);
    }
}
