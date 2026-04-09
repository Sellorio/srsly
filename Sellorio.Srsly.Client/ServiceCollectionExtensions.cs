using Microsoft.Extensions.DependencyInjection;
using Sellorio.Clients.Rest;
using Sellorio.Srsly.Client.Users;
using Sellorio.Srsly.ServiceInterfaces.Users;

namespace Sellorio.Srsly.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSrslyClientSideServices(this IServiceCollection services)
    {
        services.TryAddRestClient<IUserService, UserService>();

        return services;
    }
}
