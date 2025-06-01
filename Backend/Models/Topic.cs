namespace Backend.Models;

public class Topic
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public int Popularity { get; set; } = 0;
}