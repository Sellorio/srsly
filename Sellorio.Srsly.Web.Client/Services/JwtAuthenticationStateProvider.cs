using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Sellorio.Srsly.Web.Client.Services;

public class JwtAuthenticationStateProvider(IJSRuntime jsRuntime) : AuthenticationStateProvider
{
    private const string TokenStorageKey = "srsly-auth-token";
    private static readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(_anonymous);
        }

        var claims = ParseClaims(token);

        if (IsExpired(claims))
        {
            await ClearAuthenticationStateAsync();
            return new AuthenticationState(_anonymous);
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            return await jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenStorageKey);
        }
        catch (InvalidOperationException)
        {
            return null;
        }
        catch (JSDisconnectedException)
        {
            return null;
        }
    }

    public async Task SetAuthenticationStateAsync(string token)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenStorageKey, token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task ClearAuthenticationStateAsync()
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenStorageKey);
        }
        catch (InvalidOperationException)
        {
        }
        catch (JSDisconnectedException)
        {
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    private static bool IsExpired(IReadOnlyCollection<Claim> claims)
    {
        var expClaim = claims.FirstOrDefault(x => x.Type == "exp")?.Value;

        return
            long.TryParse(expClaim, NumberStyles.None, CultureInfo.InvariantCulture, out var expiration) &&
            DateTimeOffset.FromUnixTimeSeconds(expiration) <= DateTimeOffset.UtcNow;
    }

    private static IReadOnlyCollection<Claim> ParseClaims(string token)
    {
        var tokenParts = token.Split('.');

        if (tokenParts.Length < 2)
        {
            return [];
        }

        var payload =
            tokenParts[1]
                .Replace('-', '+')
                .Replace('_', '/');

        payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');

        var payloadBytes = Convert.FromBase64String(payload);
        var payloadData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(payloadBytes);

        if (payloadData is null)
        {
            return [];
        }

        var claims = new List<Claim>();

        foreach (var (key, value) in payloadData)
        {
            if (value.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in value.EnumerateArray())
                {
                    claims.Add(new Claim(key, item.ToString()));
                }

                continue;
            }

            claims.Add(new Claim(key, value.ToString()));
        }

        return claims;
    }
}
