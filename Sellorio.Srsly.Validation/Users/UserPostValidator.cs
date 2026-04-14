using System.Threading.Tasks;
using Sellorio.Srsly.Models.Users;
using Sellorio.Validation;

namespace Sellorio.Srsly.Validation.Users;

internal class UserPostValidator : IUserPostValidator
{
    public async Task ValidateAsync(IValidationBuilder<UserPost> validate)
    {
        validate.Attributes();
        await validate.ForAsync(x => x.Username, v => v.UseValidator<IUniqueUsernameValidator>(optional: true));
        await validate.ForAsync(x => x.Email, v => v.UseValidator<IUniqueEmailValidator>(optional: true));
    }
}
