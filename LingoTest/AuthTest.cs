using Backend.Controllers;
using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace LingoTest;

[TestFixture]
public class AuthControllerTest
{
    private Mock<IAuthService> _authServiceMock;
    private AuthController _authController;

    [SetUp]
    public void Setup()
    {
        _authServiceMock = new Mock<IAuthService>();
        _authController = new AuthController(_authServiceMock.Object);
        var httpContext = new DefaultHttpContext();
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };
        _authController.ControllerContext = controllerContext;
    }

    [Test]
    public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
    {
        var registrationRequest = new RegistrationRequest("test@test.com", "password123", "TestUser");
        var result = await _authController.Register(registrationRequest);
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Register_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        var registrationRequest = new RegistrationRequest("test@test.com", "password123", "TestUser");
        var exceptionMessage = "Database connection failed";
        _authServiceMock.Setup(x => x.RegisterAsync(
            registrationRequest.Email, 
            registrationRequest.Password, 
            registrationRequest.Username, 
            "User"))
            .ThrowsAsync(new Exception(exceptionMessage));
        var result = await _authController.Register(registrationRequest);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo(exceptionMessage));
        });
    }

    [Test]
    public async Task Authenticate_ReturnsOk_WhenLoginIsSuccessful()
    {
        var loginRequest = new LoginRequest("test@test.com", "password123");
        var successfulResult = new LoginResult 
        ( 
            true, 
             null,
            "sample-jwt-token"
        );
        _authServiceMock.Setup(x => x.LoginAsync(loginRequest.Email, loginRequest.Password))
            .ReturnsAsync(successfulResult);
        var result = await _authController.Authenticate(loginRequest);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo("sample-jwt-token"));
            var cookies = _authController.Response.Headers["Set-Cookie"];
            Assert.That(cookies.Any(c => c.Contains("jwt=sample-jwt-token")), Is.True);
        });
    }

    [Test]
    public async Task Authenticate_ReturnsBadRequest_WhenLoginFails()
    {
        var loginRequest = new LoginRequest("test@test.com", "wrongpassword");
        var failedResult = new LoginResult 
        (
            false, 
            "Invalid credentials",
            "token"
        );
        _authServiceMock.Setup(x => x.LoginAsync(loginRequest.Email, loginRequest.Password))
            .ReturnsAsync(failedResult);
        var result = await _authController.Authenticate(loginRequest);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public void CheckAuthentication_ReturnsOk_WhenJwtCookieExists()
    {
        _authController.Request.Cookies = new MockRequestCookieCollection(new Dictionary<string, string>
        {
            { "jwt", "token" }
        });
        var result = _authController.CheckAuthentication();
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo("Authenticated!"));
        });
    }

    [Test]
    public void CheckAuthentication_ReturnsUnauthorized_WhenJwtCookieDoesNotExist()
    {
        _authController.Request.Cookies = new MockRequestCookieCollection(new Dictionary<string, string>());
        var result = _authController.CheckAuthentication();
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.That(unauthorizedResult?.Value, Is.EqualTo("Not Authenticated!"));
        });
    }

    [Test]
    public void Logout_ReturnsOk_AndDeletesCookie()
    {
        var result = _authController.Logout();
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var responseObject = okResult?.Value as object;
            Assert.That(responseObject?.GetType().GetProperty("message")?.GetValue(responseObject), 
                Is.EqualTo("Logged out successfully"));
        });
    }
}

// To store cookies in memory while testing
public class MockRequestCookieCollection : IRequestCookieCollection
{
    private readonly Dictionary<string, string> _cookies;

    public MockRequestCookieCollection(Dictionary<string, string> cookies)
    {
        _cookies = cookies;
    }

    public string this[string key] => _cookies.TryGetValue(key, out var value) ? value : null;
    public int Count => _cookies.Count;
    public ICollection<string> Keys => _cookies.Keys;

    public bool ContainsKey(string key) => _cookies.ContainsKey(key);
    public bool TryGetValue(string key, out string value) => _cookies.TryGetValue(key, out value);
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _cookies.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}