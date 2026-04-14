using System;
using System.Threading.Tasks;
using Sellorio.Results;
using Sellorio.Srsly.Models.Users;

namespace Sellorio.Srsly.ServiceInterfaces.Users;

public interface IUserService
{
    Task<ValueResult<User>> GetUserAsync(Guid id);
    Task<Result<UserPost>> RegisterAsync(UserPost user);
    Task<Result> VerifyAsync(string verificationCode);
}
