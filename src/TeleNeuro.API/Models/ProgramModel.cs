namespace TeleNeuro.API.Models
{
    public class ProgramModel
    {
        public int Id { get; init; }
        public int CategoryId { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public bool IsActive { get; init; }
        public bool IsPublic { get; init; }
    }
}
