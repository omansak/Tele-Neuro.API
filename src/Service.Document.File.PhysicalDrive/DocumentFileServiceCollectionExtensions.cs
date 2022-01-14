using Microsoft.Extensions.DependencyInjection;
using System;

namespace Service.Document.File.PhysicalDrive
{
    public static class DocumentFileServiceCollectionExtensions
    {
        public static DocumentFileServiceBuilder AddDocumentFileService(this IServiceCollection services, DocumentFileServiceOptions options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var builder = new DocumentFileServiceBuilder(services, options);
            builder.AddDocumentFile();
            return builder;
        }
    }
}