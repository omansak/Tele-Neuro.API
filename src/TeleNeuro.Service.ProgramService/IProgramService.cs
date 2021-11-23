using System.Collections.Generic;
using System.Threading.Tasks;
using PlayCore.Core.Model;
using TeleNeuro.Entities;
using TeleNeuro.Service.ProgramService.Models;

namespace TeleNeuro.Service.ProgramService
{
    public interface IProgramService
    {
        /// <summary>
        /// Returns ProgramInfo
        /// </summary>
        /// <param name="pageInfo"></param>
        /// <returns></returns>
        Task<List<ProgramInfo>> ListPrograms(PageInfo pageInfo = null);

        /// <summary>
        /// Returns ProgramInfo counts
        /// </summary>
        /// <returns></returns>
        Task<int> CountPrograms();

        /// <summary>
        /// Returns ProgramInfo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ProgramInfo> GetProgram(int id);

        /// <summary>
        /// Insert or update Program (CreatedDate can not modify)
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        Task<ProgramInfo> UpdateProgram(Program program);

        /// <summary>
        /// Toggle Program IsActive Status
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        Task<bool> ToggleProgramStatus(int programId);

        /// <summary>
        /// Assign Exercise to Program
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Assign's Id</returns>
        Task<int> AssignExercise(AssignExerciseModel model);

        /// <summary>
        /// Assign user to program
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> AssignUser(AssignUserModel model);

        /// <summary>
        /// Delete a relation of user of program relation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> DeleteAssignedUser(AssignUserModel model);

        /// <summary>
        /// List assigned users of program
        /// </summary>
        Task<(List<AssignedProgramUserInfo>, int)> ListAssignedUsers(AssignedProgramUserModel model);

        /// <summary>
        /// List assigned programs of users
        /// </summary>
        Task<(List<AssignedProgramOfUserInfo>, int)> ListAssignedPrograms(AssignedProgramOfUserModel model);

        /// <summary>
        /// Return Exercises of Program
        /// </summary>
        Task<List<ProgramAssignedExerciseInfo>> AssignedExercises(int programId, bool? isActiveExercise = null);

        /// <summary>
        /// Change Sequence Assigned Exercise
        /// </summary>
        /// <param name="relationId"></param>
        /// <param name="direction">-1 : Up, 0 : No Change, 1 : Down</param>
        /// <returns></returns>
        Task<bool> ChangeSequenceAssignedExercise(int relationId, int direction);

        /// <summary>
        /// Delete a assigned exercise of program
        /// </summary>
        Task<bool> DeleteAssignedExercise(int relationId);
    }
}