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
    public DbSet<Topic> Topics { get; set; }
    public DbSet<AiWordSet> AiWordSets { get; set; }
    public DbSet<AiWordPair> AiWordPairs { get; set; }

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
        
        modelBuilder.Entity<WordPair>()
            .HasOne(wp => wp.Set)
            .WithMany(cs => cs.WordPairs)
            .HasForeignKey(wp => wp.SetId);
        
        modelBuilder.Entity<AiWordSet>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.UserId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.DifficultyLevel).HasMaxLength(50).IsRequired();
            entity.HasOne(e => e.Topic);
        });

        modelBuilder.Entity<AiWordPair>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstWord).HasMaxLength(100).IsRequired();
            entity.Property(e => e.SecondWord).HasMaxLength(100).IsRequired();
        
            entity.HasOne(e => e.WordSet)
                .WithMany(ws => ws.WordPairs)
                .HasForeignKey(e => e.WordSetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Topic>().HasData(
            new Topic { Id = 1, Name = "Sport", Description = "Sports disciplines, competitions, sporting events and equipment" },
            new Topic { Id = 2, Name = "Weather", Description = "Weather phenomena, forecasts and seasons" },
            new Topic { Id = 3, Name = "Food", Description = "Foods, cooking, recipes and gastronomy" },
            new Topic { Id = 4, Name = "Travel", Description = "Travel, destinations, cultures and tourism" },
            new Topic { Id = 5, Name = "Technology", Description = "Technological innovations, devices and digital world" },
            new Topic { Id = 6, Name = "Music", Description = "Music genres, instruments, performers and concerts" },
            new Topic { Id = 7, Name = "Movies", Description = "Films, cinema, actors and genres" },
            new Topic { Id = 8, Name = "Health", Description = "Health, wellness, medicine and disease prevention" },
            new Topic { Id = 9, Name = "Education", Description = "Learning, education, schools and academic disciplines" },
            new Topic { Id = 10, Name = "Environment", Description = "Environmental protection, nature and ecology" },
            new Topic { Id = 11, Name = "Business", Description = "Business, entrepreneurship, management and commerce" },
            new Topic { Id = 12, Name = "Fashion", Description = "Fashion, clothing, styles and trends" },
            new Topic { Id = 13, Name = "Politics", Description = "Politics, governance, elections and public affairs" },
            new Topic { Id = 14, Name = "Animals", Description = "Animals, species, habitats and behavior" },
            new Topic { Id = 15, Name = "Science", Description = "Scientific discoveries, research and innovations" },
            new Topic { Id = 16, Name = "Art", Description = "Art forms, painting, sculpture and artists" },
            new Topic { Id = 17, Name = "Books", Description = "Literature, books, authors and genres" },
            new Topic { Id = 18, Name = "Space", Description = "Space exploration, planets, astronomy and space travel" },
            new Topic { Id = 19, Name = "History", Description = "Historical eras, events, figures and locations" },
            new Topic { Id = 20, Name = "Architecture", Description = "Architecture, buildings, styles and design" },
            new Topic { Id = 21, Name = "Gardening", Description = "Gardening, plants, garden design and care" },
            new Topic { Id = 22, Name = "Automotive", Description = "Vehicles, cars, motorcycles and transportation" },
            new Topic { Id = 23, Name = "Photography", Description = "Photography, cameras, techniques and styles" },
            new Topic { Id = 24, Name = "Fitness", Description = "Fitness, exercise, physical activity and sports" },
            new Topic { Id = 25, Name = "Gaming", Description = "Video games, gaming platforms and game development" },
            new Topic { Id = 26, Name = "Cooking", Description = "Cooking techniques, culinary arts and recipes" },
            new Topic { Id = 27, Name = "Psychology", Description = "Psychology, behavior, personality and mental health" },
            new Topic { Id = 28, Name = "Religion", Description = "Religions, belief systems, traditions and ceremonies" },
            new Topic { Id = 29, Name = "Languages", Description = "Languages, language learning, communication and language families" },
            new Topic { Id = 30, Name = "Economy", Description = "Economy, finance, markets and investments" },
            new Topic { Id = 31, Name = "Geography", Description = "Geography, landscapes, countries and natural formations" },
            new Topic { Id = 32, Name = "Astronomy", Description = "Astronomy, celestial bodies, galaxies and phenomena" },
            new Topic { Id = 33, Name = "Electronics", Description = "Electronics, circuits, devices and development" },
            new Topic { Id = 34, Name = "Mathematics", Description = "Mathematics, numbers, operations and equations" },
            new Topic { Id = 35, Name = "Dance", Description = "Dance, dance styles, choreography and performances" },
            new Topic { Id = 36, Name = "Comedy", Description = "Humor, stand-up, entertainment and comedy" },
            new Topic { Id = 37, Name = "Biology", Description = "Biology, life processes, organisms and taxonomy" },
            new Topic { Id = 38, Name = "Chemistry", Description = "Chemistry, elements, compounds and reactions" },
            new Topic { Id = 39, Name = "Physics", Description = "Physics, natural laws, energy and particles" },
            new Topic { Id = 40, Name = "Medicine", Description = "Medicine, healing, therapies and diagnostics" },
            new Topic { Id = 41, Name = "Mythology", Description = "Mythology, legends, gods and stories" },
            new Topic { Id = 42, Name = "Philosophy", Description = "Philosophy, thinkers, theories and movements" },
            new Topic { Id = 43, Name = "Hobbies", Description = "Hobbies, leisure activities and collecting" },
            new Topic { Id = 44, Name = "Culture", Description = "Cultures, customs, traditions and societies" },
            new Topic {  Id = 45, Name = "Social Media", Description = "Social media, platforms, trends and communication" },
            new Topic { Id = 46, Name = "Jobs", Description = "Professions, occupations, careers and labor market" },
            new Topic { Id = 47, Name = "Family", Description = "Family life, relationships, generations and roles" },
            new Topic { Id = 48, Name = "Transportation", Description = "Transportation, vehicles, infrastructure and logistics" },
            new Topic { Id = 49, Name = "Law", Description = "Law, legislation, legal systems and cases" },
            new Topic { Id = 50, Name = "Holidays", Description = "Holidays, vacations, traditions and events" }
        );
        
        modelBuilder.Entity<UserChallenge>()
            .HasOne(uc => uc.User)
            .WithMany(u => u.UserChallenges)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserChallenge>()
            .HasOne(uc => uc.DailyChallenge)
            .WithMany(dc => dc.UserChallenges)
            .HasForeignKey(uc => uc.DailyChallengeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DailyChallenge>()
            .HasIndex(dc => dc.Date)
            .IsUnique();

        modelBuilder.Entity<UserChallenge>()
            .HasIndex(uc => new { uc.UserId, uc.DailyChallengeId })
            .IsUnique();

        modelBuilder.Entity<UserChallenge>()
            .HasIndex(uc => uc.DailyChallengeId);

        modelBuilder.Entity<UserChallenge>()
            .HasIndex(uc => uc.Score);
    }
}
