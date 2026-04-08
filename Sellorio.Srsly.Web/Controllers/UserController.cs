using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sellorio.Srsly.Models.Users;
using Sellorio.Srsly.Services.Users;

namespace Sellorio.Srsly.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    [HttpGet("me")]
    public ActionResult<AuthenticatedUser> GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = User.Identity?.Name;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        if (!Guid.TryParse(userId, out var id) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
        {
            return Unauthorized();
        }

        _ = Enum.TryParse<UserStatus>(User.FindFirst(JwtAuthenticationOptions.StatusClaimType)?.Value, out var status);

        return new AuthenticatedUser
        {
            Id = id,
            Username = username,
            Email = email,
            Status = status
        };
    }
}
