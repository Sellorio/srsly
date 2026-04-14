using System;
using System.Threading.Tasks;
using Sellorio.Clients.Rest;
using Sellorio.Results;
using Sellorio.Srsly.Models.Users;
using Sellorio.Srsly.ServiceInterfaces.Users;

namespace Sellorio.Srsly.Client.Users;

internal class UserService(IRestClient restClient) : IUserService
{
    public Task<ValueResult<User>> GetUserAsync(Guid id)
    {
        return restClient.Get($"/api/users/{id}").ToValueResult<User>();
    }

    public Task<Result<UserPost>> RegisterAsync(UserPost user)
    {
        return restClient.Post("/api/users", user).ToResult<UserPost>();
    }

    public Task<Result> VerifyAsync(string verificationCode)
    {
        return restClient.Post($"/api/users/verify?code={verificationCode}").ToResult();
    }
}
