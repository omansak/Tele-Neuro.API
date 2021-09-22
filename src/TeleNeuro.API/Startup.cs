using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PlayCore.Core.Extension;
using Service.Document.Image.ImageSharp;
using SixLabors.ImageSharp.Web.DependencyInjection;
using System;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using PlayCore.Core.HostedService;
using PlayCore.Core.Logger;
using Service.Document.DocumentServiceSelector;
using Service.Document.Video.Vimeo;
using TeleNeuro.Entities;
using TeleNeuro.Entity.Context;
using TeleNeuro.Service.CategoryService;
using TeleNeuro.Service.ExerciseService;
using VimeoDotNet.Exceptions;

namespace TeleNeuro.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            //Configuration = configuration;
            var builder = new ConfigurationBuilder()
                .SetBasePath(webHostEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.Development.json", optional: true, true);

            Configuration = builder.Build();
            WebHostEnvironment = webHostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TeleNeuro.API", Version = "v1" });
            });
            //PlayCore
            services.AddBaseRepository();
            services.AddBaseEntityRepository();
            //Context
            services.AddDbContext<TeleNeuroDatabaseContext>(options => options.UseSqlServer("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=TeleNeuro;"));
            //Dependencies
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IExerciseService, ExerciseService>();
            services.AddScoped<Service.DocumentService.IDocumentService, Service.DocumentService.DocumentService>();
            services
                .AddDocumentImageService(new DocumentImageServiceOptions
                {
                    BaseFolder = WebHostEnvironment.WebRootPath,
                    Directory = Configuration["Path:ImageUploadDirectory"],
                    CacheFolder = Configuration["Path:ImageUploadedCacheDirectory"]
                })
                .Configure<Service.DocumentService.IDocumentService>((i, j) =>
                {
                    i.CompletedAction = (result) =>
                    {
                        j.InsertDocument(new Document
                        {
                            Guid = result.Guid,
                            Name = result.Name,
                            FileName = result.FileName,
                            Extension = result.Extension,
                            ContentType = result.ContentType,
                            Directory = result.DocumentPath.Directory,
                            Path = result.DocumentPath.Path,
                            PhysicalBase = result.DocumentPath.Base,
                            PhysicalFullPath = result.DocumentPath.FullPath,
                            HostBase = Configuration["Path:Host"],
                            HostFullPath = Path.Join(Configuration["Path:Host"], result.DocumentPath.Path),
                            Type = (int)result.Type,
                            CreatedDate = result.CreatedDate,
                            IsActive = true
                        }).GetAwaiter().GetResult();
                    };
                });

            services
                .AddDocumentVideoService(new DocumentVideoServiceOptions
                {
                    Token = Configuration["Credential:VimeoToken"]
                })
                .Configure<Service.DocumentService.IDocumentService>((i, j) =>
                {
                    i.CompletedAction = (result) =>
                    {
                        j.InsertDocument(new Document
                        {
                            Guid = result.Guid,
                            Name = result.Name,
                            FileName = result.FileName,
                            Extension = result.Extension,
                            ContentType = result.ContentType,
                            Directory = result.DocumentPath.Directory,
                            Path = result.DocumentPath.Path,
                            HostBase = result.DocumentPath.Base,
                            HostFullPath = result.DocumentPath.FullPath,
                            Type = (int)result.Type,
                            CreatedDate = result.CreatedDate,
                            IsActive = true
                        }).GetAwaiter().GetResult();
                    };
                });
            services.AddScoped<IDocumentServiceSelector, DocumentServiceSelector>();
            // Loggers
            services.AddBasicLogger();
            services.AddSpecificBasicLogger<QueuedHostedService>(nameof(QueuedHostedService));
            // BackgroundService
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            // MVC
            services.AddControllers();
            services.AddMvcCore();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TeleNeuro.API v1"));
            }
            // Static Files
            app.UseImageSharp();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
                },
            });


            app.UseGlobalExceptionHandler(i =>
            {
                if (i is SixLabors.ImageSharp.ImageProcessingException || i is SixLabors.ImageSharp.ImageFormatException)
                {
                    string guid = Guid.NewGuid().ToString();
                    return new Exception($"Döküman yüklerken bir hata meydana geldi (IMAGE).\n\nGUID : {guid}", i);
                }
                if (i is VimeoApiException)
                {
                    string guid = Guid.NewGuid().ToString();
                    return new Exception($"Döküman yüklerken bir hata meydana geldi (VIDEO).\n\nGUID : {guid}", i);
                }
                if (i is DbException)
                {
                    string guid = Guid.NewGuid().ToString();
                    return new Exception($"Veritabaný üzerinde hata meydana geldi.\n\nGUID : {guid}", i);
                }
                return i;
            });
            // MVC
            app.UseHttpsRedirection();
            app.UseCors(i => i
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
