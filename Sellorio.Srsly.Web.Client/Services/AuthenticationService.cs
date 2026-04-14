using System.Threading.Tasks;
using Sellorio.Clients.Rest;
using Sellorio.Results;
using Sellorio.Srsly.Models.Users;

namespace Sellorio.Srsly.Web.Client.Services;

public class AuthenticationService(IRestClient restClient, JwtAuthenticationStateProvider authenticationStateProvider) : IAuthenticationService
{
    public async Task<ValueResult<Login>> LoginAsync(LoginPost request)
    {
        var result = await restClient.Post($"api/authentication/login", request).ToValueResult<Login>();

        if (!result.WasSuccess)
        {
            return result;
        }

        await authenticationStateProvider.SetAuthenticationStateAsync(result.Value.Token);

        return result;
    }

    public async Task LogoutAsync()
    {
        _ = await restClient.Post($"api/authentication/logout");
        await authenticationStateProvider.ClearAuthenticationStateAsync();
    }
}
