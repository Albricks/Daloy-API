using System.ComponentModel.DataAnnotations;
namespace daloy_api.DTOs;

public class RegisterRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    public string Username { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;

    [Required]
    public string FullName { get; set; } = default!;

    public DateTime BirthDate { get; set; }

    public IFormFile? Avatar { get; set; }
}

