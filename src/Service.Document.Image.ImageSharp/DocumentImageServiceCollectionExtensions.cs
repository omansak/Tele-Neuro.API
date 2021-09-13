using Microsoft.Extensions.DependencyInjection;
using System;

namespace Service.Document.Image.ImageSharp
{
    public static class DocumentImageServiceCollectionExtensions
    {
        public static DocumentImageServiceBuilder AddDocumentImageService(this IServiceCollection services, DocumentImageServiceOptions options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var builder = new DocumentImageServiceBuilder(services, options);
            builder.AddDocumentImageSharp();
            builder.AddImageSharpCacheBuilder();
            return builder;
        }
        public static DocumentImageServiceBuilder AddDocumentImageServiceWithoutCacheBuilder(this IServiceCollection services, DocumentImageServiceOptions options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var builder = new DocumentImageServiceBuilder(services, options);
            builder.AddDocumentImageSharp();
            return builder;
        }
    }
}
