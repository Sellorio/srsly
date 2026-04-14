using Microsoft.Extensions.DependencyInjection;
using Sellorio.Srsly.Validation.Users;

namespace Sellorio.Srsly.Validation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSrslyCoreValidators(this IServiceCollection services)
    {
        return
            services
                .AddScoped<IUserPostValidator, UserPostValidator>()
                ;
    }
}
