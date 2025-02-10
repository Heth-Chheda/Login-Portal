using LoginPortal.Controllers;
using LoginPortal.Data;
using LoginPortal.Models;
using LoginPortal.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

public class AuthControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IPasswordService> _mockPasswordService;
    private readonly Mock<ApplicationDbContext> _mockDbContext;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly TokenService _mockTokenService;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        // Mock UserManager dependency
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null
        );

        // Initialize other mocked dependencies
        _mockUserService = new Mock<IUserService>();
        _mockPasswordService = new Mock<IPasswordService>();
        _mockDbContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
        _mockConfiguration = new Mock<IConfiguration>();

        // Instantiate TokenService with mocked dependencies
        _mockTokenService = new TokenService(
            _mockDbContext.Object,
            _mockConfiguration.Object,
            _mockUserService.Object,
            _mockUserManager.Object
        );

        // Initialize the AuthController
        _authController = new AuthController(
            _mockUserService.Object,
            _mockPasswordService.Object,
            _mockTokenService
        );
    }

    [Fact]
    public async Task Register_EmailAlreadyTaken_ReturnsBadRequest()
    {
        // Arrange
        var registrationRequest = new RegistrationRequest { Email = "test@example.com", Password = "password123" };
        _mockUserService.Setup(s => s.IsEmailTakenAsync(registrationRequest.Email)).ReturnsAsync(true);

        // Act
        var result = await _authController.Register(registrationRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<RegistrationResponse>(badRequestResult.Value);
        Assert.Equal(ErrorCode.InvalidEmail, response.ErrorCode);
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsOk()
    {
        // Arrange
        var registrationRequest = new RegistrationRequest { Email = "test@example.com", Password = "password123" };
        _mockUserService.Setup(s => s.IsEmailTakenAsync(registrationRequest.Email)).ReturnsAsync(false);

        // Act
        var result = await _authController.Register(registrationRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<RegistrationResponse>(okResult.Value);
        Assert.Equal(ErrorCode.Success, response.ErrorCode);
    }

    [Fact]
    public async Task Login_InvalidEmail_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest { Email = "invalid@example.com", Password = "password123" };
        _mockUserService.Setup(s => s.GetUserByEmailAsync(loginRequest.Email)).ReturnsAsync((User_Details?)null);

        // Act
        var result = await _authController.Login(loginRequest);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(unauthorizedResult.Value);
        Assert.Equal(ErrorCode.InvalidEmail, response.ErrorCode);
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest { Email = "test@example.com", Password = "wrongpassword" };
        var user = new User_Details { Email = "test@example.com", HashedPassword = "hashedpassword" };

        _mockUserService.Setup(s => s.GetUserByEmailAsync(loginRequest.Email)).ReturnsAsync(user);
        _mockPasswordService.Setup(s => s.VerifyPassword(loginRequest.Password, user.HashedPassword)).Returns(false);

        // Act
        var result = await _authController.Login(loginRequest);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(unauthorizedResult.Value);
        Assert.Equal(ErrorCode.InvalidPassword, response.ErrorCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOk()
    {
        // Arrange
        var loginRequest = new LoginRequest { Email = "test@example.com", Password = "password123" };
        var user = new User_Details { Email = "test@example.com", HashedPassword = "hashedpassword" };

        _mockUserService.Setup(s => s.GetUserByEmailAsync(loginRequest.Email)).ReturnsAsync(user);
        _mockPasswordService.Setup(s => s.VerifyPassword(loginRequest.Password, user.HashedPassword)).Returns(true);

        // Act
        var result = await _authController.Login(loginRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.Equal(ErrorCode.Success, response.ErrorCode);
    }
}
