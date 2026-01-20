namespace daloy_api.DTOs
{
    public class MeDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
    }

}
