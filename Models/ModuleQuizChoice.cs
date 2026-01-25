namespace daloy_api.Models
{
    public class ModuleQuizChoice
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }

        public string ChoiceLabel { get; set; } = null!; // A, B, C, D
        public string ChoiceText { get; set; } = null!;
        public bool IsCorrect { get; set; }

        public ModuleQuizQuestion Question { get; set; } = null!;
    }
}
