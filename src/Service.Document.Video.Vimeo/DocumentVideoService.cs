using Service.Document.Model;
using System;
using System.IO;
using System.Threading.Tasks;
using VimeoDotNet;
using VimeoDotNet.Enums;
using VimeoDotNet.Exceptions;
using VimeoDotNet.Models;
using VimeoDotNet.Net;

namespace Service.Document.Video.Vimeo
{
    public class DocumentVideoService : IDocumentVideoService
    {
        public Action<DocumentResult> CompletedAction { get; private set; }
        private readonly VimeoClient _client;

        public DocumentVideoService(string token, Action<DocumentResult> completedAction = null)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));
            CompletedAction = completedAction;
            _client ??= new VimeoClient(token);
        }
        public DocumentVideoService SetCompletedAction(Action<DocumentResult> completedAction)
        {
            CompletedAction = completedAction;
            return this;
        }
        public async Task<DocumentResult> SaveAsync(string filePath, string fileName = null, string mimeType = null, Action<DocumentResult> completed = null)
        {
            return await UploadAsync(new BinaryContent(filePath), fileName, completed);
        }

        public async Task<DocumentResult> SaveAsync(byte[] data, string fileName = null, string mimeType = null, Action<DocumentResult> completed = null)
        {
            return await UploadAsync(new BinaryContent(data, mimeType), fileName, completed);
        }

        public async Task<DocumentResult> SaveAsync(Stream stream, string fileName = null, string mimeType = null, Action<DocumentResult> completed = null)
        {
            return await UploadAsync(new BinaryContent(stream, mimeType), fileName, completed);
        }

        public async Task<DocumentResult> SaveAsync(Uri uri, string fileName = null, string mimeType = null, Action<DocumentResult> completed = null)
        {
            using (var wc = new System.Net.WebClient())
            {
                return await UploadAsync(new BinaryContent(wc.DownloadData(uri.AbsoluteUri), mimeType), fileName, completed);
            }
        }

        private async Task<DocumentResult> UploadAsync(BinaryContent binaryContent, string fileName = null, Action<DocumentResult> completed = null)
        {
            using (var file = binaryContent)
            {
                file.OriginalFileName = fileName;
                var result = await _client.UploadEntireFileAsync(file);
                if (result.IsVerifiedComplete && result.ClipId.HasValue)
                {
                    var video = await _client.GetVideoAsync(result.ClipId.Value);
                    await _client.UpdateVideoMetadataAsync(result.ClipId.Value, new VideoUpdateMetadata
                    {
                        EmbedPrivacy = VideoEmbedPrivacyEnum.Public,
                        AllowDownloadVideo = false,
                        Name = fileName,
                        Privacy = VideoPrivacyEnum.Disable
                    });
                    var documentResult = new DocumentResult
                    {
                        Guid = result.ClipId.Value.ToString(),
                        Name = $"{video.Name}.{MimeTypeAssistant.GetExtension(result.File.ContentType)}",
                        FileName = fileName ?? $"{video.Name}.{MimeTypeAssistant.GetExtension(result.File.ContentType)}",
                        Extension = MimeTypeAssistant.GetExtension(result.File.ContentType),
                        ContentType = result.File.ContentType,
                        Type = DocumentType.Video,
                        DocumentPath = new DocumentPath
                        {
                            Base = "https://player.vimeo.com/",
                            Directory = "video",
                            Path = $"video/{result.ClipId.Value}",
                            FullPath = $"https://player.vimeo.com/video/{result.ClipId.Value}"
                        },
                        CreatedDate = DateTime.Now
                    };
                    if (completed != null)
                    {
                        completed(documentResult);
                    }

                    if (CompletedAction != null)
                    {
                        CompletedAction(documentResult);
                    }

                    return documentResult;
                }
                throw new VimeoApiException("Video did not upload");
            }
        }

    }
}