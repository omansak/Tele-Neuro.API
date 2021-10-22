using TeleNeuro.Entities;

namespace TeleNeuro.Service.ProgramService.Models
{
    public class ProgramInfo
    {
        public Program Program { get; set; }
        public Category Category { get; set; }
        public Document CategoryDocument { get; set; }
    }
}
