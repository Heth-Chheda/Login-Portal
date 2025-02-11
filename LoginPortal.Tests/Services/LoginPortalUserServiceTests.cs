using LoginPortal.Data;
using LoginPortal.Models;
using LoginPortal.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace LoginPortal.Tests.Services
{
    public class LoginPortalUserServiceTests
    {
        private static ApplicationDbContext CreateInMemoryContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            return new ApplicationDbContext(options);
        }

        private static Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null
            );
        }

        [Fact]
        public async Task CheckIfUserExistsAsync_ReturnsTrue_WhenUserExists()
        {
            // Arrange
            var databaseName = "CheckUserExistsTestDb";
            using var context = CreateInMemoryContext(databaseName);
            var mockUserManager = MockUserManager();
            var userService = new UserService(context, mockUserManager.Object);

            // Seed data
            context.UserDetails.Add(new User_Details
            {
                UserId = 1,
                Email = "test@example.com",
                HashedPassword = "oldpassword"
            });
            await context.SaveChangesAsync();

            // Act
            var result = await userService.CheckIfUserExistsAsync("test@example.com");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CheckIfUserExistsAsync_ReturnsFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var databaseName = "UserDoesNotExistTestDb";
            using var context = CreateInMemoryContext(databaseName);
            var mockUserManager = MockUserManager();
            var userService = new UserService(context, mockUserManager.Object);

            // Act
            var result = await userService.CheckIfUserExistsAsync("nonexistent@example.com");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddUserAsync_AddsUserToDatabase()
        {
            // Arrange
            var databaseName = "AddUserTestDb";
            using var context = CreateInMemoryContext(databaseName);
            var mockUserManager = MockUserManager();
            var userService = new UserService(context, mockUserManager.Object);

            var newUser = new User_Details
            {
                UserId = 2,
                Email = "newuser@example.com",
                HashedPassword = "newpassword"
            };

            // Act
            await userService.AddUserAsync(newUser);

            // Assert
            var userInDb = await context.UserDetails.FindAsync(2);
            Assert.NotNull(userInDb);
            Assert.Equal("newuser@example.com", userInDb.Email);
        }

        [Fact]
        public async Task UpdatePasswordAsync_UpdatesUserPassword_WhenUserExists()
        {
            // Arrange
            var databaseName = "UpdatePasswordTestDb";
            using var context = CreateInMemoryContext(databaseName);
            var mockUserManager = MockUserManager();
            var userService = new UserService(context, mockUserManager.Object);

            // Seed data
            context.UserDetails.Add(new User_Details
            {
                UserId = 1,
                Email = "test@example.com",
                HashedPassword = "oldpassword"
            });
            await context.SaveChangesAsync();

            // Act
            var updatedUser = await userService.UpdatePasswordAsync("test@example.com", "newpassword");

            // Assert
            Assert.NotNull(updatedUser);
            Assert.Equal("newpassword", updatedUser?.HashedPassword);
        }

        [Fact]
        public async Task UpdatePasswordAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var databaseName = "PasswordNotFoundTestDb";
            using var context = CreateInMemoryContext(databaseName);
            var mockUserManager = MockUserManager();
            var userService = new UserService(context, mockUserManager.Object);

            // Act
            var updatedUser = await userService.UpdatePasswordAsync("nonexistent@example.com", "newpassword");

            // Assert
            Assert.Null(updatedUser);
        }

        [Fact]
        public async Task DeleteUserAsync_RemovesUserFromDatabase_WhenUserExists()
        {
            // Arrange
            var databaseName = "DeleteUserTestDb";
            using var context = CreateInMemoryContext(databaseName);
            var mockUserManager = MockUserManager();
            var userService = new UserService(context, mockUserManager.Object);

            // Seed data
            context.UserDetails.Add(new User_Details
            {
                UserId = 1,
                Email = "test@example.com",
                HashedPassword = "oldpassword"
            });
            await context.SaveChangesAsync();

            // Act
            var result = await userService.DeleteUserAsync(1);

            // Assert
            Assert.True(result);
            var userInDb = await context.UserDetails.FindAsync(1);
            Assert.Null(userInDb);
        }

        [Fact]
        public async Task DeleteUserAsync_ReturnsFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var databaseName = "DeleteUserNotFoundTestDb";
            using var context = CreateInMemoryContext(databaseName);
            var mockUserManager = MockUserManager();
            var userService = new UserService(context, mockUserManager.Object);

            // Act
            var result = await userService.DeleteUserAsync(99);

            // Assert
            Assert.False(result);
        }
    }
}
