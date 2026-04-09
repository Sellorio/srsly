using Sellorio.AutoMapping;
using Sellorio.Srsly.Data.Users;
using Sellorio.Srsly.Models.Users;

namespace Sellorio.Srsly.Services.Users;

internal class Mapper : MapperBase, IMapper
{
    public User Map(UserData from) => Map<User>(from);
}
