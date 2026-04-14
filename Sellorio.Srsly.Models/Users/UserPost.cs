using System.ComponentModel.DataAnnotations;

namespace Sellorio.Srsly.Models.Users;

public class UserPost
{
    [Required, StringLength(50)]
    public required string Username { get; set; }

    [Required, EmailAddress, StringLength(150)]
    public required string Email { get; set; }

    [Required, StringLength(50)]
    public required string Password { get; set; }
}
