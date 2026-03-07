namespace daloy_api.DTOs.admin
{
    public class AdminDashboardChartsDto
    {
        public ChartSeriesDto WeeklyActiveLearners { get; set; } = new();
        public ChartSeriesDto ModuleCompletionDistribution { get; set; } = new();
        public ChartSeriesDto AverageQuizPercentage { get; set; } = new();
        public ChartSeriesDto VideoCompletionDistribution { get; set; } = new();
    }

    public class ChartSeriesDto
    {
        public List<string> Labels { get; set; } = new();
        public List<decimal> Values { get; set; } = new();
    }

}
