using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeleNeuro.Entities
{
    [Table("USER_BROCHURE_RELATION")]
    public class UserBrochureRelation
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("BROCHURE_ID")]
        public int BrochureId { get; set; }
        [Column("USER_ID")]
        public int UserId { get; set; }
        [Column("CREATED_USER")]
        public int CreatedUser { get; set; }
        [Column("CREATED_DATE")]
        public DateTime CreatedDate { get; set; }
    }
}
