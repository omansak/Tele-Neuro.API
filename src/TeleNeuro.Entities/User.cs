using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace TeleNeuro.Entities
{
    [Table("USER")]
    public class User
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("EMAIL")]
        public string Email { get; set; }
        [JsonIgnore]
        [Column("PASSWORD")]
        public string Password { get; set; }
        [JsonIgnore]
        [Column("CREATED_DATE")]
        public DateTime CreatedDate { get; set; }
        [JsonIgnore]
        [Column("CREATED_USER")]
        public int? CreatedUser { get; set; }
        [Column("LAST_LOGIN")]
        public DateTime LastLogin { get; set; }
    }
}
