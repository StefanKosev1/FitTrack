using System.Security.Cryptography;

namespace FitTrack.Core.Services;

internal static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    public static (string Salt, string Hash) HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = HashPassword(password, salt);

        return (Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }

    public static bool VerifyPassword(string password, string encodedSalt, string encodedHash)
    {
        var salt = Convert.FromBase64String(encodedSalt);
        var storedHash = Convert.FromBase64String(encodedHash);
        var computedHash = HashPassword(password, salt);

        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
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
}
