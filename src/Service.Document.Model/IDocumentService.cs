using System;
using System.IO;
using System.Threading.Tasks;

namespace Service.Document.Model
{
    public interface IDocumentService
    {
        Task<DocumentResult> SaveAsync(string filePath, string fileName = null, Action<DocumentResult> completed = null);
        Task<DocumentResult> SaveAsync(byte[] data, string fileName = null, Action<DocumentResult> completed = null);
        Task<DocumentResult> SaveAsync(Stream stream, string fileName = null, Action<DocumentResult> completed = null);
        Task<DocumentResult> SaveAsync(Uri uri, string fileName = null, Action<DocumentResult> completed = null);
    }
}
