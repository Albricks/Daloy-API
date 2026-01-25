using daloy_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    // Existing video stuff
    public DbSet<Video> Videos => Set<Video>();
    public DbSet<UserVideoProgress> UserVideoProgresses => Set<UserVideoProgress>();

    // LMS / Modules
    public DbSet<Module> Modules { get; set; }
    public DbSet<ModuleObjective> ModuleObjectives { get; set; }
    public DbSet<Lesson> Lessons { get; set; }

    // 🔴 FIXED TYPO HERE
    public DbSet<UserModuleProgress> UserModuleProgresses { get; set; }
    public DbSet<LearningModule> LearningModules { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<ModuleQuiz> ModuleQuizzes { get; set; }
    public DbSet<ModuleQuizQuestion> QuizQuestions { get; set; }

    public DbSet<ModuleQuizChoice> QuizChoices { get; set; }

    public DbSet<BudgetDiaryEntry> BudgetDiaryEntries { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<BudgetDiaryEntry>()
        .HasIndex(e => new { e.UserId, e.EntryDate })
        .IsUnique();
    }

}
