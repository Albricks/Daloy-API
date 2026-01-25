namespace daloy_api.Models
{
    public class ModuleQuiz
    {
        public Guid Id { get; set; }
        public Guid ModuleId { get; set; }

        public string Title { get; set; } = null!;
        public int TotalQuestions { get; set; }

        public DateTime CreatedAt { get; set; }

        public Module Module { get; set; } = null!;
        public ICollection<ModuleQuizQuestion> Questions { get; set; } = new List<ModuleQuizQuestion>();
    }
}
