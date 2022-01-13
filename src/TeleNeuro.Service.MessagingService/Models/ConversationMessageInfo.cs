using System;
using System.Collections.Generic;

namespace TeleNeuro.Service.MessagingService.Models
{
    public class ConversationMessageInfo
    {
        public List<ConversationMessage> ConversationMessage { get; set; }
        public DateTime? Cursor { get; set; }
    }
}
