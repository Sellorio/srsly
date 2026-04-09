using Sellorio.AutoMapping;
using Sellorio.Srsly.Data.Users;
using Sellorio.Srsly.Models.Users;

namespace Sellorio.Srsly.Services.Users;

internal interface IMapper : IMap<UserData, User>;
