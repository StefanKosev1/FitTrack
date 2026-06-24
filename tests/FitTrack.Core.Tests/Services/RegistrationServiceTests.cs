using FitTrack.Core.Services;
using FitTrack.Core.Tests.Fakes;

namespace FitTrack.Core.Tests.Services;

[TestClass]
public sealed class RegistrationServiceTests
{
    // Tests that valid registration details return a successful authentication result.
    [TestMethod]
    public async Task RegisterAsync_WithValidDetails_ReturnsSuccessfulResult()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var service = new RegistrationService(repository);

        // Act
        var result = await service.RegisterAsync(
            "Alex Member",
            "alex@example.com",
            "Password123!");

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.UserId);
        Assert.IsNull(result.ErrorMessage);
    }

    // Tests that leading and trailing spaces are removed from saved user details.
    [TestMethod]
    public async Task RegisterAsync_WithWhitespaceAroundDetails_ReturnsNormalizedDetails()
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
        Assert.AreEqual("Alex Member", result.FullName);
        Assert.AreEqual("alex@example.com", result.Email);
    }

    // Tests that registration stores a password hash and salt instead of the plain password.
    [TestMethod]
    public async Task RegisterAsync_WithValidDetails_PersistsHashedPassword()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var service = new RegistrationService(repository);

        // Act
        await service.RegisterAsync(
            "Alex Member",
            "alex@example.com",
            "Password123!");

        // Assert
        var savedUser = await repository.GetByEmailAsync("alex@example.com");
        Assert.IsNotNull(savedUser);
        Assert.AreNotEqual("Password123!", savedUser.PasswordHash);
        Assert.IsFalse(string.IsNullOrWhiteSpace(savedUser.PasswordSalt));
    }

    // Tests that registering with an email that already exists returns a failure result.
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

    // Tests that a duplicate registration does not overwrite the existing user.
    [TestMethod]
    public async Task RegisterAsync_WithExistingEmail_DoesNotReplaceExistingUser()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var service = new RegistrationService(repository);
        await service.RegisterAsync("Alex Member", "alex@example.com", "Password123!");

        // Act
        await service.RegisterAsync("Other Member", "alex@example.com", "Different123!");

        // Assert
        var savedUser = await repository.GetByEmailAsync("alex@example.com");
        Assert.IsNotNull(savedUser);
        Assert.AreEqual("Alex Member", savedUser.FullName);
    }

    // Tests that an empty email is rejected by registration validation.
    [TestMethod]
    public async Task RegisterAsync_WithEmptyEmail_ReturnsValidationFailure()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var service = new RegistrationService(repository);

        // Act
        var result = await service.RegisterAsync("Alex Member", "", "Password123!");

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("A valid email address is required.", result.ErrorMessage);
        Assert.IsNull(result.UserId);
    }

    // Tests that an email without an at sign is rejected by registration validation.
    [TestMethod]
    public async Task RegisterAsync_WithEmailMissingAtSign_ReturnsValidationFailure()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var service = new RegistrationService(repository);

        // Act
        var result = await service.RegisterAsync("Alex Member", "alexexample.com", "Password123!");

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("A valid email address is required.", result.ErrorMessage);
        Assert.IsNull(result.UserId);
    }

    // Tests that an email without a domain is rejected by registration validation.
    [TestMethod]
    public async Task RegisterAsync_WithEmailMissingDomain_ReturnsValidationFailure()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var service = new RegistrationService(repository);

        // Act
        var result = await service.RegisterAsync("Alex Member", "alex@", "Password123!");

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("A valid email address is required.", result.ErrorMessage);
        Assert.IsNull(result.UserId);
    }

    // Tests that invalid email input does not create a user record.
    [TestMethod]
    public async Task RegisterAsync_WithInvalidEmail_DoesNotCreateUser()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var service = new RegistrationService(repository);

        // Act
        await service.RegisterAsync("Alex Member", "alexexample.com", "Password123!");

        // Assert
        Assert.IsNull(await repository.GetByEmailAsync("alexexample.com"));
    }

    // Tests that an empty password is rejected by registration validation.
    [TestMethod]
    public async Task RegisterAsync_WithEmptyPassword_ReturnsValidationFailure()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var service = new RegistrationService(repository);

        // Act
        var result = await service.RegisterAsync("Alex Member", "alex@example.com", "");

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Password must be between 8 and 100 characters.", result.ErrorMessage);
        Assert.IsNull(result.UserId);
    }

    // Tests that passwords shorter than eight characters are rejected.
    [TestMethod]
    public async Task RegisterAsync_WithPasswordShorterThan8Characters_ReturnsValidationFailure()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var service = new RegistrationService(repository);

        // Act
        var result = await service.RegisterAsync("Alex Member", "alex@example.com", "short");

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Password must be between 8 and 100 characters.", result.ErrorMessage);
        Assert.IsNull(result.UserId);
    }

    // Tests that a whitespace-only password is rejected by registration validation.
    [TestMethod]
    public async Task RegisterAsync_WithWhitespacePassword_ReturnsValidationFailure()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var service = new RegistrationService(repository);

        // Act
        var result = await service.RegisterAsync("Alex Member", "alex@example.com", "        ");

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Password must be between 8 and 100 characters.", result.ErrorMessage);
        Assert.IsNull(result.UserId);
    }

    // Tests that passwords longer than one hundred characters are rejected.
    [TestMethod]
    public async Task RegisterAsync_WithPasswordLongerThan100Characters_ReturnsValidationFailure()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var service = new RegistrationService(repository);

        // Act
        var result = await service.RegisterAsync(
            "Alex Member",
            "alex@example.com",
            new string('a', 101));

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Password must be between 8 and 100 characters.", result.ErrorMessage);
        Assert.IsNull(result.UserId);
    }

    // Tests that invalid password input does not create a user record.
    [TestMethod]
    public async Task RegisterAsync_WithInvalidPassword_DoesNotCreateUser()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var service = new RegistrationService(repository);

        // Act
        await service.RegisterAsync("Alex Member", "alex@example.com", "short");

        // Assert
        Assert.IsNull(await repository.GetByEmailAsync("alex@example.com"));
    }
}
