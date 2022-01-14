using Microsoft.AspNetCore.Http;

namespace TeleNeuro.API.Models
{
    public class BrochureModel
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public bool IsActive { get; init; }
        public IFormFile File { get; init; }
    }
}
