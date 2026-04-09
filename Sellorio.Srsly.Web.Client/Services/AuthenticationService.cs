using System.Threading.Tasks;
using Sellorio.Clients.Rest;
using Sellorio.Srsly.Models.Users;

namespace Sellorio.Srsly.Web.Client.Services;

public class AuthenticationService(IRestClient restClient, JwtAuthenticationStateProvider authenticationStateProvider) : IAuthenticationService
{
    public async Task<(bool Succeeded, string? ErrorMessage)> LoginAsync(LoginPost request)
    {
        var result = await restClient.Post($"api/authentication/login", request).ToValueResult<Login>();

        if (!result.WasSuccess)
        {
            return (false, result.ToString());
        }

        await authenticationStateProvider.SetAuthenticationStateAsync(result.Value.Token);

        return (true, null);
    }

    public async Task LogoutAsync()
    {
        _ = await restClient.Post($"api/authentication/logout");
        await authenticationStateProvider.ClearAuthenticationStateAsync();
    }
}
