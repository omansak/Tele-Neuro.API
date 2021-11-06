using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeleNeuro.Entities
{
    [Table("USER_ROLE_RELATION")]
    public class UserRoleRelation
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("ROLE_ID")]
        public int RoleId { get; set; }
        [Column("USER_ID")]
        public int UserId { get; set; }
        [Column("CREATED_DATE")]
        public DateTime CreatedDate { get; set; }
        [Column("CREATED_USER")]
        public int? CreatedUser { get; set; }
    }

}
