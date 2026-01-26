namespace daloy_api.Models
{
    public class BudgetDiaryEntry
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public DateTime EntryDate { get; set; }

        public decimal Budget { get; set; }
        public decimal Spent { get; set; }

        public decimal Saved { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
