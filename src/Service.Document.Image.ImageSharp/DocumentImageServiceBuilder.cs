using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Service.Document.Model;
using SixLabors.ImageSharp.Web;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Middleware;
using SixLabors.ImageSharp.Web.Processors;
using SixLabors.ImageSharp.Web.Providers;

namespace Service.Document.Image.ImageSharp
{
    public class DocumentImageServiceOptions
    {
        public string BaseFolder { get; set; } = "";
        public string Directory { get; set; } = "image";
        public string CacheFolder { get; set; } = "image-cache";
        public ImageFormat SaveFormat { get; set; } = ImageFormat.Default;
        public int Quality { get; set; } = 100;
        public Action<DocumentResult> CompletedAction { get; set; } = null;
    }
    public class DocumentImageServiceBuilder
    {
        public IServiceCollection Services { get; }
        public DocumentImageServiceOptions Options { get; private set; }
        public DocumentImageServiceBuilder(IServiceCollection services, DocumentImageServiceOptions options = null)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Options = options ?? new DocumentImageServiceOptions();
        }

        public IServiceCollection AddDocumentImageSharp()
        {
            Services.AddScoped<IDocumentImageService>(i => new DocumentImageService(Options.BaseFolder, Options.Directory, Options.SaveFormat, Options.Quality, Options.CompletedAction));
            return Services;
        }

        public IImageSharpBuilder AddImageSharpCacheBuilder()
        {
            var service = Services.AddImageSharp()
                .SetRequestParser<QueryCollectionRequestParser>()
                .Configure<PhysicalFileSystemCacheOptions>(options =>
                {
                    options.CacheFolder = Options.CacheFolder;
                })
                .SetCache(provider => new PhysicalFileSystemCache(
                    provider.GetRequiredService<IOptions<PhysicalFileSystemCacheOptions>>(),
                    provider.GetRequiredService<IWebHostEnvironment>(),
                    provider.GetRequiredService<IOptions<ImageSharpMiddlewareOptions>>(),
                    provider.GetRequiredService<FormatUtilities>()))
                .SetCacheHash<CacheHash>()
                .AddProvider<PhysicalFileSystemProvider>()
                .AddProcessor<ResizeWebProcessor>()
                .AddProcessor<FormatWebProcessor>()
                .AddProcessor<BackgroundColorWebProcessor>()
                .AddProcessor<JpegQualityWebProcessor>();
            return service;
        }
        public DocumentImageServiceBuilder Configure<TDep>(Action<DocumentImageServiceOptions, TDep> options) where TDep : class
        {
            Services
                .Replace(
                    ServiceDescriptor
                        .Scoped<IDocumentImageService>(
                            (i) =>
                            {
                                options?.Invoke(Options, i.GetRequiredService<TDep>());
                                return new DocumentImageService(Options.BaseFolder, Options.Directory,
                                    Options.SaveFormat,
                                    Options.Quality, Options.CompletedAction);
                            }));
            return this;
        }
    }
}
