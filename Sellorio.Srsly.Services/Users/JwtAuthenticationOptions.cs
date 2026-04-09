namespace Sellorio.Srsly.Services.Users;

public class JwtAuthenticationOptions
{
    public static string SectionName => "JwtAuthentication";
    public static string CookieName => "srsly-auth-token";
    public static string StatusClaimType => "status";

    public string Issuer { get; set; } = "https://dev-srs.sellor.io";
    public string Audience { get; set; } = "https://dev-srs.sellor.io";
    public string SigningKey { get; set; } = "ReplaceThisDevelopmentSigningKeyWithASecureProductionValue";
    public int TokenLifetimeMinutes { get; set; } = 60;
}
