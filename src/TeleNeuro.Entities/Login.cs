using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeleNeuro.Entities
{
    [Table("LOGIN")]
    public class Login
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("EMAIL")]
        public string Email { get; set; }
        [Column("PASSWORD")]
        public string Password { get; set; }
        [Column("CREATED_DATE")]
        public DateTime CreatedDate { get; set; }
        [Column("CREATED_USER")]
        public int? CreatedUser { get; set; }
        [Column("LAST_LOGIN")]
        public DateTime LastLogin { get; set; }
    }
}
