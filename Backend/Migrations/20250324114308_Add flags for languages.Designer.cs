﻿// <auto-generated />
using System;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Backend.Migrations
{
    [DbContext(typeof(LingoLiftContext))]
    [Migration("20250324114308_Add flags for languages")]
    partial class Addflagsforlanguages
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Backend.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<int?>("LevelId")
                        .HasColumnType("int");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("ProfilePictureUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ApplicationUser");
                });

            modelBuilder.Entity("Backend.Models.BlindedCorrect", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BlindedTestId")
                        .HasColumnType("int");

                    b.Property<string>("Correct")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BlindedTestId");

                    b.ToTable("BlindedCorrects");
                });

            modelBuilder.Entity("Backend.Models.BlindedTest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Story")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("BlindedTests");
                });

            modelBuilder.Entity("Backend.Models.BlindedWord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BlindedTestId")
                        .HasColumnType("int");

                    b.Property<string>("Word")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BlindedTestId");

                    b.ToTable("BlindedWords");
                });

            modelBuilder.Entity("Backend.Models.CorrectionSentence", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CorrectionTestId")
                        .HasColumnType("int");

                    b.Property<string>("Word")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CorrectionTestId");

                    b.ToTable("CorrectionSentences");
                });

            modelBuilder.Entity("Backend.Models.CorrectionTest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CorrectionTests");
                });

            modelBuilder.Entity("Backend.Models.CustomSet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("FirstLanguageId")
                        .HasColumnType("int");

                    b.Property<int>("LevelId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SecondLanguageId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("LevelId");

                    b.HasIndex("UserId");

                    b.ToTable("Sets");
                });

            modelBuilder.Entity("Backend.Models.DailyChallenge", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<string>("QuestionData")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DailyChallenges");
                });

            modelBuilder.Entity("Backend.Models.Language", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Flag")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LanguageName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Languages");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Flag = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a5/Flag_of_the_United_Kingdom_%281-2%29.svg/1200px-Flag_of_the_United_Kingdom_%281-2%29.svg.png",
                            LanguageName = "English"
                        },
                        new
                        {
                            Id = 2,
                            Flag = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/9a/Flag_of_Spain.svg/1200px-Flag_of_Spain.svg.png",
                            LanguageName = "Spanish"
                        },
                        new
                        {
                            Id = 3,
                            Flag = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/ba/Flag_of_Germany.svg/1280px-Flag_of_Germany.svg.png",
                            LanguageName = "German"
                        },
                        new
                        {
                            Id = 4,
                            Flag = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c1/Flag_of_Hungary.svg/255px-Flag_of_Hungary.svg.png",
                            LanguageName = "Hungarian"
                        },
                        new
                        {
                            Id = 5,
                            Flag = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/03/Flag_of_Italy.svg/1200px-Flag_of_Italy.svg.png",
                            LanguageName = "Italian"
                        },
                        new
                        {
                            Id = 6,
                            Flag = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c3/Flag_of_France.svg/2560px-Flag_of_France.svg.png",
                            LanguageName = "French"
                        });
                });

            modelBuilder.Entity("Backend.Models.Level", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("LevelName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Levels");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            LevelName = "Beginner"
                        },
                        new
                        {
                            Id = 2,
                            LevelName = "Elementary"
                        },
                        new
                        {
                            Id = 3,
                            LevelName = "Intermediate"
                        },
                        new
                        {
                            Id = 4,
                            LevelName = "Upper Intermediate"
                        },
                        new
                        {
                            Id = 5,
                            LevelName = "Advanced"
                        },
                        new
                        {
                            Id = 6,
                            LevelName = "Proficient"
                        });
                });

            modelBuilder.Entity("Backend.Models.ReadingQuestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("QuestionText")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<int>("ReadingTestId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ReadingTestId");

                    b.ToTable("ReadingQuestions");
                });

            modelBuilder.Entity("Backend.Models.ReadingTest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Story")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ReadingTests");
                });

            modelBuilder.Entity("Backend.Models.Test", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CompletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("LanguageId")
                        .HasColumnType("int");

                    b.Property<int>("LevelId")
                        .HasColumnType("int");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.Property<int>("SetId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("LanguageId");

                    b.HasIndex("LevelId");

                    b.HasIndex("UserId");

                    b.ToTable("Tests");
                });

            modelBuilder.Entity("Backend.Models.UserChallenge", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("DailyChallengeId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("Points")
                        .HasColumnType("int");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.Property<int>("Seconds")
                        .HasColumnType("int");

                    b.HasKey("UserId", "DailyChallengeId");

                    b.HasIndex("DailyChallengeId");

                    b.ToTable("UserChallenges");
                });

            modelBuilder.Entity("Backend.Models.UserLanguage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("LanguageId")
                        .HasColumnType("int");

                    b.Property<int>("LevelId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("LanguageId");

                    b.HasIndex("LevelId");

                    b.ToTable("UserLanguageLevels");
                });

            modelBuilder.Entity("Backend.Models.WordPair", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CustomSetId")
                        .HasColumnType("int");

                    b.Property<string>("FirstWord")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SecondWord")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SetId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CustomSetId");

                    b.ToTable("WordPairs");
                });

            modelBuilder.Entity("Backend.Models.WritingQuestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("QuestionText")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("WritingQuestionsId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("WritingQuestionsId");

                    b.ToTable("WritingQuestions");
                });

            modelBuilder.Entity("Backend.Models.WritingQuestions", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("WritingQuestionSet");
                });

            modelBuilder.Entity("Backend.Models.BlindedCorrect", b =>
                {
                    b.HasOne("Backend.Models.BlindedTest", "BlindedTest")
                        .WithMany("Corrects")
                        .HasForeignKey("BlindedTestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BlindedTest");
                });

            modelBuilder.Entity("Backend.Models.BlindedWord", b =>
                {
                    b.HasOne("Backend.Models.BlindedTest", "BlindedTest")
                        .WithMany("Words")
                        .HasForeignKey("BlindedTestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BlindedTest");
                });

            modelBuilder.Entity("Backend.Models.CorrectionSentence", b =>
                {
                    b.HasOne("Backend.Models.CorrectionTest", "CorrectionTest")
                        .WithMany("Sentences")
                        .HasForeignKey("CorrectionTestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CorrectionTest");
                });

            modelBuilder.Entity("Backend.Models.CustomSet", b =>
                {
                    b.HasOne("Backend.Models.ApplicationUser", null)
                        .WithMany("CustomSets")
                        .HasForeignKey("ApplicationUserId");

                    b.HasOne("Backend.Models.Level", "Level")
                        .WithMany()
                        .HasForeignKey("LevelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Level");
                });

            modelBuilder.Entity("Backend.Models.ReadingQuestion", b =>
                {
                    b.HasOne("Backend.Models.ReadingTest", "ReadingTest")
                        .WithMany("Questions")
                        .HasForeignKey("ReadingTestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ReadingTest");
                });

            modelBuilder.Entity("Backend.Models.Test", b =>
                {
                    b.HasOne("Backend.Models.ApplicationUser", null)
                        .WithMany("Tests")
                        .HasForeignKey("ApplicationUserId");

                    b.HasOne("Backend.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Models.Level", "Level")
                        .WithMany()
                        .HasForeignKey("LevelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Language");

                    b.Navigation("Level");
                });

            modelBuilder.Entity("Backend.Models.UserChallenge", b =>
                {
                    b.HasOne("Backend.Models.DailyChallenge", null)
                        .WithMany("UserChallenges")
                        .HasForeignKey("DailyChallengeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Backend.Models.UserLanguage", b =>
                {
                    b.HasOne("Backend.Models.ApplicationUser", null)
                        .WithMany("UserLanguages")
                        .HasForeignKey("ApplicationUserId");

                    b.HasOne("Backend.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Models.Level", "Level")
                        .WithMany()
                        .HasForeignKey("LevelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Language");

                    b.Navigation("Level");
                });

            modelBuilder.Entity("Backend.Models.WordPair", b =>
                {
                    b.HasOne("Backend.Models.CustomSet", null)
                        .WithMany("WordPairs")
                        .HasForeignKey("CustomSetId");
                });

            modelBuilder.Entity("Backend.Models.WritingQuestion", b =>
                {
                    b.HasOne("Backend.Models.WritingQuestions", "WritingQuestions")
                        .WithMany("Questions")
                        .HasForeignKey("WritingQuestionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WritingQuestions");
                });

            modelBuilder.Entity("Backend.Models.ApplicationUser", b =>
                {
                    b.Navigation("CustomSets");

                    b.Navigation("Tests");

                    b.Navigation("UserLanguages");
                });

            modelBuilder.Entity("Backend.Models.BlindedTest", b =>
                {
                    b.Navigation("Corrects");

                    b.Navigation("Words");
                });

            modelBuilder.Entity("Backend.Models.CorrectionTest", b =>
                {
                    b.Navigation("Sentences");
                });

            modelBuilder.Entity("Backend.Models.CustomSet", b =>
                {
                    b.Navigation("WordPairs");
                });

            modelBuilder.Entity("Backend.Models.DailyChallenge", b =>
                {
                    b.Navigation("UserChallenges");
                });

            modelBuilder.Entity("Backend.Models.ReadingTest", b =>
                {
                    b.Navigation("Questions");
                });

            modelBuilder.Entity("Backend.Models.WritingQuestions", b =>
                {
                    b.Navigation("Questions");
                });
#pragma warning restore 612, 618
        }
    }
}
