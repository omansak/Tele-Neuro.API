using Microsoft.Extensions.DependencyInjection;
using PlayCore.Core.Repository;

namespace PlayCore.Core.Extension
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddBaseEntityRepository(this IServiceCollection services)
        {
            return services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
        }
        public static IServiceCollection AddBaseRepository(this IServiceCollection services)
        {
            return services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        }
    }
}
