namespace daloy_api.DTOs.Admin
{
    public class AdminUserListDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
    }
}
