using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Service.Document.Model;

namespace Service.Document.DocumentServiceSelector
{
    public interface IDocumentServiceSelector
    {
        IDocumentService GetService(IFormFile file, IEnumerable<DocumentType> availableDocumentTypes = null);
    }
}
