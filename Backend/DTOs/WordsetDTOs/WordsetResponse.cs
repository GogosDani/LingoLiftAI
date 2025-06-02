using Backend.Models;

namespace Backend.DTOs.WordsetDTOs;

public record WordsetResponse(int Id, string Name, Language FirstLanguage, Language SecondLanguage, List<WordPairResponse> WordPairs);