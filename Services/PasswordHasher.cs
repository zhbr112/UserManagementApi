using System.Security.Cryptography;

namespace UserManagementApi;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 10000;

    public string HashPassword(string password)
    {
        using var algorithm = new Rfc2898DeriveBytes(
            password,
            SaltSize,
            Iterations,
            HashAlgorithmName.SHA256);

        var salt = Convert.ToBase64String(algorithm.Salt);
        var hash = Convert.ToBase64String(algorithm.GetBytes(HashSize));

        return $"{Iterations}.{salt}.{hash}";
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var parts = hashedPassword.Split('.', 3);
        if (parts.Length != 3)
            return false;

        var iterations = int.Parse(parts[0]);
        var salt = Convert.FromBase64String(parts[1]);
        var hash = Convert.FromBase64String(parts[2]);

        using var algorithm = new Rfc2898DeriveBytes(
            providedPassword,
            salt,
            iterations,
            HashAlgorithmName.SHA256);

        var newHash = algorithm.GetBytes(HashSize);
        return CryptographicOperations.FixedTimeEquals(hash, newHash);
    }
}