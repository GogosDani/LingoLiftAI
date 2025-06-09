using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class Createdailychallenge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Points",
                table: "UserChallenges");

            migrationBuilder.DropColumn(
                name: "Seconds",
                table: "UserChallenges");

            migrationBuilder.RenameColumn(
                name: "QuestionData",
                table: "DailyChallenges",
                newName: "Content");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "UserChallenges",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "DailyChallenges",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "DailyChallenges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserChallenges_Score",
                table: "UserChallenges",
                column: "Score");

            migrationBuilder.CreateIndex(
                name: "IX_UserChallenges_UserId_DailyChallengeId",
                table: "UserChallenges",
                columns: new[] { "UserId", "DailyChallengeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyChallenges_Date",
                table: "DailyChallenges",
                column: "Date",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserChallenges_Score",
                table: "UserChallenges");

            migrationBuilder.DropIndex(
                name: "IX_UserChallenges_UserId_DailyChallengeId",
                table: "UserChallenges");

            migrationBuilder.DropIndex(
                name: "IX_DailyChallenges_Date",
                table: "DailyChallenges");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "UserChallenges");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "DailyChallenges");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "DailyChallenges",
                newName: "QuestionData");

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "UserChallenges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Seconds",
                table: "UserChallenges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "DailyChallenges",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
