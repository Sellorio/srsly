using System;
using System.Threading.Tasks;
using Sellorio.Results;
using Sellorio.Srsly.Models.Users;

namespace Sellorio.Srsly.ServiceInterfaces.Users;

public interface IUserService
{
    Task<ValueResult<User>> GetUserAsync(Guid id);
}
