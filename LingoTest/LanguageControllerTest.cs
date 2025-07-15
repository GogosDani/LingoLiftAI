using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Controllers;
using Backend.DTOs;
using Backend.Models;
using Backend.Services.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Moq;

[TestFixture]
public class LanguageControllerTest
{
    private Mock<ILanguageRepository> _languageRepositoryMock;
    private Mock<IUserLanguageRepository> _userLanguageRepositoryMock;
    private LanguageController _languageController;

    [SetUp]
    public void Setup()
    {
        _languageRepositoryMock = new Mock<ILanguageRepository>();
        _userLanguageRepositoryMock = new Mock<IUserLanguageRepository>();
        _languageController = new LanguageController(_languageRepositoryMock.Object, _userLanguageRepositoryMock.Object);
        
        var httpContext = new DefaultHttpContext();
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };
        _languageController.ControllerContext = controllerContext;
    }

    [Test]
    public async Task GetLanguages_ReturnsOkResult_WhenLanguagesExist()
    {
        var expectedLanguages = new List<Language>
        {
            new Language { Id = 1, LanguageName = "English" },
            new Language { Id = 2, LanguageName = "Spanish" }
        };
        _languageRepositoryMock.Setup(x => x.GetAllLanguages())
            .ReturnsAsync(expectedLanguages);
        var result = await _languageController.GetLanguages();
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(expectedLanguages));
        });
    }

    [Test]
    public async Task GetLanguages_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        var exceptionMessage = "Database connection failed";
        _languageRepositoryMock.Setup(x => x.GetAllLanguages())
            .ThrowsAsync(new Exception(exceptionMessage));
        var result = await _languageController.GetLanguages();
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo(exceptionMessage));
        });
    }

    [Test]
    public async Task AddUserLanguageBeginner_ReturnsOkResult_WhenValidRequestWithJwtToken()
    {
        var request = new UserLanguageRequest(1);
        var userId = "123";
        var jwtToken = CreateJwtToken(userId);
        var mockHttpContext = new Mock<HttpContext>();
        var mockRequest = new Mock<HttpRequest>();
        var mockCookies = new Mock<IRequestCookieCollection>();
        mockCookies.Setup(c => c["jwt"]).Returns(jwtToken);
        mockRequest.Setup(r => r.Cookies).Returns(mockCookies.Object);
        mockHttpContext.Setup(h => h.Request).Returns(mockRequest.Object);
        _languageController.ControllerContext = new ControllerContext()
        {
            HttpContext = mockHttpContext.Object
        };
        _userLanguageRepositoryMock.Setup(x => x.AddUserLanguageLevel(userId, request.LanguageId, "Beginner"))
            .Returns(Task.CompletedTask);
        var result = await _languageController.AddUserLanguageBeginner(request);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo("Language added to user successfully!"));
        });
    }

    [Test]
    public async Task AddUserLanguageBeginner_ReturnsBadRequest_WhenNoJwtToken()
    {
        var request = new UserLanguageRequest(1);
        var mockHttpContext = new Mock<HttpContext>();
        var mockRequest = new Mock<HttpRequest>();
        var mockCookies = new Mock<IRequestCookieCollection>();
        mockCookies.Setup(c => c["jwt"]).Returns((string)null);
        mockRequest.Setup(r => r.Cookies).Returns(mockCookies.Object);
        mockHttpContext.Setup(h => h.Request).Returns(mockRequest.Object);
        _languageController.ControllerContext = new ControllerContext()
        {
            HttpContext = mockHttpContext.Object
        };
        var result = await _languageController.AddUserLanguageBeginner(request);
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task AddUserLanguageBeginner_ReturnsBadRequest_WhenInvalidJwtToken()
    {
        var request = new UserLanguageRequest(1);
        var invalidJwtToken = "invalid.jwt.token";
        var mockHttpContext = new Mock<HttpContext>();
        var mockRequest = new Mock<HttpRequest>();
        var mockCookies = new Mock<IRequestCookieCollection>();
        mockCookies.Setup(c => c["jwt"]).Returns(invalidJwtToken);
        mockRequest.Setup(r => r.Cookies).Returns(mockCookies.Object);
        mockHttpContext.Setup(h => h.Request).Returns(mockRequest.Object);
        _languageController.ControllerContext = new ControllerContext()
        {
            HttpContext = mockHttpContext.Object
        };
        var result = await _languageController.AddUserLanguageBeginner(request);
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }


    private string CreateJwtToken(string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = System.Text.Encoding.ASCII.GetBytes("a3c7e9d01b6f49ab12d60ce3fdd27b9a78a16e90c5473c482b43eb987d6225ce");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}