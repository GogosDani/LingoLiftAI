using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Models;
using Backend.Services;
using Moq;

[TestFixture]
public class TokenServiceTest
{
    private Mock<Microsoft.Extensions.Configuration.IConfiguration> _configurationMock;
    private TokenService _tokenService;
    private ApplicationUser _testUser;

    [SetUp]
    public void Setup()
    {
        _configurationMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        _configurationMock.Setup(c => c["ValidIssuer"]).Returns("TestIssuer");
        _configurationMock.Setup(c => c["ValidAudience"]).Returns("TestAudience");
        _configurationMock.Setup(c => c["JwtSecretKey"]).Returns("test-secret-key-that-is-at-least-256-bits-long-for-hmac-sha256");
        
        _tokenService = new TokenService(_configurationMock.Object);
        _testUser = new ApplicationUser
        {
            Id = "test-user",
            UserName = "testuser",
            Email = "test@test.com"
        };
    }

    [Test]
    public void CreateToken_ReturnsValidJwtToken_WhenValidUserAndRole()
    {
        var role = "User";
        var token = _tokenService.CreateToken(_testUser, role);
        Assert.Multiple(() =>
        {
            Assert.That(token, Is.Not.Null);
            Assert.That(token, Is.Not.Empty);
            Assert.That(token.Count(c => c == '.'), Is.EqualTo(2));
        });
    }

    [Test]
    public void CreateToken_ReturnsValidJwtToken_WhenValidUserAndNullRole()
    {
        string role = null;
        var token = _tokenService.CreateToken(_testUser, role);
        Assert.Multiple(() =>
        {
            Assert.That(token, Is.Not.Null);
            Assert.That(token, Is.Not.Empty);
            Assert.That(token.Count(c => c == '.'), Is.EqualTo(2));
        });
    }

    [Test]
    public void CreateToken_TokenContainsExpectedClaims_WhenValidUserAndRole()
    {
        var role = "Admin";
        var token = _tokenService.CreateToken(_testUser, role);
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        Assert.Multiple(() =>
        {
            Assert.That(jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value, 
                Is.EqualTo(_testUser.Id));
            Assert.That(jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value, 
                Is.EqualTo(_testUser.UserName));
            Assert.That(jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value, 
                Is.EqualTo(_testUser.Email));
            Assert.That(jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value, 
                Is.EqualTo(role));
            Assert.That(jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value, 
                Is.EqualTo(_testUser.Id));
            Assert.That(jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value, 
                Is.Not.Null);
            Assert.That(jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iat)?.Value, 
                Is.Not.Null);
        });
    }

    [Test]
    public void CreateToken_TokenDoesNotContainRoleClaim_WhenRoleIsNull()
    {
        string role = null;
        var token = _tokenService.CreateToken(_testUser, role);
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        Assert.That(jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role), Is.Null);
    }

    [Test]
    public void CreateToken_TokenHasCorrectIssuerAndAudience()
    {
        var role = "User";
        var token = _tokenService.CreateToken(_testUser, role);
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        Assert.Multiple(() =>
        {
            Assert.That(jwtToken.Issuer, Is.EqualTo("TestIssuer"));
            Assert.That(jwtToken.Audiences.First(), Is.EqualTo("TestAudience"));
        });
    }

    [Test]
    public void CreateToken_GeneratesUniqueJtiClaim_ForMultipleTokens()
    {
        var role = "User";
        var token1 = _tokenService.CreateToken(_testUser, role);
        var token2 = _tokenService.CreateToken(_testUser, role);
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken1 = tokenHandler.ReadJwtToken(token1);
        var jwtToken2 = tokenHandler.ReadJwtToken(token2);
        var jti1 = jwtToken1.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
        var jti2 = jwtToken2.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
        Assert.Multiple(() =>
        {
            Assert.That(jti1, Is.Not.Null);
            Assert.That(jti2, Is.Not.Null);
            Assert.That(jti1, Is.Not.EqualTo(jti2));
        });
    }

    [Test]
    public void CreateToken_ThrowsException_WhenUserIsNull()
    {
        ApplicationUser nullUser = null;
        var role = "User";
        Assert.Throws<NullReferenceException>(() => _tokenService.CreateToken(nullUser, role));
    }

    [Test]
    public void CreateToken_ThrowsException_WhenJwtSecretKeyIsMissing()
    {
        var configMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        configMock.Setup(c => c["ValidIssuer"]).Returns("TestIssuer");
        configMock.Setup(c => c["ValidAudience"]).Returns("TestAudience");
        configMock.Setup(c => c["JwtSecretKey"]).Returns((string)null);
        var tokenService = new TokenService(configMock.Object);
        var role = "User";
        Assert.Throws<ArgumentNullException>(() => tokenService.CreateToken(_testUser, role));
    }

    [Test]
    public void CreateToken_UsesHmacSha256Algorithm()
    {
        var role = "User";
        var token = _tokenService.CreateToken(_testUser, role);
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        Assert.That(jwtToken.Header.Alg, Is.EqualTo("HS256"));
    }

    [Test]
    public void CreateToken_IatClaimIsCorrectFormat()
    {
        var role = "User";
        var token = _tokenService.CreateToken(_testUser, role);
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var iatClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iat);
        Assert.Multiple(() =>
        {
            Assert.That(iatClaim, Is.Not.Null);
            Assert.That(iatClaim.ValueType, Is.EqualTo(ClaimValueTypes.Integer64));
            Assert.That(long.TryParse(iatClaim.Value, out _), Is.True);
        });
    }

    [Test]
    public void CreateToken_ConfigurationIsAccessedCorrectly()
    {
        var role = "User";
        _tokenService.CreateToken(_testUser, role);
        _configurationMock.Verify(c => c["ValidIssuer"], Times.Once);
        _configurationMock.Verify(c => c["ValidAudience"], Times.Once);
        _configurationMock.Verify(c => c["JwtSecretKey"], Times.Once);
    }
}