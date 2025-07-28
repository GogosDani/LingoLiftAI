using System.Net;
using System.Net.Http.Json;
using System.Text;
using Backend.Data;
using Backend.Services;
using dotenv.net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace IntegrationTest;

public class WordsetIntTest
{
    private readonly WebFactory _app;
    private readonly HttpClient _client;
    private readonly TokenCreator _tokenCreator;
    private readonly LingoLiftContext _dbContext;
    private string _userToken;

    public WordsetIntTest()
    {
        _app = new();
        _client = _app.CreateClient();

        DotEnv.Load();
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();
        _tokenCreator = new(new TokenService(configuration));
        _userToken = _tokenCreator.GenerateJwtToken();
        _dbContext = _app.Scope.ServiceProvider.GetRequiredService<LingoLiftContext>();
    }

[Fact]
public async Task GetUsersWordsets_WithValidToken_ReturnsWordsets()
{
    var request = new HttpRequestMessage(HttpMethod.Get, "/api/wordset");
    request.Headers.Add("Authorization", $"Bearer {_userToken}");
    var response = await _client.SendAsync(request);
    var content = await response.Content.ReadAsStringAsync();
    Assert.NotNull(content);
    Assert.True(content.Length > 0);
}

[Fact]
public async Task GetWordsetById_WithNonExistingId_ReturnsNotFound()
{
    var request = new HttpRequestMessage(HttpMethod.Get, "/api/wordset/99999");
    request.Headers.Add("Authorization", $"Bearer {_userToken}");
    var response = await _client.SendAsync(request);
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
}
    
}