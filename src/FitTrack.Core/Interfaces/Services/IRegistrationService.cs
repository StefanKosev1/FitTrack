using FitTrack.Core.Results;

namespace FitTrack.Core.Interfaces.Services;

public interface IRegistrationService
{
    Task<AuthResult> RegisterAsync(string fullName, string email, string password);
}
