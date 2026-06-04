using FitTrack.Core.Services;
using Xunit;

namespace FitTrack.Core.Tests;

public class RegistrationServiceTests
{
    [Fact]
    public async Task RegisterAsync_WithNewEmail_CreatesUserAndReturnsSuccess()
    {
        var repository = new FakeUserRepository();
        var service = new RegistrationService(repository);

        var result = await service.RegisterAsync(" Alex Member ", " alex@example.com ", "Password123!");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.UserId);
        Assert.Equal("Alex Member", result.FullName);
        Assert.Equal("alex@example.com", result.Email);
        Assert.Null(result.ErrorMessage);

        var savedUser = await repository.GetByEmailAsync("alex@example.com");
        Assert.NotNull(savedUser);
        Assert.NotEqual("Password123!", savedUser.PasswordHash);
        Assert.False(string.IsNullOrWhiteSpace(savedUser.PasswordSalt));
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ReturnsFailure()
    {
        var repository = new FakeUserRepository();
        var service = new RegistrationService(repository);
        await service.RegisterAsync("Alex Member", "alex@example.com", "Password123!");

        var result = await service.RegisterAsync("Other Member", " alex@example.com ", "Different123!");

        Assert.False(result.IsSuccess);
        Assert.Equal("An account with this email already exists.", result.ErrorMessage);
        Assert.Null(result.UserId);
    }
}
