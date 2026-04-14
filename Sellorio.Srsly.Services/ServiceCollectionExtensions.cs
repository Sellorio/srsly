using Microsoft.Extensions.DependencyInjection;
using Sellorio.Srsly.ServiceInterfaces.Users;
using Sellorio.Srsly.Services.Users;
using Sellorio.Srsly.Validation;
using Sellorio.Srsly.Validation.Users;
using Sellorio.Validation.Setup;

namespace Sellorio.Srsly.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSrslyServerSideServices(this IServiceCollection services)
    {
        services
            .AddValidationService()
            .AddSrslyCoreValidators()

            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IUniqueUsernameValidator, UniqueUsernameValidator>()
            .AddScoped<IUniqueEmailValidator, UniqueEmailValidator>()
            .AddScoped<IMapper, Mapper>()
            ;

        return services;
    }
}
