using ShopFlow.Api.Domain;

namespace ShopFlow.Api.Carts.Dtos;

public record CartDto(
    Guid Id,
    Guid UserId,
    IReadOnlyList<CartItemDto> Items,
    decimal Total)
{
    public static CartDto From(Cart cart) => new(
        cart.Id,
        cart.UserId,
        cart.Items.Select(CartItemDto.From).ToList(),
        cart.Items.Sum(i => i.UnitPrice * i.Quantity));
}

public record CartItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string ProductImageUrl,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal)
{
    public static CartItemDto From(CartItem item) => new(
        item.Id,
        item.ProductId,
        item.Product?.Name ?? string.Empty,
        item.Product?.ImageUrl ?? string.Empty,
        item.Quantity,
        item.UnitPrice,
        item.UnitPrice * item.Quantity);
}
