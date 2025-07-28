using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Backend.Controllers;
using Backend.DTOs.WordsetDTOs;
using Backend.Models;
using Backend.Services.Repositories;
using Microsoft.IdentityModel.Tokens;

[TestFixture]
public class WordsetControllerTests
{
    private Mock<IWordsetRepository> _mockWordsetRepository;
    private Mock<ILanguageRepository> _mockLanguageRepository;
    private WordsetController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockWordsetRepository = new Mock<IWordsetRepository>();
        _mockLanguageRepository = new Mock<ILanguageRepository>();
        _controller = new WordsetController(_mockWordsetRepository.Object, _mockLanguageRepository.Object);
        var httpContext = new DefaultHttpContext();
        var token = CreateJwtToken("user123");
        var mockCookies = new Mock<IRequestCookieCollection>();
        mockCookies.Setup(c => c.TryGetValue("jwt", out It.Ref<string>.IsAny))
            .Returns((string key, out string value) =>
            {
                value = token;
                return true;
            });
        httpContext.Request.Cookies = mockCookies.Object;
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Test]
    public async Task CreateWordset_RepositoryThrowsException_ReturnsInternalServerError()
    {
        _mockWordsetRepository.Setup(r =>
                r.CreateWordset(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception("Database error"));
        var result = await _controller.CreateWordset("Test", 1, 2);
        var statusResult = result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(500, statusResult.StatusCode);
    }

    [Test]
    public async Task AddWordPair_ValidWordsetId_ReturnsOkWithWordPairId()
    {
        var wordsetId = 1;
        var wordPair = new WordPair { Id = 456 };
        _mockWordsetRepository.Setup(r => r.AddWordPair(wordsetId))
            .ReturnsAsync(wordPair);
        var result = await _controller.AddWordPair(wordsetId);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(456, okResult.Value);
    }

    [Test]
    public async Task EditWordPair_ValidInput_ReturnsOkWithUpdatedWordPair()
    {
        var wordPairId = 1;
        var model = new WordPairModel { FirstWord = "Hello", SecondWord = "Hola" };
        var updatedWordPair = new WordPair { Id = wordPairId, FirstWord = "Hello", SecondWord = "Hola" };
        _mockWordsetRepository.Setup(r => r.EditWordPair(wordPairId, model.FirstWord, model.SecondWord))
            .ReturnsAsync(updatedWordPair);
        var result = await _controller.EditWordPair(wordPairId, model);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(updatedWordPair, okResult.Value);
    }

    [Test]
    public async Task DeleteWordPair_ExistingWordPair_ReturnsOk()
    {
        var wordPairId = 1;
        _mockWordsetRepository.Setup(r => r.DeleteWordPair(wordPairId))
            .ReturnsAsync(true);
        var result = await _controller.DeleteWordPair(wordPairId);
        Assert.IsInstanceOf<OkResult>(result);
    }

    [Test]
    public async Task DeleteWordPair_NonExistingWordPair_ReturnsNotFound()
    {
        var wordPairId = 999;
        _mockWordsetRepository.Setup(r => r.DeleteWordPair(wordPairId))
            .ReturnsAsync(false);
        var result = await _controller.DeleteWordPair(wordPairId);
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual("WordPair 999 not found", notFoundResult.Value);
    }

    private string CreateJwtToken(string userId)
    {
        var handler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("a3c7e9d01b6f49ab12d60ce3fdd27b9a78a16e90c5473c482b43eb987d6225ce")),
                SecurityAlgorithms.HmacSha256)
        };
        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }
}