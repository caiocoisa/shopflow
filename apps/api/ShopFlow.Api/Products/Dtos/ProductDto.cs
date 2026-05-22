using ShopFlow.Api.Domain;

namespace ShopFlow.Api.Products.Dtos;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string ImageUrl,
    Guid CategoryId,
    string CategoryName,
    string CategorySlug,
    DateTime CreatedAt)
{
    public static ProductDto From(Product p) => new(
        p.Id,
        p.Name,
        p.Description,
        p.Price,
        p.Stock,
        p.ImageUrl,
        p.CategoryId,
        p.Category?.Name ?? string.Empty,
        p.Category?.Slug ?? string.Empty,
        p.CreatedAt);
}
