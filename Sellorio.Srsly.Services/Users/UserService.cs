using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sellorio.Results;
using Sellorio.Results.Messages;
using Sellorio.Srsly.Data;
using Sellorio.Srsly.Data.Users;
using Sellorio.Srsly.Models.Users;
using Sellorio.Srsly.ServiceInterfaces.Users;
using Sellorio.Srsly.Validation.Users;
using Sellorio.Validation;

namespace Sellorio.Srsly.Services.Users;

internal partial class UserService(
    DatabaseContext databaseContext,
    IValidationService validationService,
    IAuthenticationService authenticationService,
    IMapper mapper,
    ILogger<UserService> logger) : IUserService
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

    public async Task<Result<UserPost>> RegisterAsync(UserPost user)
    {
        var validationResult = await validationService.ValidateAsync(user, v => v.UseValidator<IUserPostValidator>());

        if (!validationResult.WasSuccess)
        {
            return validationResult;
        }

        var data = new UserData
        {
            CreatedAt = DateTime.UtcNow,
            Email = user.Email,
            Username = user.Username,
            PasswordHash = authenticationService.GeneratePasswordHash(user.Username, user.Password).Value,
            Status = UserStatus.Unverified,
            VerificationCode = Guid.NewGuid().ToString()
        };

        databaseContext.Users.Add(data);

        await databaseContext.SaveChangesAsync();

        LogVerificationCode(data.VerificationCode);

        return ResultMessage.Information("Registration successful! Please contact the server administrator for next steps.");
    }

    public async Task<Result> VerifyAsync(string verificationCode)
    {
        var user = await databaseContext.Users.FirstOrDefaultAsync(u => u.VerificationCode == verificationCode);

        if (user is null)
        {
            return ResultMessage.Error("Invalid verification code.");
        }

        if (user.Status == UserStatus.Verified)
        {
            return ResultMessage.Information("User is already verified.");
        }

        user.Status = UserStatus.Verified;
        user.VerifiedAt = DateTime.UtcNow;

        await databaseContext.SaveChangesAsync();

        return Result.Success();
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "New user registered. Verification code is {VerificationCode}.")]
    private partial void LogVerificationCode(string verificationCode);
}
