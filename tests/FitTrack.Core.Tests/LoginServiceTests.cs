using FitTrack.Core.Services;
using Xunit;

namespace FitTrack.Core.Tests;

public class LoginServiceTests
{
    [Fact]
    public async Task LoginAsync_WithCorrectPassword_ReturnsSuccess()
    {
        var repository = new FakeUserRepository();
        var registrationService = new RegistrationService(repository);
        var loginService = new LoginService(repository);
        await registrationService.RegisterAsync("Alex Member", "alex@example.com", "Password123!");

        var result = await loginService.LoginAsync(" alex@example.com ", "Password123!");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.UserId);
        Assert.Equal("Alex Member", result.FullName);
        Assert.Equal("alex@example.com", result.Email);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsFailure()
    {
        var repository = new FakeUserRepository();
        var registrationService = new RegistrationService(repository);
        var loginService = new LoginService(repository);
        await registrationService.RegisterAsync("Alex Member", "alex@example.com", "Password123!");

        var result = await loginService.LoginAsync("alex@example.com", "WrongPassword123!");

        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid email or password.", result.ErrorMessage);
        Assert.Null(result.UserId);
    }

    [Fact]
    public async Task LoginAsync_WithUnknownEmail_ReturnsFailure()
    {
        var repository = new FakeUserRepository();
        var loginService = new LoginService(repository);

        var result = await loginService.LoginAsync("missing@example.com", "Password123!");

        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid email or password.", result.ErrorMessage);
        Assert.Null(result.UserId);
    }
}
