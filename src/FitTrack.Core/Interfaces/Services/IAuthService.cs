using FitTrack.Core.Results;

namespace FitTrack.Core.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password);

    Task<AuthResult> RegisterAsync(string fullName, string email, string password);
}
