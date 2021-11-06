using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace TeleNeuro.Entities
{
    [Table("ROLE")]
    public class Role
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("KEY")]
        public string Key { get; set; }
        [Column("NAME")]
        public string Name { get; set; }
        [Column("DESCRIPTION")]
        public string Description { get; set; }
        [Column("PRIORITY")]
        public int Priority { get; set; }
    }

}
