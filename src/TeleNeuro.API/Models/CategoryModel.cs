using Microsoft.AspNetCore.Http;

namespace TeleNeuro.API.Models
{
    public class CategoryModel
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public bool IsActive { get; init; }
        public IFormFile Image { get; init; }
    }
}
