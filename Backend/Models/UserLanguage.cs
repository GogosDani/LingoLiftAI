namespace Backend.Models;

public class UserLanguage
{
    public int Id { get; set; }
    public string UserId { get; init; }
    public int LanguageId { get; init; }
    public Language Language { get; set; } 
    public int LevelId { get; set; }
    public Level Level { get; set; }
}