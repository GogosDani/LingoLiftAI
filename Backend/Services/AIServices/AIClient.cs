using System.Text;
using System.Text.Json;
using Backend.DTOs.AIDTOs;
using Backend.Exceptions.AIExceptions;

namespace Backend.Services.AIServices;

public class AIClient : IAIClient
{
    private readonly string _url;
    private readonly HttpClient _client;

    public AIClient(IConfiguration configuration)
    {
        if(configuration["GeminiUrl"] == null) throw new NullGeminiUrlException("GeminiUrl is missing");
        _url = configuration["GeminiUrl"];
        _client = new HttpClient();
    }


    public async Task<string> GetAiAnswer(string prompt)
    {
        var requestData = new { contents = new[]{new{parts = new[]{new{text = prompt}}}} };
        string json = JsonSerializer.Serialize(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _client.PostAsync(_url, content);
        string resultJson = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GeminiResponse>(resultJson);
        if(result == null) throw new NullAnswerException("GeminiResponse is null");
        return result.candidates[0].content.parts[0].text;
    }
}