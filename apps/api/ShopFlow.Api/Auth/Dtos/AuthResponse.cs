using ShopFlow.Api.Domain;

namespace ShopFlow.Api.Auth.Dtos;

public record AuthResponse(
    string Token,
    DateTime ExpiresAt,
    UserDto User);

public record UserDto(
    Guid Id,
    string Email,
    string Name,
    string Role)
{
    public static UserDto From(User user) => new(user.Id, user.Email, user.Name, user.Role.ToString());
}
