using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class Addtestmodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlindedTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Story = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlindedTests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CorrectionTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorrectionTests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReadingTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Story = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingTests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WritingQuestionSet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WritingQuestionSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlindedCorrects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlindedTestId = table.Column<int>(type: "int", nullable: false),
                    Correct = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlindedCorrects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlindedCorrects_BlindedTests_BlindedTestId",
                        column: x => x.BlindedTestId,
                        principalTable: "BlindedTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlindedWords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlindedTestId = table.Column<int>(type: "int", nullable: false),
                    Word = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlindedWords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlindedWords_BlindedTests_BlindedTestId",
                        column: x => x.BlindedTestId,
                        principalTable: "BlindedTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CorrectionSentences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorrectionTestId = table.Column<int>(type: "int", nullable: false),
                    Word = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorrectionSentences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorrectionSentences_CorrectionTests_CorrectionTestId",
                        column: x => x.CorrectionTestId,
                        principalTable: "CorrectionTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReadingQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReadingTestId = table.Column<int>(type: "int", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReadingQuestions_ReadingTests_ReadingTestId",
                        column: x => x.ReadingTestId,
                        principalTable: "ReadingTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WritingQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WritingQuestionsId = table.Column<int>(type: "int", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WritingQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WritingQuestions_WritingQuestionSet_WritingQuestionsId",
                        column: x => x.WritingQuestionsId,
                        principalTable: "WritingQuestionSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlindedCorrects_BlindedTestId",
                table: "BlindedCorrects",
                column: "BlindedTestId");

            migrationBuilder.CreateIndex(
                name: "IX_BlindedWords_BlindedTestId",
                table: "BlindedWords",
                column: "BlindedTestId");

            migrationBuilder.CreateIndex(
                name: "IX_CorrectionSentences_CorrectionTestId",
                table: "CorrectionSentences",
                column: "CorrectionTestId");

            migrationBuilder.CreateIndex(
                name: "IX_ReadingQuestions_ReadingTestId",
                table: "ReadingQuestions",
                column: "ReadingTestId");

            migrationBuilder.CreateIndex(
                name: "IX_WritingQuestions_WritingQuestionsId",
                table: "WritingQuestions",
                column: "WritingQuestionsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlindedCorrects");

            migrationBuilder.DropTable(
                name: "BlindedWords");

            migrationBuilder.DropTable(
                name: "CorrectionSentences");

            migrationBuilder.DropTable(
                name: "ReadingQuestions");

            migrationBuilder.DropTable(
                name: "WritingQuestions");

            migrationBuilder.DropTable(
                name: "BlindedTests");

            migrationBuilder.DropTable(
                name: "CorrectionTests");

            migrationBuilder.DropTable(
                name: "ReadingTests");

            migrationBuilder.DropTable(
                name: "WritingQuestionSet");
        }
    }
}
