using System;
using System.Collections.Generic;

namespace TeleNeuro.Service.MessagingService.Models
{
    public class ConversationSummary
    {
        public int ConversationId { get; set; }
        public bool IsGroup { get; set; }
        public string Name { get; set; }
        public string LastMessage { get; set; }
        public DateTime? LastMessageDate { get; set; }
        public bool HasUnread { get; set; }
        public IEnumerable<ParticipantUserInfo> Participants { get; set; }
    }
}
