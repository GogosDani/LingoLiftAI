using Microsoft.AspNetCore.Identity;

namespace Backend.Models;

public class ApplicationUser : IdentityUser
{
    public string ProfilePictureUrl { get; set; }
    public int LevelId { get; set; }
    public ICollection<Test> Tests { get; set; } = new List<Test>();
    public ICollection<CustomSet> CustomSets { get; set; } = new List<CustomSet>();
    public ICollection<UserLanguage> UserLanguages { get; set; } = new List<UserLanguage>();
}