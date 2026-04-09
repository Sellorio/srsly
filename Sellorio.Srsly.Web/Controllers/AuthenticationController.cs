using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Sellorio.Srsly.Models.Users;
using Sellorio.Srsly.Services.Users;
using System.Net;
using Sellorio.Extensions.AspNetCore;

namespace Sellorio.Srsly.Web.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginPost request)
    {
        var authenticationResult = await authenticationService.AuthenticateWithTokenAsync(request.Username, request.Password);

        if (!authenticationResult.WasSuccess)
        {
            return authenticationResult.ToActionResult(failureStatusCode: HttpStatusCode.Unauthorized);
        }

        Response.Cookies.Append(
            JwtAuthenticationOptions.CookieName,
            authenticationResult.Value.Token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = authenticationResult.Value.ExpiresAtUtc,
                IsEssential = true
            });

        return authenticationResult.ToActionResult();
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(JwtAuthenticationOptions.CookieName);
        return NoContent();
    }
}
