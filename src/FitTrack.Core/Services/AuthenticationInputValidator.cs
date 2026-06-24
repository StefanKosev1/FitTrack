using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FitTrack.Core.Services;

internal static class AuthenticationInputValidator
{
    public const int MinimumPasswordLength = 8;
    public const int MaximumPasswordLength = 100;

    private static readonly EmailAddressAttribute EmailValidator = new();

    public static bool IsValidEmail([NotNullWhen(true)] string? email)
    {
        return !string.IsNullOrWhiteSpace(email) && EmailValidator.IsValid(email);
    }

    public static bool IsValidPassword([NotNullWhen(true)] string? password)
    {
        return !string.IsNullOrWhiteSpace(password)
            && password.Length is >= MinimumPasswordLength and <= MaximumPasswordLength;
    }
}
