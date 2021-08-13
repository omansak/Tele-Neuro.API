﻿using Microsoft.Extensions.DependencyInjection;
using PlayCore.Core.Logger;

namespace PlayCore.Core.Extension
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Use for general logs such as request,response etc.
        /// </summary>
        /// <example>Use for general logs such as request,response etc.</example>
        /// <code>private readonly IBasicLogger || IBasicLogger<Class>;</code>
        /// <param name="services">In Startup.cs services</param>
        /// <param name="fileName">Log file name</param>
        /// <param name="directory">Log directory name</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddBasicLogger(this IServiceCollection services, string fileName = "Logs", string directory = "Logs")
        {
            services.AddSingleton(i => new BasicLoggerConfiguration
            {
                Directory = directory,
                FileName = fileName
            });
            services.AddSingleton(typeof(IBasicLogger<>), typeof(BasicLogger<>));

            services.AddSingleton(typeof(IBasicLogger), i => new BasicLogger(new BasicLoggerConfiguration
            {
                Directory = directory,
                FileName = fileName
            }));
            return services;
        }
        /// <summary>
        /// Use for specific file logger such User Logs
        /// </summary>
        /// <typeparam name="TCategory">Log Name </typeparam>
        /// <param name="services">In Startup.cs service</param>
        /// <param name="fileName">Log file name</param>
        /// <param name="directory">Log directory nam</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddSpecificBasicLogger<TCategory>(this IServiceCollection services, string fileName = "Logs", string directory = "Logs") where TCategory : class
        {
            return services.AddSingleton(typeof(IBasicLogger<TCategory>), i => new BasicLogger<TCategory>(new BasicLoggerConfiguration
            {
                Directory = directory,
                FileName = fileName
            }));
        }
    }
}
