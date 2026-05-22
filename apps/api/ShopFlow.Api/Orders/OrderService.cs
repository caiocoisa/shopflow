using Microsoft.EntityFrameworkCore;
using ShopFlow.Api.Common;
using ShopFlow.Api.Domain;
using ShopFlow.Api.Orders.Dtos;
using ShopFlow.Api.Persistence;

namespace ShopFlow.Api.Orders;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _db;
    public OrderService(ApplicationDbContext db) => _db = db;

    public async Task<OrderDto> CreateFromCartAsync(Guid userId, CancellationToken ct = default)
    {
        var cart = await _db.Carts
            .Include(c => c.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);

        if (cart is null || cart.Items.Count == 0)
            throw new BadRequestException("Carrinho vazio — não é possível criar pedido");

        foreach (var item in cart.Items)
        {
            if (item.Product is null)
                throw new BadRequestException($"Produto do item {item.Id} não encontrado");
            if (item.Product.Stock < item.Quantity)
                throw new InsufficientStockException(item.Product.Name, item.Quantity, item.Product.Stock);
        }

        var order = new Order
        {
            UserId = userId,
            Status = OrderStatus.Pending,
            Total = cart.Items.Sum(i => i.UnitPrice * i.Quantity),
            Items = cart.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.Product!.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        foreach (var item in cart.Items)
            item.Product!.Stock -= item.Quantity;

        _db.Orders.Add(order);
        _db.CartItems.RemoveRange(cart.Items);
        await _db.SaveChangesAsync(ct);

        return OrderDto.From(order);
    }

    public async Task<PagedResult<OrderDto>> ListMineAsync(Guid userId, int page, int pageSize, CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _db.Orders.Include(o => o.Items).AsNoTracking().Where(o => o.UserId == userId);
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => OrderDto.From(o))
            .ToListAsync(ct);

        return new PagedResult<OrderDto>(items, page, pageSize, total);
    }

    public async Task<OrderDto> GetByIdAsync(Guid orderId, Guid currentUserId, bool isAdmin, CancellationToken ct = default)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == orderId, ct)
            ?? throw new NotFoundException("Pedido", orderId);

        if (order.UserId != currentUserId && !isAdmin)
            throw new ForbiddenException("Você não tem acesso a este pedido");

        return OrderDto.From(order);
    }
}
