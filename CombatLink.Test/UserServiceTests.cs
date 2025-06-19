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
        public async Task RegisterUserAsync_ShouldReturnTrue_AndCaptureEmailAndHashedPassword()
        {
            string? capturedEmail = null;
            string? capturedPasswordHash = null;

            _passwordHasherMock.Setup(x => x.HashPassword(null, "password")).Returns("hashedPassword");

            _userRepoMock
                .Setup(x => x.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((email, passwordHash) =>
                {
                    capturedEmail = email;
                    capturedPasswordHash = passwordHash;
                })
                .ReturnsAsync(true);

            var result = await _userService.RegisterUserAsync("test@example.com", "password");

            Assert.True(result);
            Assert.Equal("test@example.com", capturedEmail);
            Assert.Equal("hashedPassword", capturedPasswordHash);
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
        public async Task UpdateUserProfile_ShouldThrowException_WhenFirstNameIsNull()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.UpdateUserProfile(1, null, "Doe", DateTime.UtcNow, 80, 180, 10));
            Assert.Contains("First name", ex.Message);
        }

        [Fact]
        public async Task UpdateUserProfile_ShouldThrowException_WhenFirstNameIsTooLong()
        {
            string longName = new string('A', 51);
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.UpdateUserProfile(1, longName, "Doe", DateTime.UtcNow, 80, 180, 10));
            Assert.Contains("First name", ex.Message);
        }

        [Fact]
        public async Task UpdateUserProfile_ShouldThrowException_WhenLastNameIsEmpty()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.UpdateUserProfile(1, "John", "", DateTime.UtcNow, 80, 180, 10));
            Assert.Contains("Last name", ex.Message);
        }

        [Fact]
        public async Task UpdateUserProfile_ShouldThrowException_WhenLastNameIsTooLong()
        {
            string longName = new string('B', 60);
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.UpdateUserProfile(1, "John", longName, DateTime.UtcNow, 80, 180, 10));
            Assert.Contains("Last name", ex.Message);
        }

        [Fact]
        public async Task UpdateUserProfile_ShouldThrowException_WhenDateOfBirthInFuture()
        {
            var futureDate = DateTime.UtcNow.AddDays(1);
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.UpdateUserProfile(1, "John", "Doe", futureDate, 80, 180, 10));
            Assert.Contains("Date of birth", ex.Message);
        }

        [Fact]
        public async Task UpdateUserProfile_ShouldThrowException_WhenWeightTooLow()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.UpdateUserProfile(1, "John", "Doe", DateTime.UtcNow, 29, 180, 10));
            Assert.Contains("Weight", ex.Message);
        }

        [Fact]
        public async Task UpdateUserProfile_ShouldThrowException_WhenWeightTooHigh()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.UpdateUserProfile(1, "John", "Doe", DateTime.UtcNow, 201, 180, 10));
            Assert.Contains("Weight", ex.Message);
        }

        [Fact]
        public async Task UpdateUserProfile_ShouldThrowException_WhenHeightTooLow()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.UpdateUserProfile(1, "John", "Doe", DateTime.UtcNow, 80, 99, 10));
            Assert.Contains("Height", ex.Message);
        }

        [Fact]
        public async Task UpdateUserProfile_ShouldThrowException_WhenHeightTooHigh()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.UpdateUserProfile(1, "John", "Doe", DateTime.UtcNow, 80, 251, 10));
            Assert.Contains("Height", ex.Message);
        }

        [Fact]
        public async Task UpdateUserProfile_ShouldThrowException_WhenExperienceNegative()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.UpdateUserProfile(1, "John", "Doe", DateTime.UtcNow, 80, 180, -1));
            Assert.Contains("Experience", ex.Message);
        }

        [Fact]
        public async Task UpdateUserProfile_ShouldThrowException_WhenExperienceTooHigh()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.UpdateUserProfile(1, "John", "Doe", DateTime.UtcNow, 80, 180, 1001));
            Assert.Contains("Experience", ex.Message);
        }

        [Fact]
        public async Task UpdateUserProfile_ShouldReturnTrue_AndCaptureValues()
        {
            int? capturedId = null;
            string? capturedFirst = null;
            string? capturedLast = null;
            DateTime? capturedDob = null;
            decimal? capturedWeight = null;
            decimal? capturedHeight = null;
            int? capturedExperience = null;
            string? capturedPicUrl = null;

            _userRepoMock
                .Setup(x => x.UpdateUserProfile(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<int>(),
                    It.IsAny<string?>()))
                .Callback<int, string, string, DateTime, decimal, decimal, int, string?>(
                    (id, first, last, dob, w, h, exp, pic) =>
                    {
                        capturedId = id;
                        capturedFirst = first;
                        capturedLast = last;
                        capturedDob = dob;
                        capturedWeight = w;
                        capturedHeight = h;
                        capturedExperience = exp;
                        capturedPicUrl = pic;
                    })
                .ReturnsAsync(true);

            var dob = new DateTime(2000, 1, 1);
            var result = await _userService.UpdateUserProfile(1, "John", "Doe", dob, 80, 180, 10, "pic.jpg");

            Assert.True(result);
            Assert.Equal(1, capturedId);
            Assert.Equal("John", capturedFirst);
            Assert.Equal("Doe", capturedLast);
            Assert.Equal(dob, capturedDob);
            Assert.Equal(80, capturedWeight);
            Assert.Equal(180, capturedHeight);
            Assert.Equal(10, capturedExperience);
            Assert.Equal("pic.jpg", capturedPicUrl);
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
