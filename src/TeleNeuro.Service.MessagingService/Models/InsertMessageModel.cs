namespace TeleNeuro.Service.MessagingService.Models
{
    public class InsertMessageModel
    {
        public int UserId { get; set; }
        public int ConversationId { get; set; }
        public string Message { get; set; }
    }
}
