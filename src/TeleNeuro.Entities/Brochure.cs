using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeleNeuro.Entities
{
    [Table("BROCHURE")]
    public class Brochure
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("IS_ACTIVE")]
        public bool IsActive { get; set; }
        [Column("CREATED_DATE")]
        public DateTime CreatedDate { get; set; }
        [Column("CREATED_USER")]
        public int CreatedUser { get; set; }
        [Column("DOCUMENT_GUID")]
        public string DocumentGuid { get; set; }

    }

}
