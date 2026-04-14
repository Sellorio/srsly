using System.Threading.Tasks;
using Sellorio.Results;
using Sellorio.Srsly.Models.Users;

namespace Sellorio.Srsly.Web.Client.Services;

public interface IAuthenticationService
{
    Task<ValueResult<Login>> LoginAsync(LoginPost request);
    Task LogoutAsync();
}