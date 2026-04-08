using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Sellorio.Srsly.Models.Users;

namespace Sellorio.Srsly.Data.Users;

[Index(nameof(Email), IsUnique = true)]
[Index(nameof(Username), IsUnique = true)]
[Index(nameof(VerificationCode), IsUnique = true)]
public class UserData
{
    public Guid Id { get; set; }

    [Required, StringLength(50)]
    public required string Username { get; set; }

    [Required, EmailAddress, StringLength(150)]
    public required string Email { get; set; }

    [Required, StringLength(255)]
    public required string PasswordHash { get; set; }

    [Required, StringLength(20)]
    public required string VerificationCode { get; set; }

    public required UserStatus Status { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset? VerifiedAt { get; set; }
}

