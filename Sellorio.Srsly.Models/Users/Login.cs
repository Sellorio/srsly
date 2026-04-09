using System;

namespace Sellorio.Srsly.Models.Users;

public class Login
{
    public required string Token { get; set; }
    public required DateTimeOffset ExpiresAtUtc { get; set; }
    public required User User { get; set; }
}

