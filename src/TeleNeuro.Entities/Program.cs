using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeleNeuro.Entities
{
    [Table("PROGRAM")]
    public class Program
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("CATEGORY_ID")]
        public int CategoryId { get; set; }
        [Column("NAME")]
        public string Name { get; set; }
        [Column("DESCRIPTION")]
        public string Description { get; set; }
        [Column("IS_ACTIVE")]
        public bool IsActive { get; set; }
        [Column("IS_PUBLIC")]
        public bool IsPublic { get; set; }
        [Column("CREATED_USER")]
        public int? CreatedUser { get; set; }
        [Column("CREATED_DATE")]
        public DateTime CreatedDate { get; set; }
    }

}
