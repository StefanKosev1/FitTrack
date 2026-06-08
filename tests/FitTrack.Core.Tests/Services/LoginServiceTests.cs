using FitTrack.Core.Services;
using FitTrack.Core.Tests.Fakes;

namespace FitTrack.Core.Tests.Services;

[TestClass]
public sealed class LoginServiceTests
{
    [TestMethod]
    public async Task LoginAsync_WithCorrectPassword_ReturnsSuccess()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var registrationService = new RegistrationService(repository);
        var loginService = new LoginService(repository);
        await registrationService.RegisterAsync("Alex Member", "alex@example.com", "Password123!");

        // Act
        var result = await loginService.LoginAsync(" alex@example.com ", "Password123!");

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.UserId);
        Assert.AreEqual("Alex Member", result.FullName);
        Assert.AreEqual("alex@example.com", result.Email);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task LoginAsync_WithWrongPassword_ReturnsFailure()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var registrationService = new RegistrationService(repository);
        var loginService = new LoginService(repository);
        await registrationService.RegisterAsync("Alex Member", "alex@example.com", "Password123!");

        // Act
        var result = await loginService.LoginAsync("alex@example.com", "WrongPassword123!");

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Invalid email or password.", result.ErrorMessage);
        Assert.IsNull(result.UserId);
    }

    [TestMethod]
    public async Task LoginAsync_WithUnknownEmail_ReturnsFailure()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var service = new LoginService(repository);

        // Act
        var result = await service.LoginAsync("missing@example.com", "Password123!");

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Invalid email or password.", result.ErrorMessage);
        Assert.IsNull(result.UserId);
    }
}
