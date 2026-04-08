namespace Sellorio.Srsly.Services.Users;

public class JwtAuthenticationOptions
{
    public const string SectionName = "JwtAuthentication";
    public const string CookieName = "srsly-auth-token";
    public const string StatusClaimType = "status";

    public string Issuer { get; set; } = "Sellorio.Srsly";
    public string Audience { get; set; } = "Sellorio.Srsly";
    public string SigningKey { get; set; } = "ReplaceThisDevelopmentSigningKeyWithASecureProductionValue";
    public int TokenLifetimeMinutes { get; set; } = 60;
}
