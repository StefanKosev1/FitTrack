namespace FitTrack.Core.Results;

public class AuthResult
{
    public bool IsSuccess { get; set; }

    public Guid? UserId { get; set; }

    public string? ErrorMessage { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }
}
