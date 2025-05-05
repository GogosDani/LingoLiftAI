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
    public DbSet<WritingQuestions> WritingQuestionSet { get; set; }
    public DbSet<WritingQuestion> WritingQuestions { get; set; }
    public DbSet<ReadingQuestion> ReadingQuestions { get; set; }
    public DbSet<ReadingTest> ReadingTests { get; set; }
    public DbSet<BlindedTest> BlindedTests {get; set;}
    public DbSet<BlindedWord> BlindedWords {get; set;}
    public DbSet<BlindedCorrect> BlindedCorrects { get; set; }
    public DbSet<CorrectionTest> CorrectionTests {get; set;}
    public DbSet<CorrectionSentence> CorrectionSentences { get; set; }

    public LingoLiftContext(DbContextOptions<LingoLiftContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Language>(entity =>
            {
                entity.HasData(
                    new Language { Id = 1,  LanguageName = "English", Flag = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a5/Flag_of_the_United_Kingdom_%281-2%29.svg/1200px-Flag_of_the_United_Kingdom_%281-2%29.svg.png"},
                    new Language { Id = 2, LanguageName = "Spanish", Flag = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/9a/Flag_of_Spain.svg/1200px-Flag_of_Spain.svg.png"},
                    new Language { Id = 3, LanguageName = "German", Flag = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/ba/Flag_of_Germany.svg/1280px-Flag_of_Germany.svg.png"},
                    new Language { Id = 4, LanguageName = "Hungarian", Flag = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c1/Flag_of_Hungary.svg/255px-Flag_of_Hungary.svg.png"},
                    new Language { Id = 5, LanguageName = "Italian", Flag = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/03/Flag_of_Italy.svg/1200px-Flag_of_Italy.svg.png"},
                    new Language { Id = 6, LanguageName = "French", Flag = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c3/Flag_of_France.svg/2560px-Flag_of_France.svg.png" }
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
        
        modelBuilder.Entity<WritingQuestions>()
            .HasKey(wq => wq.Id);

        modelBuilder.Entity<WritingQuestions>()
            .HasMany(wq => wq.Questions)
            .WithOne(q => q.WritingQuestions)
            .HasForeignKey(q => q.WritingQuestionsId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<WritingQuestion>()
            .HasKey(q => q.Id);

        modelBuilder.Entity<WritingQuestion>()
            .Property(q => q.QuestionText)
            .IsRequired()
            .HasMaxLength(200);
        
        
        modelBuilder.Entity<ReadingTest>()
            .HasKey(rt => rt.Id);

        modelBuilder.Entity<ReadingTest>()
            .Property(rt => rt.UserId)
            .IsRequired();

        modelBuilder.Entity<ReadingTest>()
            .Property(rt => rt.Story)
            .IsRequired();

        modelBuilder.Entity<ReadingQuestion>()
            .HasKey(q => q.Id); 

        modelBuilder.Entity<ReadingQuestion>()
            .Property(q => q.QuestionText)
            .IsRequired()
            .HasMaxLength(250);

        modelBuilder.Entity<ReadingTest>()
            .HasMany(rt => rt.Questions)  
            .WithOne(q => q.ReadingTest)
            .HasForeignKey(q => q.ReadingTestId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<BlindedTest>()
            .HasKey(bt => bt.Id);

        modelBuilder.Entity<BlindedWord>()
            .HasKey(bw => bw.Id);
        modelBuilder.Entity<BlindedWord>()
            .HasOne(bw => bw.BlindedTest)
            .WithMany(bt => bt.Words)
            .HasForeignKey(bw => bw.BlindedTestId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BlindedCorrect>()
            .HasKey(bc => bc.Id);
        modelBuilder.Entity<BlindedCorrect>()
            .HasOne(bc => bc.BlindedTest)
            .WithMany(bt => bt.Corrects)
            .HasForeignKey(bc => bc.BlindedTestId)
            .OnDelete(DeleteBehavior.Cascade);
        
        
        modelBuilder.Entity<CorrectionTest>()
            .HasKey(ct => ct.Id);

        modelBuilder.Entity<CorrectionSentence>()
            .HasKey(cs => cs.Id);
        modelBuilder.Entity<CorrectionSentence>()
            .HasOne(cs => cs.CorrectionTest)
            .WithMany(ct => ct.Sentences)
            .HasForeignKey(cs => cs.CorrectionTestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
    
}
