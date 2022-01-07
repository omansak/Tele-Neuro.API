using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

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
                    var configurationBuilder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory());

                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development)
                    {
                        configurationBuilder.AddJsonFile($"appsettings.Development.json", optional: true, true);
                    }
                    else
                    {
                        configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    }


                    var configuration = configurationBuilder.Build();

                    webBuilder.UseWebRoot(!string.IsNullOrWhiteSpace(configuration["Path:wwwroot"]) ? configuration["Path:wwwroot"] : "wwwroot");

                    webBuilder.UseStartup<Startup>();
                });
    }
}
