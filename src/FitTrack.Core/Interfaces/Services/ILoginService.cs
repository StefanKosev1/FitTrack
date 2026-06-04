using FitTrack.Core.Results;

namespace FitTrack.Core.Interfaces.Services;

public interface ILoginService
{
    Task<AuthResult> LoginAsync(string email, string password);
}
