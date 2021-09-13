using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TeleNeuro.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var wwwrootPath = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.Development.json", optional: true, true)
                        .Build()["Path:wwwroot"];
                    if (!string.IsNullOrWhiteSpace(wwwrootPath))
                        webBuilder.UseWebRoot(wwwrootPath);
                    else
                        webBuilder.UseWebRoot("wwwroot");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
