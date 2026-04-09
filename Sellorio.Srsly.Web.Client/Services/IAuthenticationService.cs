using System.Threading.Tasks;
using Sellorio.Srsly.Models.Users;

namespace Sellorio.Srsly.Web.Client.Services;

public interface IAuthenticationService
{
    Task<(bool Succeeded, string? ErrorMessage)> LoginAsync(LoginPost request);
    Task LogoutAsync();
}