using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using PlayCore.Core.ActionFilter.ModelStateValidator;
using PlayCore.Core.LocalizationString;

namespace PlayCore.Core.Extension
{
    public static class ActionFilterExtensions
    {
        private static IFilterMetadata AddModelStateValidator(this FilterCollection builder, int order = -1)
        {
            return order > -1 ? builder.Add<ModelStateValidator>(order) : builder.Add<ModelStateValidator>();
        }
        public static IServiceCollection AddModelStateValidator(this IServiceCollection services, ModelStateValidatorStrings strings = null, int order = -1)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddSingleton(strings ?? new ModelStateValidatorStrings());
            return order > -1
                ? services.Configure<MvcOptions>(i => i.Filters.Add<ModelStateValidator>(order))
                : services.Configure<MvcOptions>(i => i.Filters.Add<ModelStateValidator>());
        }
    }
}
