using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeleNeuro.Entities
{
    [Table("MESSAGE")]
    public class Message
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("CONVERSATION_ID")]
        public int ConversationId { get; set; }
        [Column("MESSAGE")]
        public string MessageString { get; set; }
        [Column("USER_ID")]
        public int UserId { get; set; }
        [Column("CREATE_DATE")]
        public DateTime CreateDate { get; set; }
    }
}
