namespace daloy_api.DTOs
{
    public class MeDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public DateTime? BirthDate { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsAdmin { get; set; }
    }


}
