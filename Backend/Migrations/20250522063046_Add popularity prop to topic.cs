using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class Addpopularityproptotopic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Popularity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Topics",
                columns: new[] { "Id", "Description", "Name", "Popularity" },
                values: new object[,]
                {
                    { 1, "Sports disciplines, competitions, sporting events and equipment", "Sport", 0 },
                    { 2, "Weather phenomena, forecasts and seasons", "Weather", 0 },
                    { 3, "Foods, cooking, recipes and gastronomy", "Food", 0 },
                    { 4, "Travel, destinations, cultures and tourism", "Travel", 0 },
                    { 5, "Technological innovations, devices and digital world", "Technology", 0 },
                    { 6, "Music genres, instruments, performers and concerts", "Music", 0 },
                    { 7, "Films, cinema, actors and genres", "Movies", 0 },
                    { 8, "Health, wellness, medicine and disease prevention", "Health", 0 },
                    { 9, "Learning, education, schools and academic disciplines", "Education", 0 },
                    { 10, "Environmental protection, nature and ecology", "Environment", 0 },
                    { 11, "Business, entrepreneurship, management and commerce", "Business", 0 },
                    { 12, "Fashion, clothing, styles and trends", "Fashion", 0 },
                    { 13, "Politics, governance, elections and public affairs", "Politics", 0 },
                    { 14, "Animals, species, habitats and behavior", "Animals", 0 },
                    { 15, "Scientific discoveries, research and innovations", "Science", 0 },
                    { 16, "Art forms, painting, sculpture and artists", "Art", 0 },
                    { 17, "Literature, books, authors and genres", "Books", 0 },
                    { 18, "Space exploration, planets, astronomy and space travel", "Space", 0 },
                    { 19, "Historical eras, events, figures and locations", "History", 0 },
                    { 20, "Architecture, buildings, styles and design", "Architecture", 0 },
                    { 21, "Gardening, plants, garden design and care", "Gardening", 0 },
                    { 22, "Vehicles, cars, motorcycles and transportation", "Automotive", 0 },
                    { 23, "Photography, cameras, techniques and styles", "Photography", 0 },
                    { 24, "Fitness, exercise, physical activity and sports", "Fitness", 0 },
                    { 25, "Video games, gaming platforms and game development", "Gaming", 0 },
                    { 26, "Cooking techniques, culinary arts and recipes", "Cooking", 0 },
                    { 27, "Psychology, behavior, personality and mental health", "Psychology", 0 },
                    { 28, "Religions, belief systems, traditions and ceremonies", "Religion", 0 },
                    { 29, "Languages, language learning, communication and language families", "Languages", 0 },
                    { 30, "Economy, finance, markets and investments", "Economy", 0 },
                    { 31, "Geography, landscapes, countries and natural formations", "Geography", 0 },
                    { 32, "Astronomy, celestial bodies, galaxies and phenomena", "Astronomy", 0 },
                    { 33, "Electronics, circuits, devices and development", "Electronics", 0 },
                    { 34, "Mathematics, numbers, operations and equations", "Mathematics", 0 },
                    { 35, "Dance, dance styles, choreography and performances", "Dance", 0 },
                    { 36, "Humor, stand-up, entertainment and comedy", "Comedy", 0 },
                    { 37, "Biology, life processes, organisms and taxonomy", "Biology", 0 },
                    { 38, "Chemistry, elements, compounds and reactions", "Chemistry", 0 },
                    { 39, "Physics, natural laws, energy and particles", "Physics", 0 },
                    { 40, "Medicine, healing, therapies and diagnostics", "Medicine", 0 },
                    { 41, "Mythology, legends, gods and stories", "Mythology", 0 },
                    { 42, "Philosophy, thinkers, theories and movements", "Philosophy", 0 },
                    { 43, "Hobbies, leisure activities and collecting", "Hobbies", 0 },
                    { 44, "Cultures, customs, traditions and societies", "Culture", 0 },
                    { 45, "Social media, platforms, trends and communication", "Social Media", 0 },
                    { 46, "Professions, occupations, careers and labor market", "Jobs", 0 },
                    { 47, "Family life, relationships, generations and roles", "Family", 0 },
                    { 48, "Transportation, vehicles, infrastructure and logistics", "Transportation", 0 },
                    { 49, "Law, legislation, legal systems and cases", "Law", 0 },
                    { 50, "Holidays, vacations, traditions and events", "Holidays", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Topics");
        }
    }
}
