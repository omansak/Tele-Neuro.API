using TeleNeuro.Entities;

namespace TeleNeuro.Service.CategoryService.Models
{
    public class CategoryInfo
    {
        public Category Category { get; set; }
        public Document Document { get; set; }
        public int ProgramCount { get; set; }
    }
}
