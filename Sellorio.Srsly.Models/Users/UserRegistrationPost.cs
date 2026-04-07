using System.ComponentModel.DataAnnotations;

namespace Sellorio.Srsly.Models.Users;

public class UserRegistrationPost
{
    [Required, StringLength(50)]
    public required string Username { get; set; }

    [Required, EmailAddress, StringLength(150)]
    public required string Email { get; set; }
}
