using System.ComponentModel.DataAnnotations;

namespace ShopFlow.Api.Carts.Dtos;

public class AddToCartRequest
{
    [Required]
    public Guid ProductId { get; set; }

    [Required, Range(1, 1000)]
    public int Quantity { get; set; }
}

public class UpdateCartItemRequest
{
    [Required, Range(1, 1000)]
    public int Quantity { get; set; }
}
