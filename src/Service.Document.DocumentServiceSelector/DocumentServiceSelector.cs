using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Service.Document.Image.ImageSharp;
using Service.Document.Model;
using Service.Document.Video.Vimeo;

namespace Service.Document.DocumentServiceSelector
{
    public class DocumentServiceSelector : IDocumentServiceSelector
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<DocumentType, Type> _documentServices = new()
        {
            { DocumentType.Image, typeof(IDocumentImageService) },
            { DocumentType.Video, typeof(IDocumentVideoService) },
        };

        public DocumentServiceSelector(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDocumentService GetService(IFormFile file, IEnumerable<DocumentType> availableDocumentTypes = null)
        {
            var type = file.ContentType?.Split("/")?[0]?.ToLower();
            DocumentType documentType = type switch
            {
                "image" => DocumentType.Image,
                "video" => DocumentType.Video,
                _ => throw new ArgumentOutOfRangeException()
            };
            if (availableDocumentTypes != null && availableDocumentTypes.All(i => i != documentType))
            {
                throw new FormatException("File format is not available");
            }
            return _serviceProvider.GetRequiredService(_documentServices.GetValueOrDefault(documentType)!) as IDocumentService; ;
        }
    }
}
