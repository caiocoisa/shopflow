using ShopFlow.Api.Common;
using ShopFlow.Api.Orders.Dtos;

namespace ShopFlow.Api.Orders;

public interface IOrderService
{
    Task<OrderDto> CreateFromCartAsync(Guid userId, CancellationToken ct = default);
    Task<PagedResult<OrderDto>> ListMineAsync(Guid userId, int page, int pageSize, CancellationToken ct = default);
    Task<OrderDto> GetByIdAsync(Guid orderId, Guid currentUserId, bool isAdmin, CancellationToken ct = default);
}
