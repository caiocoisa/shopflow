using System.ComponentModel.DataAnnotations;

namespace ShopFlow.Api.Products.Dtos;

public class UpsertProductRequest
{
    [Required, MinLength(2), MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    [Required, MinLength(2), MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required, Range(0.01, 1_000_000)]
    public decimal Price { get; set; }

    [Required, Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [Required, Url, MaxLength(512)]
    public string ImageUrl { get; set; } = string.Empty;

    [Required]
    public Guid CategoryId { get; set; }
}
