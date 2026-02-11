namespace daloy_api.DTOs.admin
{
    public class AdminSituationalSummaryDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";

        public Guid ModuleId { get; set; }
        public string ModuleTitle { get; set; } = "";

        public Guid ActivityId { get; set; }
        public string ActivityTitle { get; set; } = "";

        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

}
