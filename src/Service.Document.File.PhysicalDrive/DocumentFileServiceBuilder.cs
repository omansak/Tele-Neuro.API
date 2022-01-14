using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Service.Document.Model;

namespace Service.Document.File.PhysicalDrive
{
    public class DocumentFileServiceOptions
    {
        public string BaseFolder { get; set; } = "";
        public string Directory { get; set; } = "file";
        public Action<DocumentResult> CompletedAction { get; set; } = null;
    }
    public class DocumentFileServiceBuilder
    {
        public IServiceCollection Services { get; }
        public DocumentFileServiceOptions Options { get; private set; }
        public DocumentFileServiceBuilder(IServiceCollection services, DocumentFileServiceOptions options = null)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Options = options ?? new DocumentFileServiceOptions();
        }

        public IServiceCollection AddDocumentFile()
        {
            Services.AddScoped<IDocumentFileService>(i => new DocumentFileService(Options.BaseFolder, Options.Directory, Options.CompletedAction));
            return Services;
        }

        public DocumentFileServiceBuilder Configure<TDep>(Action<DocumentFileServiceOptions, TDep> options) where TDep : class
        {
            Services
                .Replace(
                    ServiceDescriptor
                        .Scoped<IDocumentFileService>(
                            (i) =>
                            {
                                options?.Invoke(Options, i.GetRequiredService<TDep>());
                                return new DocumentFileService(Options.BaseFolder, Options.Directory, Options.CompletedAction);
                            }));
            return this;
        }
    }
}
