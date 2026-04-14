using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Sellorio.Results;
using Sellorio.Srsly.Models.Users;
using Sellorio.Srsly.Services.Users;
using ServicesAuthenticationService = Sellorio.Srsly.Services.Users.IAuthenticationService;

namespace Sellorio.Srsly.Web.Services;

internal class AuthenticationService(
    ServicesAuthenticationService authenticationService,
    AuthenticationStateProvider authenticationStateProvider,
    IHttpContextAccessor httpContextAccessor)
        : Client.Services.IAuthenticationService
{
    public async Task<ValueResult<Login>> LoginAsync(LoginPost request)
    {
        var authenticationResult = await authenticationService.AuthenticateWithTokenAsync(request.Username, request.Password);

        if (!authenticationResult.WasSuccess)
        {
            return authenticationResult;
        }

        SetAuthenticationCookie(authenticationResult.Value.Token, authenticationResult.Value.ExpiresAtUtc);
        SetAuthenticationState(CreatePrincipal(authenticationResult.Value.Token));

        return authenticationResult;
    }

    public Task LogoutAsync()
    {
        DeleteAuthenticationCookie();
        SetAuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        return Task.CompletedTask;
    }

    private void SetAuthenticationState(ClaimsPrincipal principal)
    {
        if (authenticationStateProvider is IHostEnvironmentAuthenticationStateProvider hostEnvironmentAuthenticationStateProvider)
        {
            hostEnvironmentAuthenticationStateProvider.SetAuthenticationState(Task.FromResult(new AuthenticationState(principal)));
        }
    }

    private void SetAuthenticationCookie(string token, in System.DateTimeOffset expiresAtUtc)
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext?.Response.HasStarted != false)
        {
            return;
        }

        httpContext.Response.Cookies.Append(
            JwtAuthenticationOptions.CookieName,
            token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = httpContext.Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = expiresAtUtc,
                IsEssential = true
            });
    }

    private void DeleteAuthenticationCookie()
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext?.Response.HasStarted != false)
        {
            return;
        }

        httpContext.Response.Cookies.Delete(JwtAuthenticationOptions.CookieName);
    }

    private static ClaimsPrincipal CreatePrincipal(string token)
    {
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        return new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, "jwt"));
    }
}
