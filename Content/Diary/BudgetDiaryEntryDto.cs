namespace daloy_api.Content.Diary
{
    public class BudgetDiaryEntryDto
    {
        public Guid Id { get; set; }
        public string Date { get; set; } = null!;

        public decimal Budget { get; set; }
        public decimal Spent { get; set; }

        // ✅ NEW
        public decimal Saved { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
