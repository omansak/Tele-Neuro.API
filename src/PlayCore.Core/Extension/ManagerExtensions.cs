using Microsoft.Extensions.DependencyInjection;
using PlayCore.Core.Managers.JWTAuthenticationManager;

namespace PlayCore.Core.Extension
{
    public static class ManagerExtensions
    {
        public static IServiceCollection AddJWTAuthenticationManager(this IServiceCollection services)
        {
            services.AddHostedService<JwtManagerRefreshCache>();
            services.AddSingleton<IJwtAuthenticationManager>(i => new JwtAuthenticationManager(new JwtTokenConfig()));
            return services;
        }
        public static IServiceCollection AddJWTAuthenticationManager(this IServiceCollection services, JwtTokenConfig config)
        {
            services.AddHostedService<JwtManagerRefreshCache>();
            services.AddSingleton<IJwtAuthenticationManager>(i => new JwtAuthenticationManager(config));
            return services;
        }
    }
}
