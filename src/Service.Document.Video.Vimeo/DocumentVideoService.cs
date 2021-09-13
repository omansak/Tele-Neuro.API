using PlayCore.Core.CustomException;
using PlayCore.Core.Extension;
using PlayCore.Core.Logger;
using System;
using System.IO;
using System.Threading.Tasks;
using VimeoDotNet;
using VimeoDotNet.Net;

namespace Service.Document.Video.Vimeo
{
    public class DocumentVideoService : IDocumentVideoService
    {
        private readonly VimeoClient _client;
        private readonly IBasicLogger<DocumentVideoService> _basicLogger;

        public DocumentVideoService(string token, IBasicLogger<DocumentVideoService> basicLogger)
        {
            _basicLogger = basicLogger;
            _client ??= new VimeoClient(token);
        }
        public async Task<Video> UploadAsync(string filePath)
        {
            return await UploadAsync(new BinaryContent(filePath));
        }
        public async Task<Video> UploadAsync(byte[] data, string contentType)
        {
            return await UploadAsync(new BinaryContent(data, contentType));
        }
        public async Task<Video> UploadAsync(Stream stream, string contentType)
        {
            return await UploadAsync(new BinaryContent(stream, contentType));
        }
        public async Task<Video> UploadAsync(Uri uri)
        {
            try
            {
                var result = await _client.UploadPullLinkAsync(uri.AbsoluteUri);
                if (result?.Id.HasValue == true)
                    return new Video
                    {
                        Id = result.Id.Value,
                        Name = result.Name,
                        Url = result.Uri
                    };
            }
            catch (Exception e)
            {
                _basicLogger.Log(e.ToJson());
            }

            throw new UIException("Dosya Yüklenemedi");
        }
        private async Task<Video> UploadAsync(BinaryContent binaryContent)
        {
            try
            {
                using (var file = binaryContent)
                {
                    var result = await _client.UploadEntireFileAsync(file);
                    if (result.IsVerifiedComplete && result.ClipId.HasValue)
                        return new Video
                        {
                            Id = result.ClipId.Value,
                            Name = result.Ticket.Video.Name,
                            Url = result.ClipUri
                        };
                }
            }
            catch (Exception e)
            {
                _basicLogger.Log(e.ToJson());
            }

            throw new UIException("Dosya Yüklenemedi");
        }
    }
}