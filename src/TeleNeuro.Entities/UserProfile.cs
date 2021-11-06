using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace TeleNeuro.Entities
{
    [Table("USER_PROFILE")]
    public class UserProfile
    {
        [Key]
        [JsonIgnore]
        [Column("ID")]
        public int Id { get; set; }
        [Column("USER_ID")]
        public int UserId { get; set; }
        [Column("NAME")]
        public string Name { get; set; }
        [Column("SURNAME")]
        public string Surname { get; set; }
        [Column("BIRTH_DATE")]
        public DateTime? BirthDate { get; set; }
    }

}
