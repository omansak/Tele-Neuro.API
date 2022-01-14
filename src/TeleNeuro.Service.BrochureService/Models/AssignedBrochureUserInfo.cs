namespace TeleNeuro.Service.BrochureService.Models
{
    public class AssignedBrochureUserInfo
    {
        public int RelationId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
    }
}
