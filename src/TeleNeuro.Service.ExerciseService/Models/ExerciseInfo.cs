using TeleNeuro.Entities;

namespace TeleNeuro.Service.ExerciseService.Models
{
    public class ExerciseInfo
    {
        public Exercise Exercise { get; set; }
        public Document Document { get; set; }
        public int ProgramCount { get; set; }
    }
}
