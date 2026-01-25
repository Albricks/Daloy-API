namespace daloy_api.Models
{
    public class UserModuleProgress
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ModuleId { get; set; }


        public int ProgressPercent { get; set; }
        public ModuleStatus Status { get; set; }
    }
}
