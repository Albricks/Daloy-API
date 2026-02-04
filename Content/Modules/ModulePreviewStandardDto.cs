namespace daloy_api.Content.Modules
{
    public class ModulePreviewStandardDto
    {
        public Guid ModuleId { get; set; }

        public required string PamantayangPangnilalaman { get; set; }
        public required string PamantayanSaPagganap { get; set; }
        public required string MgaKasanayanSaPagkatuto { get; set; }
        public required string MelcsCode { get; set; }
    }

}
