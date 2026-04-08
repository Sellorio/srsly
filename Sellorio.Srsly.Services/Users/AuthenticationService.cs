using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sellorio.Results;
using Sellorio.Results.Messages;
using Sellorio.Srsly.Data;
using Sellorio.Srsly.Models.Users;

namespace Sellorio.Srsly.Services.Users;

public class AuthenticationService(DatabaseContext databaseContext) : IAuthenticationService
{
    private const string PasswordHashAlgorithm = "PBKDF2-SHA512-V1";
    private const int MinimumIterations = 100_000;
    private const int SaltSize = 16;
    private const int HashSize = 32;

    public async Task<ValueResult<User>> AuthenticateUserAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return ResultMessage.Error("Invalid username or password.");
        }

        var userData =
            await databaseContext.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Username == username);

        if (userData is null || !VerifyPasswordHash(userData.Username, password, userData.PasswordHash))
        {
            return ResultMessage.Error("Invalid username or password.");
        }

        return new User
        {
            Id = userData.Id,
            Username = userData.Username,
            Email = userData.Email,
            PasswordHash = userData.PasswordHash,
            Status = userData.Status,
            CreatedAt = userData.CreatedAt,
            VerifiedAt = userData.VerifiedAt
        };
    }

    public ValueResult<string> GeneratePasswordHash(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return ResultMessage.Error("Username is required.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return ResultMessage.Error("Password is required.");
        }

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = HashPassword(username, password, salt, MinimumIterations, HashSize);

        return ValueResult.Success($"{PasswordHashAlgorithm}${MinimumIterations.ToString(CultureInfo.InvariantCulture)}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}");
    }

    private static byte[] HashPassword(string username, string password, byte[] salt, int iterations, int outputSize)
    {
        var secret = Encoding.UTF8.GetBytes($"{username}\0{password}");

        try
        {
            return Rfc2898DeriveBytes.Pbkdf2(secret, salt, iterations, HashAlgorithmName.SHA512, outputSize);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(secret);
        }
    }

    private static bool VerifyPasswordHash(string username, string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        var hashParts = passwordHash.Split('$', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (hashParts.Length != 4 || !string.Equals(hashParts[0], PasswordHashAlgorithm, StringComparison.Ordinal))
        {
            return false;
        }

        if (!int.TryParse(hashParts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var iterations) || iterations < MinimumIterations)
        {
            return false;
        }

        byte[] salt;
        byte[] expectedHash;

        try
        {
            salt = Convert.FromBase64String(hashParts[2]);
            expectedHash = Convert.FromBase64String(hashParts[3]);
        }
        catch (FormatException)
        {
            return false;
        }

        if (salt.Length != SaltSize || expectedHash.Length == 0)
        {
            return false;
        }

        var actualHash = HashPassword(username, password, salt, iterations, expectedHash.Length);
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
