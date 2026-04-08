using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Sellorio.Srsly.Models.Users;
using Sellorio.Srsly.Services.Users;

namespace Sellorio.Srsly.Web.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthenticationResponse>> Login([FromBody] AuthenticationRequest request)
    {
        var authenticationResponse = await authenticationService.AuthenticateWithTokenAsync(request.Username, request.Password);

        if (authenticationResponse is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Invalid username or password."
            });
        }

        Response.Cookies.Append(
            JwtAuthenticationOptions.CookieName,
            authenticationResponse.Token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = authenticationResponse.ExpiresAtUtc,
                IsEssential = true
            });

        return authenticationResponse;
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(JwtAuthenticationOptions.CookieName);
        return NoContent();
    }
}
