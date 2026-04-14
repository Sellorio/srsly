using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sellorio.Extensions.AspNetCore;
using Sellorio.Results;
using Sellorio.Srsly.Models.Users;
using Sellorio.Srsly.ServiceInterfaces.Users;

namespace Sellorio.Srsly.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("me")]
    public Task<IActionResult> GetMeIdAsync()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        return userService.GetUserAsync(Guid.Parse(userId)).ToActionResultAsync();
    }

    [HttpGet("me/id")]
    public IActionResult GetMeId()
    {
        // This can be used to efficiently validate that the user has a valid session
        return
            ValueResult.Success(
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)
                    .ToActionResult();
    }

    [AllowAnonymous]
    [HttpPost]
    public Task<IActionResult> RegisterAsync(UserPost user)
    {
        return userService.RegisterAsync(user).ToActionResultAsync();
    }

    [AllowAnonymous]
    [HttpPost("verify")]
    public Task<IActionResult> VerifyAsync(string code)
    {
        return userService.VerifyAsync(code).ToActionResultAsync();
    }
}
