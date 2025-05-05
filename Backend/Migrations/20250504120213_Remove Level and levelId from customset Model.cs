using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLevelandlevelIdfromcustomsetModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sets_Levels_LevelId",
                table: "Sets");

            migrationBuilder.DropIndex(
                name: "IX_Sets_LevelId",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "Sets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LevelId",
                table: "Sets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sets_LevelId",
                table: "Sets",
                column: "LevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_Levels_LevelId",
                table: "Sets",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
