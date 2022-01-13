namespace TeleNeuro.Service.MessagingService.Models
{
    public class CreateConversationModel
    {
        public int UserId { get; set; }
        public int[] Participants { get; init; }
        public string GroupName { get; init; }
    }
}
