using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using PlayCore.Core.LocalizationString;
using PlayCore.Core.Middleware.GlobalExceptionHandler;
using System;

namespace PlayCore.Core.Extension
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandler>();
        }
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder, Func<Exception, Exception> action)
        {
            return builder.UseMiddleware<GlobalExceptionHandler>(action);
        }
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder, GlobalExceptionHandlerStrings strings)
        {
            return builder.UseMiddleware<GlobalExceptionHandler>(strings);
        }
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder, GlobalExceptionHandlerStrings strings, Func<Exception, Exception> action)
        {
            return builder.UseMiddleware<GlobalExceptionHandler>(strings, action);
        }
    }
}
