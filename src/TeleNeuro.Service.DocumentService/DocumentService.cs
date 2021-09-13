using System.Threading.Tasks;
using PlayCore.Core.CustomException;
using PlayCore.Core.Repository;
using TeleNeuro.Entities;
using TeleNeuro.Entity.Context;

namespace TeleNeuro.Service.DocumentService
{
    public class DocumentService : IDocumentService
    {
        private readonly IBaseRepository<Document, TeleNeuroDatabaseContext> _documentRepository;

        public DocumentService(IBaseRepository<Document, TeleNeuroDatabaseContext> documentRepository)
        {
            _documentRepository = documentRepository;
        }
        public async Task<int> InsertDocument(Document document)
        {
            if (document.Id > 0)
                throw new UIException("Doküman güncelleme işlemi yapılamaz.");
            var result = await _documentRepository.InsertAsync(document);
            return result.Id;
        }
    }
}
