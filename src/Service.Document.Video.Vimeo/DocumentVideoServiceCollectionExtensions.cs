using Microsoft.Extensions.DependencyInjection;
using System;

namespace Service.Document.Video.Vimeo
{
    public static class DocumentVideoServiceCollectionExtensions
    {
        public static DocumentVideoServiceBuilder AddDocumentVideoService(this IServiceCollection services, DocumentVideoServiceOptions options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var builder = new DocumentVideoServiceBuilder(services, options);
            builder.AddDocumentVideo();
            return builder;
        }
    }
}
