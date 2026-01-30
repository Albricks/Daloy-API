namespace daloy_api.DTOs
{
    public class UpdateVideoProgressDto
    {
        public Guid VideoId { get; set; }
        public int WatchedSeconds { get; set; }
    }

}
