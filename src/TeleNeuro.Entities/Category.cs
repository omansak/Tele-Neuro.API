using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeleNeuro.Entities
{
    [Table("CATEGORY")]
    public class Category
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("NAME")]
        public string Name { get; set; }
        [Column("DESCRIPTION")]
        public string Description { get; set; }
        [Column("IS_ACTIVE")]
        public bool IsActive { get; set; } = true;
        [Column("CREATED_DATE")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [Column("CREATED_USER")]
        public string CreatedUser { get; set; }
        [Column("DOCUMENT_GUID")]
        public string DocumentGuid { get; set; }
        [NotMapped]
        public Document Document { get; set; }
    }
}
