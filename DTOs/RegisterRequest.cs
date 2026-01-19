namespace daloy_api.DTOs;

public class RegisterRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public DateTime BirthDate { get; set; }
}
