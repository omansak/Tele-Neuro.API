using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Service.Document.Model;
using System;

namespace Service.Document.Video.Vimeo
{
    public class DocumentVideoServiceOptions
    {
        public string Token { get; set; }
        public Action<DocumentResult> CompletedAction { get; set; } = null;
    }
    public class DocumentVideoServiceBuilder
    {
        public IServiceCollection Services { get; }
        public DocumentVideoServiceOptions Options { get; private set; }
        public DocumentVideoServiceBuilder(IServiceCollection services, DocumentVideoServiceOptions options = null)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Options = options ?? new DocumentVideoServiceOptions();
        }

        public IServiceCollection AddDocumentVideo()
        {
            Services.AddScoped<IDocumentVideoService>(i => new DocumentVideoService(Options.Token, Options.CompletedAction));
            return Services;
        }
        public DocumentVideoServiceBuilder Configure<TDep>(Action<DocumentVideoServiceOptions, TDep> options) where TDep : class
        {
            Services
                .Replace(
                    ServiceDescriptor
                        .Scoped<IDocumentVideoService>(
                            (i) =>
                            {
                                options?.Invoke(Options, i.GetRequiredService<TDep>());
                                return new DocumentVideoService(Options.Token, Options.CompletedAction);
                            }));
            return this;
        }
    }
}
