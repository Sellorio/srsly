using Microsoft.Extensions.DependencyInjection;
using Sellorio.Srsly.Services.Users;

namespace Sellorio.Srsly.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSrslyServerSideServices(this IServiceCollection services)
    {
        services
            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddScoped<IMapper, Mapper>()
            ;

        return services;
    }
}
