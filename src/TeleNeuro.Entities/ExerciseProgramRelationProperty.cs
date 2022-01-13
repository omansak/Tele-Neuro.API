using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeleNeuro.Entities
{
    [Table("EXERCISE_PROGRAM_RELATION_PROPERTY")]
    public class ExerciseProgramRelationProperty
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("EXERCISE_RELATION_ID")]
        public int ExerciseRelationId { get; set; }
        [Column("EXERCISE_PROPERTY_ID")]
        public int ExercisePropertyId { get; set; }
        [Column("VALUE")]
        public string Value { get; set; }
        [Column("CREATED_DATE")]
        public DateTime CreatedDate { get; set; }
        [Column("CREATED_USER")]
        public int? CreatedUser { get; set; }
    }
}
