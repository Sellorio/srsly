using System.Threading.Tasks;
using Sellorio.Results;
using Sellorio.Srsly.Models.Users;

namespace Sellorio.Srsly.Services.Users;

public interface IAuthenticationService
{
    Task<ValueResult<User>> AuthenticateUserAsync(string username, string password);
    Task<ValueResult<Login>> AuthenticateWithTokenAsync(string username, string password);
    ValueResult<string> GeneratePasswordHash(string username, string password);
}
