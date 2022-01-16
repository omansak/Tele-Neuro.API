using System;
using System.Collections.Generic;
using TeleNeuro.Entities;

namespace TeleNeuro.Service.MessagingService.Models
{
    public class ConversationMessage
    {
        public int ConversationId { get; set; }
        public int UserId { get; set; }
        public int MessageId { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
        public IEnumerable<MessageRead> MessageReads { get; set; }
    }
}
