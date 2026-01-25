namespace daloy_api.Models
{
    public class ModuleQuizQuestion
    {
        public Guid Id { get; set; }
        public Guid ModuleQuizId { get; set; }

        public string QuestionText { get; set; } = null!;
        public int Order { get; set; }

        public ModuleQuiz ModuleQuiz { get; set; } = null!;
        public ICollection<ModuleQuizChoice> Choices { get; set; } = new List<ModuleQuizChoice>();
    }
}
