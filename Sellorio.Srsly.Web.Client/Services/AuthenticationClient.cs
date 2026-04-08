using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Sellorio.Srsly.Models.Users;

namespace Sellorio.Srsly.Web.Client.Services;

public class AuthenticationClient(HttpClient httpClient, JwtAuthenticationStateProvider authenticationStateProvider)
{
    public async Task<(bool Succeeded, string? ErrorMessage)> LoginAsync(AuthenticationRequest request)
    {
        var response = await httpClient.PostAsJsonAsync("api/authentication/login", request);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return (false, string.IsNullOrWhiteSpace(errorMessage) ? "Unable to sign in." : errorMessage);
        }

        var authenticationResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

        if (authenticationResponse is null)
        {
            return (false, "Unable to sign in.");
        }

        await authenticationStateProvider.SetAuthenticationStateAsync(authenticationResponse.Token);
        return (true, null);
    }

    public async Task LogoutAsync()
    {
        await httpClient.PostAsync("api/authentication/logout", null);
        await authenticationStateProvider.ClearAuthenticationStateAsync();
    }
}
