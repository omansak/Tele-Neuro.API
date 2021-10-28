using Microsoft.Extensions.DependencyInjection;
using PlayCore.Core.Managers.JWTAuthenticationManager;

namespace PlayCore.Core.Extension
{
    public static class ManagerExtensions
    {
        public static IServiceCollection AddJWTAuthenticationManager(this IServiceCollection services)
        {
            services.AddHostedService<JWTManagerRefreshCache>();
            services.AddSingleton<IJWTAuthenticationManager>(i => new JWTAuthenticationManager(new JWTTokenConfig()));
            return services;
        }
        public static IServiceCollection AddJWTAuthenticationManager(this IServiceCollection services, JWTTokenConfig config)
        {
            services.AddHostedService<JWTManagerRefreshCache>();
            services.AddSingleton<IJWTAuthenticationManager>(i => new JWTAuthenticationManager(config));
            return services;
        }
    }
}
