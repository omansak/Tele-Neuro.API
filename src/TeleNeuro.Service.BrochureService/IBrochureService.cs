using System.Collections.Generic;
using System.Threading.Tasks;
using PlayCore.Core.Model;
using TeleNeuro.Entities;
using TeleNeuro.Service.BrochureService.Models;

namespace TeleNeuro.Service.BrochureService
{
    public interface IBrochureService
    {
        /// <summary>
        /// Returns Exercises
        /// </summary>
        /// <returns></returns>
        Task<(List<BrochureInfo> Result, int Count)> ListExercises(PageInfo pageInfo = null);

        /// <summary>
        /// Return Exercise
        /// </summary>
        Task<BrochureInfo> GetBrochure(int id);

        /// <summary>
        /// Insert or update Brochure (CreatedDate can not modify)
        /// </summary>
        Task<BrochureInfo> UpdateBrochure(Brochure brochure);

        /// <summary>
        /// Toggle Brochure IsActive Status
        /// </summary>
        Task<bool> ToggleExerciseStatus(int exerciseId);

        /// <summary>
        /// Assign user to Broşür
        /// </summary>
        Task<int> AssignUser(AssignBrochureUserModel model);

        /// <summary>
        /// Delete a relation of user of program relation
        /// </summary>
        Task<bool> DeleteAssignedUser(AssignBrochureUserModel model);

        /// <summary>
        /// List assigned users of program
        /// </summary>
        Task<(List<AssignedBrochureUserInfo>, int)> ListAssignedUsers(AssignedBrochureUserModel model);

        /// <summary>
        /// List assigned programs of users
        /// </summary>
        Task<(List<AssignedBrochureOfUserInfo>, int)> ListAssignedBrochures(AssignedBrochureOfUserModel model);
    }
}