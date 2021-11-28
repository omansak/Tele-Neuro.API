using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeleNeuro.Entities
{
    [Table("RELATION_STAT_LOG")]
    public class RelationStatLog
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("PROGRAM_ID")]
        public int? ProgramId { get; set; }
        [Column("EXERCISE_ID")]
        public int? ExerciseId { get; set; }
        [Column("USER_ID")]
        public int? UserId { get; set; }
        [Column("ACTION_KEY")]
        public string ActionKey { get; set; }
        [Column("ACTION_ARGUMENT")]
        public string ActionArgument { get; set; }
        [Column("USER_PROGRAM_RELATION_ID")]
        public int? UserProgramRelationId { get; set; }
        [Column("EXERCISE_PROGRAM_RELATION_ID")]
        public int? ExerciseProgramRelationId { get; set; }
        [Column("CREATED_TIME")]
        public DateTime CreatedTime { get; set; }
    }
}
