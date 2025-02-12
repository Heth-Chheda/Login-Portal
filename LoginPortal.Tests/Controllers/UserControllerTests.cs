using Microsoft.EntityFrameworkCore;
using LoginPortal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using LoginPortal.Controllers;
using LoginPortal.Services;
using Microsoft.AspNetCore.Identity.Data;
using LoginPortal.Data;
using Microsoft.AspNetCore.Identity;

namespace LoginPortal.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<ApplicationDbContext> _mockDbContext;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<EmailService> _mockEmailService;
        private readonly Mock<UserService> _mockUserService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            // Mocking the necessary services
            _mockDbContext = new Mock<ApplicationDbContext>(MockBehavior.Loose, new DbContextOptions<ApplicationDbContext>());
            _mockUserService = new Mock<UserService>(_mockDbContext.Object, null);

            // Mocking DbSets for RoleClaims, UserDetails, etc.
            var mockRoleClaimsDbSet = new Mock<DbSet<IdentityRoleClaim<string>>>();
            var mockUserDetailsDbSet = new Mock<DbSet<User_Details>>();

            // Setup the RoleClaims DbSet to return an IQueryable (for querying)
            mockRoleClaimsDbSet.As<IQueryable<IdentityRoleClaim<string>>>()
                               .Setup(m => m.Provider).Returns(new List<IdentityRoleClaim<string>>().AsQueryable().Provider);
            mockRoleClaimsDbSet.As<IQueryable<IdentityRoleClaim<string>>>()
                               .Setup(m => m.Expression).Returns(new List<IdentityRoleClaim<string>>().AsQueryable().Expression);
            mockRoleClaimsDbSet.As<IQueryable<IdentityRoleClaim<string>>>()
                               .Setup(m => m.ElementType).Returns(new List<IdentityRoleClaim<string>>().AsQueryable().ElementType);
            mockRoleClaimsDbSet.As<IQueryable<IdentityRoleClaim<string>>>()
                               .Setup(m => m.GetEnumerator()).Returns(new List<IdentityRoleClaim<string>>().AsQueryable().GetEnumerator());

            // Setup the UserDetails DbSet similarly
            mockUserDetailsDbSet.As<IQueryable<User_Details>>()
                               .Setup(m => m.Provider).Returns(new List<User_Details>().AsQueryable().Provider);
            mockUserDetailsDbSet.As<IQueryable<User_Details>>()
                               .Setup(m => m.Expression).Returns(new List<User_Details>().AsQueryable().Expression);
            mockUserDetailsDbSet.As<IQueryable<User_Details>>()
                               .Setup(m => m.ElementType).Returns(new List<User_Details>().AsQueryable().ElementType);
            mockUserDetailsDbSet.As<IQueryable<User_Details>>()
                               .Setup(m => m.GetEnumerator()).Returns(new List<User_Details>().AsQueryable().GetEnumerator());

            // Setting up the mock DbContext to return the mocked DbSets
            _mockDbContext.Setup(db => db.Set<IdentityRoleClaim<string>>()).Returns(mockRoleClaimsDbSet.Object);
            _mockDbContext.Setup(db => db.Set<User_Details>()).Returns(mockUserDetailsDbSet.Object);

            // Mocking the UserManager<ApplicationUser>
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object,
                null, null, null, null, null, null, null, null
            );

            // Mocking the EmailService
            _mockEmailService = new Mock<EmailService>();

            // Create the UserService with mocked dependencies
            var userService = new UserService(_mockDbContext.Object, _mockUserManager.Object);

            // Creating the controller with mocked services
            _controller = new UserController(userService, _mockEmailService.Object);
        }

        [Fact]
        public async Task ResetPassword_ReturnsBadRequest_WhenTokenIsInvalid()
        {
            // Arrange
            var resetPasswordRequest = new LoginPortal.Models.ResetPasswordRequest
            {
                Email = "test@example.com",
                ResetCode = "invalid-token",
                NewPassword = "newpassword123"
            };

            // Mocking the ResetPasswordAsync method to return false (invalid token)
            var mockUserService = new Mock<UserService>(_mockDbContext.Object, _mockUserManager.Object);
            mockUserService.Setup(service => service.ResetPasswordAsync(resetPasswordRequest.Email, resetPasswordRequest.ResetCode, resetPasswordRequest.NewPassword)).ReturnsAsync(false);

            var controller = new UserController(mockUserService.Object, _mockEmailService.Object);

            // Act
            var result = await controller.ResetPassword(resetPasswordRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
    }
}