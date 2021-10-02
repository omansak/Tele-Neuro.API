using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeleNeuro.Entities
{
    [Table("EXERCISE_PROGRAM_RELATION")]
    public class ExerciseProgramRelation
    {
        [Column("ID")]
        public int Id { get; set; }
        [Column("PROGRAM_ID")]
        public int ProgramId { get; set; }
        [Column("EXERCISE_ID")]
        public int ExerciseId { get; set; }
        [Column("SEQUENCE")]
        public int Sequence { get; set; }
        [Column("AUTO_SKIP")]
        public bool AutoSkip { get; set; }
        [Column("AUTO_SKIP_TIME")]
        public int? AutoSkipTime { get; set; }
        [Column("CREATED_DATE")]
        public DateTime CreatedDate { get; set; }
        [Column("CREATED_USER")]
        public int? CreatedUser { get; set; }
    }
}
