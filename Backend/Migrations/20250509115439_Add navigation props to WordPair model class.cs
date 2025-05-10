using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddnavigationpropstoWordPairmodelclass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordPairs_Sets_CustomSetId",
                table: "WordPairs");

            migrationBuilder.DropIndex(
                name: "IX_WordPairs_CustomSetId",
                table: "WordPairs");

            migrationBuilder.DropColumn(
                name: "CustomSetId",
                table: "WordPairs");

            migrationBuilder.CreateIndex(
                name: "IX_WordPairs_SetId",
                table: "WordPairs",
                column: "SetId");

            migrationBuilder.AddForeignKey(
                name: "FK_WordPairs_Sets_SetId",
                table: "WordPairs",
                column: "SetId",
                principalTable: "Sets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordPairs_Sets_SetId",
                table: "WordPairs");

            migrationBuilder.DropIndex(
                name: "IX_WordPairs_SetId",
                table: "WordPairs");

            migrationBuilder.AddColumn<int>(
                name: "CustomSetId",
                table: "WordPairs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WordPairs_CustomSetId",
                table: "WordPairs",
                column: "CustomSetId");

            migrationBuilder.AddForeignKey(
                name: "FK_WordPairs_Sets_CustomSetId",
                table: "WordPairs",
                column: "CustomSetId",
                principalTable: "Sets",
                principalColumn: "Id");
        }
    }
}
