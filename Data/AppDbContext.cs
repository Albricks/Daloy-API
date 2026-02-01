using daloy_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    // Video learning
    public DbSet<LearningModule> LearningModules { get; set; }
    public DbSet<Video> Videos { get; set; }
    public DbSet<UserVideoProgress> UserVideoProgresses { get; set; }

    // Lesson-based modules
    public DbSet<Module> Modules { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<UserModuleProgress> UserModuleProgresses { get; set; }
    public DbSet<UserLessonProgress> UserLessonProgresses { get; set; }

    // Quizzes
    public DbSet<ModuleQuiz> ModuleQuizzes { get; set; }
    public DbSet<ModuleQuizQuestion> ModuleQuizQuestions { get; set; }
    public DbSet<ModuleQuizChoice> ModuleQuizChoices { get; set; }
    public DbSet<UserQuizAttempt> UserQuizAttempts { get; set; }

    // Other
    public DbSet<ModuleObjective> ModuleObjectives { get; set; }
    public DbSet<BudgetDiaryEntry> BudgetDiaryEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ----------------------------
        // Explicit table mappings
        // ----------------------------
        modelBuilder.Entity<UserModuleProgress>()
            .ToTable("UserModuleProgress");

        modelBuilder.Entity<UserLessonProgress>()
            .ToTable("UserLessonProgress");

        modelBuilder.Entity<UserVideoProgress>()
            .ToTable("UserVideoProgress");

        modelBuilder.Entity<UserQuizAttempt>()
            .ToTable("UserQuizAttempts");

        // ----------------------------
        // Indexes & constraints
        // ----------------------------
        modelBuilder.Entity<BudgetDiaryEntry>()
            .HasIndex(e => new { e.UserId, e.EntryDate })
            .IsUnique();

        modelBuilder.Entity<BudgetDiaryEntry>()
            .Property(e => e.Saved)
            .HasComputedColumnSql("[Budget] - [Spent]", stored: true);

        modelBuilder.Entity<UserModuleProgress>()
            .HasIndex(x => new { x.UserId, x.ModuleId })
            .IsUnique();

        modelBuilder.Entity<UserLessonProgress>()
            .HasIndex(x => new { x.UserId, x.LessonId })
            .IsUnique();

        modelBuilder.Entity<UserQuizAttempt>()
            .HasIndex(x => new { x.UserId, x.QuizId, x.AttemptedAt });

        modelBuilder.Entity<UserVideoProgress>()
            .HasIndex(x => new { x.UserId, x.VideoId })
            .IsUnique();
    }
}
