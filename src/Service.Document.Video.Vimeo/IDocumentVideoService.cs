using System;
using System.IO;
using System.Threading.Tasks;

namespace Service.Document.Video.Vimeo
{
    public interface IDocumentVideoService
    {
        Task<Video> UploadAsync(string filePath);
        Task<Video> UploadAsync(byte[] data, string contentType);
        Task<Video> UploadAsync(Stream stream, string contentType);
        Task<Video> UploadAsync(Uri uri);
    }
}
