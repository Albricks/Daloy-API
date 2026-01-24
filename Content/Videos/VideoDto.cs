namespace daloy_api.Content.Videos
{
    public class VideoDto
    {
        public Guid Id { get; set; }


        public string Title { get; set; } = default!;

        public string Description { get; set; } = default!;

        // ISO 8601 duration or formatted string — pick ONE
        // Option A (better for logic):
        public int DurationSeconds { get; set; }


        // Option B (UI-ready):
        public string Duration { get; set; } = default!; // "05:32"


        public string Status { get; set; } = default!;
        // "not-started" | "in-progress" | "completed"


        public string ThumbnailUrl { get; set; } = default!;


        // Future-proofing (you will want these soon)
        public Guid ModuleId { get; set; }
        public int Order { get; set; } // order within module

    }
}
