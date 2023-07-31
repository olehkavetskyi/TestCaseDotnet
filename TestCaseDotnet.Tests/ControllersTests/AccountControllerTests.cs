using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using TestCaseDotnet.Controllers;
using TestCaseDotnet.Dtos;
using TestCaseDotnet.Entities;
using TestCaseDotnet.Services;

namespace TestCaseDotnet.Tests.ControllersTests;

public class AccountControllerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly TokenService _tokenService;

    public AccountControllerTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        _signInManagerMock = new Mock<SignInManager<User>>(
            _userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(),
            null, null, null, null);

        _tokenService = Mock.Of<TokenService>();
    }


    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        string email = "test@example.com";
        string password = "wrong_password";
        _userManagerMock.Setup(m => m.FindByEmailAsync(email))
                        .ReturnsAsync(new User { Email = email });

        _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()))
                          .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _tokenService);

        // Act
        var loginDto = new LoginDto { Email = email, Password = password };
        var result = await controller.Login(loginDto);

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }


    [Fact]
    public async Task Register_EmailAlreadyExists_ReturnsBadRequest()
    {
        // Arrange
        string email = "test@example.com";
        string password = "password";
        _userManagerMock.Setup(m => m.FindByEmailAsync(email))
                        .ReturnsAsync(new User { Email = email });

        var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _tokenService);

        // Act
        var registerDto = new RegisterDto { Email = email, Password = password };
        var result = await controller.Register(registerDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
