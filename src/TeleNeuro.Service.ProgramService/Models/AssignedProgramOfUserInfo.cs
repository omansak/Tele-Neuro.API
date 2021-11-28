using System;

namespace TeleNeuro.Service.ProgramService.Models
{
    public class AssignedProgramOfUserInfo
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public int ExerciseCount { get; set; }
        public int CompletedExercisesCount { get; set; }
        public string CategoryName { get; set; }
        public DateTime AssignDate { get; set; }
    }
}
