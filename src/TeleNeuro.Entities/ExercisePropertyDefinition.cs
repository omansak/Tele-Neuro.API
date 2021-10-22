using System.ComponentModel.DataAnnotations.Schema;

namespace TeleNeuro.Entities
{
    [Table("EXERCISE_PROPERTY_DEFINITION")]
    public class ExercisePropertyDefinition
    {
        [Column("ID")]
        public int Id { get; set; }
        [Column("NAME")]
        public string Name { get; set; }
        [Column("DESCRIPTION")]
        public string Description { get; set; }
        [Column("UNIT_NAME")]
        public string UnitName { get; set; }
        [Column("KEY")]
        public string Key { get; set; }
        [Column("IS_NUMBER")]
        public bool IsNumber { get; set; }
    }
}
