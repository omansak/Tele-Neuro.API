using System;

namespace TeleNeuro.Service.MessagingService.Models
{
    public class ConversationMessageModel
    {
        public int ConversationId { get; set; }
        public int UserId { get; set; }
        public DateTime? Cursor { get; set; }
    }
}
