namespace ShopFlow.Api.Auth;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "shopflow-api";
    public string Audience { get; set; } = "shopflow-clients";
    public int ExpiresInHours { get; set; } = 24;
}
