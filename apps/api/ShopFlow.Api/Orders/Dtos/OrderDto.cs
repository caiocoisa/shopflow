using ShopFlow.Api.Domain;

namespace ShopFlow.Api.Orders.Dtos;

public record OrderDto(
    Guid Id,
    Guid UserId,
    string Status,
    decimal Total,
    DateTime CreatedAt,
    DateTime? PaidAt,
    IReadOnlyList<OrderItemDto> Items)
{
    public static OrderDto From(Order o) => new(
        o.Id,
        o.UserId,
        o.Status.ToString(),
        o.Total,
        o.CreatedAt,
        o.PaidAt,
        o.Items.Select(OrderItemDto.From).ToList());
}

public record OrderItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal)
{
    public static OrderItemDto From(OrderItem i) => new(
        i.Id, i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.UnitPrice * i.Quantity);
}
