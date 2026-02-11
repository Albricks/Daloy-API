namespace daloy_api.DTOs.Admin
{
    public class AdminKpiDto
    {
        public int TotalLearners { get; set; }
        public double AvgModuleCompletionPercent { get; set; }
        public int SituationalCompletionRate { get; set; }
        public int QuizPassRate { get; set; }
        public double AvgVideoCompletionPercent { get; set; }
    }
}
