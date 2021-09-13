using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace TeleNeuro.Entities
{
    [Table("DOCUMENT")]
    public class Document
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("GUID")]
        public string Guid { get; set; }
        [Column("NAME")]
        public string Name { get; set; }
        [Column("FILE_NAME")]
        public string FileName { get; set; }
        [Column("EXTENSION")]
        public string Extension { get; set; }
        [Column("CONTENT_TYPE")]
        public string ContentType { get; set; }
        [Column("DIRECTORY")]
        public string Directory { get; set; }
        [Column("PATH")]
        public string Path { get; set; }
        [Column("PHYSICAL_BASE")]
        public string PhysicalBase { get; set; }
        [Column("PHYSICAL_FULL_PATH")]
        public string PhysicalFullPath { get; set; }
        [Column("HOST_BASE")]
        public string HostBase { get; set; }
        [Column("HOST_FULL_PATH")]
        public string HostFullPath { get; set; }
        [Column("Type")]
        public int Type { get; set; }
        [Column("CREATED_DATE")]
        public DateTime? CreatedDate { get; set; }
        [Column("IS_ACTIVE")]
        public bool IsActive { get; set; }
    }
}
