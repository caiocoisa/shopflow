using ShopFlow.Api.Carts.Dtos;

namespace ShopFlow.Api.Carts;

public interface ICartService
{
    Task<CartDto> GetOrCreateAsync(Guid userId, CancellationToken ct = default);
    Task<CartDto> AddItemAsync(Guid userId, AddToCartRequest request, CancellationToken ct = default);
    Task<CartDto> UpdateItemAsync(Guid userId, Guid itemId, UpdateCartItemRequest request, CancellationToken ct = default);
    Task<CartDto> RemoveItemAsync(Guid userId, Guid itemId, CancellationToken ct = default);
    Task<CartDto> ClearAsync(Guid userId, CancellationToken ct = default);
}
