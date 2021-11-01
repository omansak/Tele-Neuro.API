using System.Collections.Generic;

namespace TeleNeuro.Service.ProgramService.Models
{
    public class AssignExerciseModel
    {
        public int ProgramId { get; init; }
        public int ExerciseId { get; init; }
        public bool AutoSkip { get; init; }
        public int AutoSkipTime { get; init; }
        public int UserId { get; set; }
        public List<AssignExercisePropertyModel> Properties { get; init; }
    }

    public class AssignExercisePropertyModel
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
