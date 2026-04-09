using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sellorio.Results;
using Sellorio.Results.Messages;
using Sellorio.Srsly.Data;
using Sellorio.Srsly.Models.Users;
using System.Linq;

namespace Sellorio.Srsly.Services.Users;

internal class AuthenticationService(DatabaseContext databaseContext, IOptions<JwtAuthenticationOptions> jwtAuthenticationOptions, IMapper mapper) : IAuthenticationService
{
    private const string PasswordHashAlgorithm = "PBKDF2-SHA512-V1";
    private const int MinimumIterations = 100_000;
    private const int SaltSize = 16;
    private const int HashSize = 32;

    public async Task<ValueResult<User>> AuthenticateUserAsync(string username, string password)
    {
        var user = await AuthenticateUserInternalAsync(username, password);

        if (user is null)
        {
            return ResultMessage.Error("Invalid username or password.");
        }

        return user;
    }

    public async Task<ValueResult<Login>> AuthenticateWithTokenAsync(string username, string password)
    {
        var result = await AuthenticateUserInternalAsync(username, password);

        if (!result.WasSuccess)
        {
            return ValueResult<Login>.Failure(result.Messages);
        }

        var jwtOptions = jwtAuthenticationOptions.Value;

        if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey) || jwtOptions.SigningKey.Length < 32)
        {
            throw new InvalidOperationException("JWT signing key must be configured and be at least 32 characters long.");
        }

        var issuedAtUtc = DateTimeOffset.UtcNow;
        var expiresAtUtc = issuedAtUtc.AddMinutes(jwtOptions.TokenLifetimeMinutes);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey));
        var token =
            new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims:
                [
                    new Claim(JwtRegisteredClaimNames.Sub, result.Value.Id.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, result.Value.Id.ToString()),
                    new Claim(ClaimTypes.Name, result.Value.Username),
                    new Claim(JwtRegisteredClaimNames.UniqueName, result.Value.Username),
                    new Claim(ClaimTypes.Email, result.Value.Email),
                    new Claim(JwtRegisteredClaimNames.Email, result.Value.Email),
                    new Claim(JwtAuthenticationOptions.StatusClaimType, result.Value.Status.ToString())
                ],
                notBefore: issuedAtUtc.UtcDateTime,
                expires: expiresAtUtc.UtcDateTime,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        return new Login
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = expiresAtUtc,
            User = result.Value
        };
    }

    private async Task<ValueResult<User>> AuthenticateUserInternalAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return ResultMessage.Error("Username and password are required.");
        }

        var userData =
            await databaseContext.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Username == username);

        if (userData is null ||
            userData.Status != UserStatus.Verified ||
            !VerifyPasswordHash(userData.Username, password, userData.PasswordHash))
        {
            return ResultMessage.Error("Incorrect username or password.");
        }

        return mapper.Map(userData);
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

        return
            ValueResult.Success(
                PasswordHashAlgorithm +
                MinimumIterations.ToString(CultureInfo.InvariantCulture) +
                Convert.ToBase64String(salt) +
                Convert.ToBase64String(hash));
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

        if (salt.Length != SaltSize || !expectedHash.Any())
        {
            return false;
        }

        var actualHash = HashPassword(username, password, salt, iterations, expectedHash.Length);
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
