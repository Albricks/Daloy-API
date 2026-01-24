namespace daloy_api.Models
{
    public class UserVideoProgress
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public Guid VideoId { get; set; }

        public double LastPositionSeconds { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime LastWatchedAt { get; set; }
    }
}
