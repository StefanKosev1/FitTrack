using System.Security.Cryptography;
using FitTrack.Core.Entities;
using FitTrack.Core.Interfaces.Repositories;
using FitTrack.Core.Interfaces.Services;
using FitTrack.Core.Results;

namespace FitTrack.Core.Services;

public class AuthService : IAuthService
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var normalizedEmail = email.Trim();
        var user = await _userRepository.GetByEmailAsync(normalizedEmail);

        if (user is null || !VerifyPassword(password, user.PasswordSalt, user.PasswordHash))
        {
            return new AuthResult
            {
                IsSuccess = false,
                ErrorMessage = "Invalid email or password."
            };
        }

        return new AuthResult
        {
            IsSuccess = true,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email
        };
    }

    public async Task<AuthResult> RegisterAsync(string fullName, string email, string password)
    {
        var normalizedEmail = email.Trim();
        var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail);

        if (existingUser is not null)
        {
            return new AuthResult
            {
                IsSuccess = false,
                ErrorMessage = "An account with this email already exists."
            };
        }

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = HashPassword(password, salt);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = fullName.Trim(),
            Email = normalizedEmail,
            PasswordSalt = Convert.ToBase64String(salt),
            PasswordHash = Convert.ToBase64String(hash),
            CreatedAtUtc = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);

        return new AuthResult
        {
            IsSuccess = true,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email
        };
    }

    private static byte[] HashPassword(string password, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);
    }

    private static bool VerifyPassword(string password, string encodedSalt, string encodedHash)
    {
        var salt = Convert.FromBase64String(encodedSalt);
        var storedHash = Convert.FromBase64String(encodedHash);
        var computedHash = HashPassword(password, salt);

        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
    }
}
