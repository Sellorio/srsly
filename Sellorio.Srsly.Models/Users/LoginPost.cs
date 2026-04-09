using System;

namespace Sellorio.Srsly.Models.Users;

public class LoginPost
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

