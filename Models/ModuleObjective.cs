namespace daloy_api.Models
{
    public class ModuleObjective
    {
        public Guid Id { get; set; }
        public Guid ModuleId { get; set; }
        public string Text { get; set; } = null!;
        public int Order { get; set; }
        public Module Module { get; set; } = null!;
    }
}
