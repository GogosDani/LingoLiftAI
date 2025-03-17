using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class LingoLiftContext : DbContext
{
    
    public DbSet<Language> Languages { get; set; }
    public DbSet<Level> Levels { get; set; }
    public DbSet<CustomSet> Sets { get; set; }
    public DbSet<DailyChallenge> DailyChallenges { get; set; }
    public DbSet<Test> Tests { get; set; }
    public DbSet<UserChallenge> UserChallenges { get; set; }
    public DbSet<WordPair> WordPairs { get; set; }
    public DbSet<UserLanguage> UserLanguageLevels { get; set; }

    public LingoLiftContext(DbContextOptions<LingoLiftContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Language>(entity =>
            {
                entity.HasData(
                    new Language { Id = 1,  LanguageName = "English" },
                    new Language { Id = 2, LanguageName = "Spanish" },
                    new Language { Id = 3, LanguageName = "German" },
                    new Language { Id = 4, LanguageName = "Hungarian" },
                    new Language { Id = 5, LanguageName = "Italian" },
                    new Language { Id = 6, LanguageName = "French" }
                );
            }
        );

        modelBuilder.Entity<Level>(entity =>
        {
            entity.HasData(
                new Level{Id = 1, LevelName = "Beginner"},
                new Level{Id = 2, LevelName = "Elementary"},
                new Level{Id = 3, LevelName = "Intermediate"},
                new Level{Id = 4, LevelName = "Upper Intermediate"},
                new Level{Id = 5, LevelName = "Advanced"},
                new Level{Id = 6, LevelName = "Proficient"}
                );
        });
        
        modelBuilder.Entity<Test>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Test>()
            .HasOne(t => t.Language)
            .WithMany()
            .HasForeignKey(t => t.LanguageId);

        modelBuilder.Entity<UserChallenge>()
            .HasKey(uc => new { uc.UserId, uc.DailyChallengeId });

        modelBuilder.Entity<UserChallenge>()
            .HasOne<ApplicationUser>() 
            .WithMany() // 
            .HasForeignKey(uc => uc.UserId) 
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<UserChallenge>()
            .HasOne<DailyChallenge>() 
            .WithMany()
            .HasForeignKey(uc => uc.DailyChallengeId) 
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CustomSet>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(cs => cs.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CustomSet>()
            .HasOne(cs => cs.Level)
            .WithMany()
            .HasForeignKey(cs => cs.LevelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DailyChallenge>()
            .HasMany(d => d.UserChallenges) 
            .WithOne() 
            .HasForeignKey(uc => uc.DailyChallengeId) 
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserLanguage>()
            .HasOne(ul => ul.Language)
            .WithMany()
            .HasForeignKey(ul => ul.LanguageId);
        
        modelBuilder.Entity<UserLanguage>()
            .HasOne(ul => ul.Level)
            .WithMany()
            .HasForeignKey(ul => ul.LevelId);
    }
}
