using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sellorio.Srsly.Data;
using Sellorio.Srsly.Validation.Users;
using Sellorio.Validation;

namespace Sellorio.Srsly.Services.Users;

internal class UniqueUsernameValidator(DatabaseContext databaseContext) : IUniqueUsernameValidator
{
    public async Task ValidateAsync(IValidationBuilder<string> validate)
    {
        if (await databaseContext.Users.AnyAsync(x => x.Username == validate.Target))
        {
            validate.AddMessage("Is not unique.");
        }
    }
}
