namespace Backend.Models;

public class BlindedTest
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Story { get; set; }
    public List<BlindedWord> Words { get; set; } = new();
    public List<BlindedCorrect> Corrects { get; set; } = new();
}