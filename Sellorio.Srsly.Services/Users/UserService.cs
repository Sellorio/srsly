using System;
using System.Threading.Tasks;
using Sellorio.Results;
using Sellorio.Results.Messages;
using Sellorio.Srsly.Data;
using Sellorio.Srsly.Models.Users;
using Sellorio.Srsly.ServiceInterfaces.Users;

namespace Sellorio.Srsly.Services.Users;

internal class UserService(DatabaseContext databaseContext, IMapper mapper) : IUserService
{
    public async Task<ValueResult<User>> GetUserAsync(Guid id)
    {
        var data = await databaseContext.Users.FindAsync(id);

        if (data == null)
        {
            return ResultMessage.NotFound("User");
        }

        return mapper.Map(data);
    }
}
