namespace TeleNeuro.Service.BrochureService.Models
{
    public class AssignBrochureUserModel
    {
        public int BrochureId { get; set; }
        public int UserId { get; set; }
        public int AssignedUserId { get; set; }
    }
}
