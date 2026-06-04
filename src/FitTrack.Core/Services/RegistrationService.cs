using FitTrack.Core.Entities;
using FitTrack.Core.Interfaces.Repositories;
using FitTrack.Core.Interfaces.Services;
using FitTrack.Core.Results;

namespace FitTrack.Core.Services;

public class RegistrationService : IRegistrationService
{
    private readonly IUserRepository _userRepository;

    public RegistrationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
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

        var passwordHash = PasswordHasher.HashPassword(password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = fullName.Trim(),
            Email = normalizedEmail,
            PasswordSalt = passwordHash.Salt,
            PasswordHash = passwordHash.Hash,
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
}
