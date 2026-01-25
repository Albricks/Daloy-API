namespace daloy_api.Content.Diary
{
    public class BudgetDiarySummaryDto
    {
        public decimal TotalBudget { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal TotalSaved { get; set; }


        public int EntryCount { get; set; }
    }
}
