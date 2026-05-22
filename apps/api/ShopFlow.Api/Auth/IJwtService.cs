using ShopFlow.Api.Domain;

namespace ShopFlow.Api.Auth;

public interface IJwtService
{
    TokenResult GenerateToken(User user);
}

public record TokenResult(string Token, DateTime ExpiresAt);
