using System;

namespace Sellorio.Srsly.Models.Users;

public class User
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required UserStatus Status { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset? VerifiedAt { get; set; }
}

public class AuthenticatedUser
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required UserStatus Status { get; set; }
}

public class AuthenticationRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class AuthenticationResponse
{
    public required string Token { get; set; }
    public required DateTimeOffset ExpiresAtUtc { get; set; }
    public required AuthenticatedUser User { get; set; }
}

