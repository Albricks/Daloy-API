namespace daloy_api.Content.Diary
{
    public class UpsertBudgetDiaryEntryRequest
    {
        public DateTime EntryDate { get; set; } // YYYY-MM-DD
        public decimal Budget { get; set; }
        public decimal Spent { get; set; }
        public string? Notes { get; set; }
    }
}
