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
    public DbSet<ModulePreviewStandard> ModulePreviewStandards { get; set; }
    public DbSet<UserQuizAttempt> UserQuizAttempts { get; set; }

    // Other
    public DbSet<ModuleObjective> ModuleObjectives { get; set; }
    public DbSet<BudgetDiaryEntry> BudgetDiaryEntries { get; set; }
    public DbSet<SituationalActivity> SituationalActivities { get; set; }
    public DbSet<SituationalQuestion> SituationalQuestions { get; set; }
    public DbSet<UserSituationalAttempt> UserSituationalAttempts { get; set; }
    public DbSet<UserSituationalAnswer> UserSituationalAnswers { get; set; }


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

        modelBuilder.Entity<ModulePreviewStandard>()
            .HasOne(m => m.Module)
            .WithOne(m => m.PreviewStandard)
            .HasForeignKey<ModulePreviewStandard>(m => m.ModuleId);

        // =========================
        // SituationalActivity
        // =========================
        modelBuilder.Entity<SituationalActivity>(entity =>
        {
            entity.ToTable("SituationalActivities");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.ScenarioText)
                .IsRequired();

            entity.Property(e => e.MinWordCount)
                .HasDefaultValue(25);

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(e => new { e.ModuleId, e.SortOrder })
                .IsUnique();
        });

        // =========================
        // SituationalQuestion
        // =========================
        modelBuilder.Entity<SituationalQuestion>(entity =>
        {
            entity.ToTable("SituationalQuestions");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.QuestionText)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(e => new { e.ActivityId, e.SortOrder })
                .IsUnique();

            entity.HasOne<SituationalActivity>()
                .WithMany(a => a.Questions)
                .HasForeignKey(e => e.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =========================
        // UserSituationalAttempt
        // =========================
        modelBuilder.Entity<UserSituationalAttempt>(entity =>
        {
            entity.ToTable("UserSituationalAttempts");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(e => new { e.UserId, e.ActivityId })
                .IsUnique();

            entity.HasOne<SituationalActivity>()
                .WithMany()
                .HasForeignKey(e => e.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =========================
        // UserSituationalAnswer
        // =========================
        modelBuilder.Entity<UserSituationalAnswer>(entity =>
        {
            entity.ToTable("UserSituationalAnswers");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.AnswerText)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(e => new { e.AttemptId, e.QuestionId })
                .IsUnique();

            entity.HasOne(e => e.Attempt)
                .WithMany(a => a.Answers)
                .HasForeignKey(e => e.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            // IMPORTANT: NO CASCADE here (avoids multiple cascade paths)
            entity.HasOne(e => e.Question)
                .WithMany()
                .HasForeignKey(e => e.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

}
