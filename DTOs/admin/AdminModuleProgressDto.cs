namespace daloy_api.DTOs.admin
{
    public class AdminModuleProgressDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";

        public Guid ModuleId { get; set; }
        public string ModuleTitle { get; set; } = "";
        public string Level { get; set; } = "";

        public int Status { get; set; }
        public int ProgressPercent { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? LastAccessedAt { get; set; }
    }

}
