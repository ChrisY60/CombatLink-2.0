using Xunit;
using Moq;
using CombatLink.Domain.IRepositories;
using CombatLink.Domain.Models;
using CombatLink.Application.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace CombatLink.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IPasswordHasher<object>> _passwordHasherMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher<object>>();
            _userService = new UserService(_userRepoMock.Object, _passwordHasherMock.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnTrue_WhenUserRegistered()
        {
            _passwordHasherMock.Setup(x => x.HashPassword(null, "password")).Returns("hashedPassword");
            _userRepoMock.Setup(x => x.RegisterUserAsync("test@example.com", "hashedPassword")).ReturnsAsync(true);

            var result = await _userService.RegisterUserAsync("test@example.com", "password");

            Assert.True(result);
        }

        [Fact]
        public async Task LogInUserAsync_ShouldReturnUserId_WhenCredentialsAreValid()
        {
            _userRepoMock.Setup(x => x.GetPasswordHashByEmail("test@example.com")).ReturnsAsync("hashedPassword");
            _passwordHasherMock.Setup(x => x.VerifyHashedPassword(null, "hashedPassword", "password"))
                .Returns(PasswordVerificationResult.Success);
            _userRepoMock.Setup(x => x.GetUserIdByEmail("test@example.com")).ReturnsAsync(5);

            var result = await _userService.LogInUserAsync("test@example.com", "password");

            Assert.Equal(5, result);
        }

        [Fact]
        public async Task LogInUserAsync_ShouldReturnNull_WhenPasswordInvalid()
        {
            _userRepoMock.Setup(x => x.GetPasswordHashByEmail("test@example.com")).ReturnsAsync("hashedPassword");
            _passwordHasherMock.Setup(x => x.VerifyHashedPassword(null, "hashedPassword", "wrongpassword"))
                .Returns(PasswordVerificationResult.Failed);

            var result = await _userService.LogInUserAsync("test@example.com", "wrongpassword");

            Assert.Null(result);
        }

        [Fact]
        public async Task LogInUserAsync_ShouldReturnNull_WhenUserIdNotFound()
        {
            _userRepoMock.Setup(x => x.GetPasswordHashByEmail("test@example.com")).ReturnsAsync("hashedPassword");
            _passwordHasherMock.Setup(x => x.VerifyHashedPassword(null, "hashedPassword", "password"))
                .Returns(PasswordVerificationResult.Success);
            _userRepoMock.Setup(x => x.GetUserIdByEmail("test@example.com")).ReturnsAsync((int?)null);

            var result = await _userService.LogInUserAsync("test@example.com", "password");

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateUserProfile_ShouldReturnTrue_WhenUpdated()
        {
            _userRepoMock.Setup(x => x.UpdateUserProfile(1, "John", "Doe", It.IsAny<DateTime>(), 80, 195, 24, "url"))
                .ReturnsAsync(true);

            var result = await _userService.UpdateUserProfile(1, "John", "Doe", DateTime.UtcNow, 80, 195, 24, "url");

            Assert.True(result);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUser_WhenExists()
        {
            var user = new User { Id = 1, FirstName = "John", LastName = "Doe" };
            _userRepoMock.Setup(x => x.GetUserById(1)).ReturnsAsync(user);

            var result = await _userService.GetUserById(1);

            Assert.Equal(user, result);
        }
    }
}
