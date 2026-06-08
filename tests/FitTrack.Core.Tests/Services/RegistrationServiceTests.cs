using FitTrack.Core.Services;
using FitTrack.Core.Tests.Fakes;

namespace FitTrack.Core.Tests.Services;

[TestClass]
public sealed class RegistrationServiceTests
{
    [TestMethod]
    public async Task RegisterAsync_WithNewEmail_CreatesUserAndReturnsSuccess()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var service = new RegistrationService(repository);

        // Act
        var result = await service.RegisterAsync(
            " Alex Member ",
            " alex@example.com ",
            "Password123!");

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.UserId);
        Assert.AreEqual("Alex Member", result.FullName);
        Assert.AreEqual("alex@example.com", result.Email);
        Assert.IsNull(result.ErrorMessage);

        var savedUser = await repository.GetByEmailAsync("alex@example.com");
        Assert.IsNotNull(savedUser);
        Assert.AreNotEqual("Password123!", savedUser.PasswordHash);
        Assert.IsFalse(string.IsNullOrWhiteSpace(savedUser.PasswordSalt));
    }

    [TestMethod]
    public async Task RegisterAsync_WithExistingEmail_ReturnsFailure()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var service = new RegistrationService(repository);
        await service.RegisterAsync("Alex Member", "alex@example.com", "Password123!");

        // Act
        var result = await service.RegisterAsync(
            "Other Member",
            " alex@example.com ",
            "Different123!");

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("An account with this email already exists.", result.ErrorMessage);
        Assert.IsNull(result.UserId);
    }
}
