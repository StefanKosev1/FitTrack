using FitTrack.Core.Interfaces.Repositories;
using FitTrack.Core.Interfaces.Services;
using FitTrack.Core.Results;

namespace FitTrack.Core.Services;

public class LoginService : ILoginService
{
    private readonly IUserRepository _userRepository;

    public LoginService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var normalizedEmail = email?.Trim();

        if (!AuthenticationInputValidator.IsValidEmail(normalizedEmail)
            || !AuthenticationInputValidator.IsValidPassword(password))
        {
            return InvalidCredentials();
        }

        var user = await _userRepository.GetByEmailAsync(normalizedEmail);

        if (user is null || !PasswordHasher.VerifyPassword(password, user.PasswordSalt, user.PasswordHash))
        {
            return InvalidCredentials();
        }

        return new AuthResult
        {
            IsSuccess = true,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email
        };
    }

    private static AuthResult InvalidCredentials()
    {
        return new AuthResult
        {
            IsSuccess = false,
            ErrorMessage = "Invalid email or password."
        };
    }
}
