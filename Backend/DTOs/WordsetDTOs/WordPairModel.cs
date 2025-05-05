using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.WordsetDTOs;

public class WordPairModel
{
    [Required]
    public string FirstWord { get; set; }
    [Required]
    public string SecondWord { get; set; }
}