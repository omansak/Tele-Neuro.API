using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PlayCore.Core.CustomException;
using PlayCore.Core.Extension;
using PlayCore.Core.Logger;
using PlayCore.Core.Managers.JWTAuthenticationManager;
using PlayCore.Core.QueuedHostedService;
using Service.Document.DocumentServiceSelector;
using Service.Document.File.PhysicalDrive;
using Service.Document.Image.ImageSharp;
using Service.Document.Video.Vimeo;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Web.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TeleNeuro.API.Hubs;
using TeleNeuro.API.Services;
using TeleNeuro.Entities;
using TeleNeuro.Entity.Context;
using TeleNeuro.Service.BrochureService;
using TeleNeuro.Service.CategoryService;
using TeleNeuro.Service.ExerciseService;
using TeleNeuro.Service.MessagingService;
using TeleNeuro.Service.ProgramService;
using TeleNeuro.Service.UserService;
using TeleNeuro.Service.UtilityService;
using VimeoDotNet.Exceptions;

namespace TeleNeuro.API
{
    public class Startup
    {
        public static ConcurrentBag<Role> RoleDefinitions { get; set; }
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(webHostEnvironment.ContentRootPath);
            builder = webHostEnvironment.IsDevelopment() ?
                builder.AddJsonFile($"appsettings.Development.json", optional: true, true) :
                builder.AddJsonFile($"appsettings.json", optional: false, true);

            Configuration = builder.Build();
            WebHostEnvironment = webHostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //Swagger
            services.AddSwaggerGen(i =>
            {
                i.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
                i.SwaggerDoc("v1", new OpenApiInfo { Title = "TeleNeuro.API", Version = "v1" });
                i.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please insert JWT with Bearer into field",
                        Name = "JWT Authorization",
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Type = SecuritySchemeType.Http
                    });
                i.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id =JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        new string[] { }
                    }
                });
            });
            //PlayCore
            services.AddBaseRepository();
            services.AddBaseEntityRepository();
            //Context
            services.AddDbContext<TeleNeuroDatabaseContext>(options => options.UseSqlServer(Configuration["Credential:ConnectionString"]));
            //Dependencies
            services.AddHttpContextAccessor();
            services.AddJWTAuthenticationManager(new JWTTokenConfig
            {
                Issuer = Configuration["JWTTokenConfig:Issuer"],
                Audience = Configuration["JWTTokenConfig:Audience"],
                Secret = Configuration["JWTTokenConfig:Secret"],
                AccessTokenExpirationMinute = 60 * 24 * 7,
                RefreshTokenExpirationMinute = 60 * 24 * 30
            });
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUserManagerService, UserManagerService>();
            services.AddScoped<IExerciseService, ExerciseService>();
            services.AddScoped<IBrochureService, BrochureService>();
            services.AddScoped<IProgramService, ProgramService>();
            services.AddScoped<IUtilityService, UtilityService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<INotificationHubService, NotificationHubService>();
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
                .AddDocumentFileService(new DocumentFileServiceOptions
                {
                    BaseFolder = WebHostEnvironment.WebRootPath,
                    Directory = Configuration["Path:FileUploadDirectory"],
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
            services.AddSpecificBasicLogger<IUserService>(nameof(UserService));
            // BackgroundService
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            // Auth.
            services
                .AddAuthentication(i =>
                {
                    i.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, i =>
                {
                    i.RequireHttpsMetadata = true;
                    i.SaveToken = true;
                    i.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JWTTokenConfig:Secret"])),
                        ValidIssuer = Configuration["JWTTokenConfig:Issuer"],
                        ValidAudience = Configuration["JWTTokenConfig:Audience"],
                        ValidateIssuer = !string.IsNullOrWhiteSpace(Configuration["JWTTokenConfig:Issuer"]),
                        ValidateAudience = !string.IsNullOrWhiteSpace(Configuration["JWTTokenConfig:Audience"]),
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                    i.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/Hub")))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            // Extensions
            services.AddMemoryCache();
            services.AddSignalR();

            // MVC
            if (WebHostEnvironment.IsDevelopment())
            {
                services.AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Information);
                    builder.AddFilter("Microsoft", LogLevel.Warning);
                    builder.AddFilter("System", LogLevel.Error);
                    builder.AddFilter("Engine", LogLevel.Warning);
                });
            }

            services.AddControllers();
            services.AddMvcCore().AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IUserService userService)
        {
            if (env.IsDevelopment() || env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TeleNeuro.API v1"));
            }

            //Initial Logs
            IBasicLogger logger = (IBasicLogger)app.ApplicationServices.GetService(typeof(IBasicLogger));
            if (logger != null)
            {
                logger.LogInfo("WebHostEnvironment.WebRootPath", WebHostEnvironment.WebRootPath);
                logger.LogInfo("env.IsDevelopment()", env.IsDevelopment());
            }

            // Init 
            RoleDefinitions = userService.RoleDefinition;

            // Static Files
            app.UseImageSharp();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-TypeAsd, Accept");
                }
            });


            app.UseGlobalExceptionHandler(i =>
            {
                if (i is not UIException)
                {
                    IBasicLogger exceptionLogger = (IBasicLogger)app.ApplicationServices.GetService(typeof(IBasicLogger));
                    string guid = Guid.NewGuid().ToString();
                    exceptionLogger?.LogException("--------UNHANDLED EXCEPTION--------", i);
                    switch (i)
                    {
                        case ImageProcessingException:
                        case ImageFormatException:
                            return new Exception($"Döküman yüklerken bir hata meydana geldi (IMAGE).\n\nGUID : {guid}", i);
                        case VimeoApiException:
                            return new Exception($"Döküman yüklerken bir hata meydana geldi (VIDEO).\n\nGUID : {guid}", i);
                        case DbException:
                            return new Exception($"Veritabaný üzerinde hata meydana geldi.\n\nGUID : {guid}", i);
                    }
                }
                return i;
            });

            // MVC
            app.UseHttpsRedirection();
            app.UseCors(i =>
            {
                i.SetIsOriginAllowed(_ => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/Hub/Notification");
            });
        }
    }
}
