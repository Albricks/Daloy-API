namespace daloy_api.Models
{
    public class ModulePreviewStandard
    {
        public Guid Id { get; set; }
        public Guid ModuleId { get; set; }

        public required string PamantayangPangnilalaman { get; set; }
        public required string PamantayanSaPagganap { get; set; }
        public required string MgaKasanayanSaPagkatuto { get; set; }
        public required string MelcsCode { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Module? Module { get; set; }
    }
}
