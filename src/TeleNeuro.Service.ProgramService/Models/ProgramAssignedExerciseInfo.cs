using System.Collections.Generic;
using TeleNeuro.Entities;

namespace TeleNeuro.Service.ProgramService.Models
{
    public class ProgramAssignedExerciseInfo
    {
        public int RelationId { get; set; }
        public int ProgramId { get; set; }
        public int Sequence { get; set; }
        public bool AutoSkip { get; set; }
        public int? AutoSkipTime { get; set; }
        public Exercise Exercise { get; set; }
        public Document ExerciseDocument { get; set; }
        public List<ExerciseProperty> Properties { get; set; }
    }

    public class ExerciseProperty
    {
        public string Value { get; set; }
        public ExercisePropertyDefinition Definition { get; set; }
    }
}
