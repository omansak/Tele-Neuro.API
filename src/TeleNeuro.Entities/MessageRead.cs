using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeleNeuro.Entities
{
    [Table("MESSAGE_READ")]
    public class MessageRead
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("CONVERSATION_ID")]
        public int ConversationId { get; set; }
        [Column("MESSAGE_ID")]
        public int MessageId { get; set; }
        [Column("USER_ID")]
        public int UserId { get; set; }
        [Column("IS_READ")]
        public bool IsRead { get; set; }
        [Column("CREATE_DATE")]
        public DateTime CreateDate { get; set; }
        [Column("UPDATE_DATE")]
        public DateTime UpdateDate { get; set; }
    }

}
