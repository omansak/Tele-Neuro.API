using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeleNeuro.Entities
{
    [Table("CONVERSATION_PARTICIPANT")]
    public class ConversationParticipant
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("CONVERSATION_ID")]
        public int ConversationId { get; set; }
        [Column("USER_ID")]
        public int UserId { get; set; }
    }
}
