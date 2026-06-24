using FitTrack.Core.Services;
using FitTrack.Core.Tests.Fakes;

namespace FitTrack.Core.Tests.Services;

[TestClass]
public sealed class LoginServiceTests
{
    // Tests that a registered user can log in with the correct password.
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

    public async Task LoginAsync_WithCorrectName_ReturnsSuccess()
    {

    }

    // Tests that an incorrect password returns the generic login failure result.
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

    // Tests that an unknown email returns the generic login failure result.
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

    // Tests that invalid email formats are rejected before login lookup.
    [TestMethod]
    [DataRow("")]
    [DataRow("alexexample.com")]
    [DataRow("alex@")]
    public async Task LoginAsync_WithInvalidEmail_ReturnsFailure(string email)
    {
        // Arrange
        var repository = new FakeUserRepository();
        var service = new LoginService(repository);

        // Act
        var result = await service.LoginAsync(email, "Password123!");

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Invalid email or password.", result.ErrorMessage);
        Assert.IsNull(result.UserId);
    }

    // Tests that invalid password formats are rejected before password verification.
    [TestMethod]
    [DataRow("")]
    [DataRow("short")]
    [DataRow("        ")]
    public async Task LoginAsync_WithInvalidPassword_ReturnsFailure(string password)
    {
        // Arrange
        var repository = new FakeUserRepository();
        var service = new LoginService(repository);

        // Act
        var result = await service.LoginAsync("alex@example.com", password);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Invalid email or password.", result.ErrorMessage);
        Assert.IsNull(result.UserId);
    }

    // Tests that passwords longer than one hundred characters are rejected.
    [TestMethod]
    public async Task LoginAsync_WithPasswordLongerThan100Characters_ReturnsFailure()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var service = new LoginService(repository);

        // Act
        var result = await service.LoginAsync("alex@example.com", new string('a', 101));

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Invalid email or password.", result.ErrorMessage);
        Assert.IsNull(result.UserId);
    }
}
