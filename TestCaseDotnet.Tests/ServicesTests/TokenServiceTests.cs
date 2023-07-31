using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.Text;
using TestCaseDotnet.Services;

namespace TestCaseDotnet.Tests.ServicesTests;

public class TokenServiceTests
{
    [Fact]
    public void GenerateJwtToken_ShouldReturnValidToken()
    {
        // Arrange
        var username = "testuser";
        var config = new Mock<IConfiguration>();
        config.Setup(x => x["Token:Key"]).Returns("super_secret_key");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Object["Token:Key"]));
        var tokenService = new TokenService(config.Object, key);

        // Act
        var token = tokenService.GenerateJwtToken(username);

        // Assert
        Assert.NotNull(token);
        Assert.True(token.Length > 0);
    }

}

