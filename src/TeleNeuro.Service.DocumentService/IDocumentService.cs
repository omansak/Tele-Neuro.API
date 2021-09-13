using System.Threading.Tasks;
using TeleNeuro.Entities;

namespace TeleNeuro.Service.DocumentService
{
    public interface IDocumentService
    {
        Task<int> InsertDocument(Document document);
    }
}
