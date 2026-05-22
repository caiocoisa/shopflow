using Microsoft.EntityFrameworkCore;
using ShopFlow.Api.Carts.Dtos;
using ShopFlow.Api.Common;
using ShopFlow.Api.Domain;
using ShopFlow.Api.Persistence;

namespace ShopFlow.Api.Carts;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _db;
    public CartService(ApplicationDbContext db) => _db = db;

    public async Task<CartDto> GetOrCreateAsync(Guid userId, CancellationToken ct = default)
    {
        var cart = await LoadCartAsync(userId, ct) ?? await CreateCartAsync(userId, ct);
        return CartDto.From(cart);
    }

    public async Task<CartDto> AddItemAsync(Guid userId, AddToCartRequest request, CancellationToken ct = default)
    {
        var cart = await LoadCartAsync(userId, ct) ?? await CreateCartAsync(userId, ct);

        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, ct)
            ?? throw new NotFoundException("Produto", request.ProductId);

        if (product.Stock < request.Quantity)
            throw new InsufficientStockException(product.Name, request.Quantity, product.Stock);

        var existing = cart.Items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existing is not null)
        {
            var newQty = existing.Quantity + request.Quantity;
            if (product.Stock < newQty)
                throw new InsufficientStockException(product.Name, newQty, product.Stock);
            existing.Quantity = newQty;
            existing.UnitPrice = product.Price;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = product.Id,
                Quantity = request.Quantity,
                UnitPrice = product.Price
            });
        }

        await _db.SaveChangesAsync(ct);
        cart = await LoadCartAsync(userId, ct);
        return CartDto.From(cart!);
    }

    public async Task<CartDto> UpdateItemAsync(Guid userId, Guid itemId, UpdateCartItemRequest request, CancellationToken ct = default)
    {
        var cart = await LoadCartAsync(userId, ct)
            ?? throw new NotFoundException("Carrinho não encontrado");

        var item = cart.Items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new NotFoundException("Item do carrinho", itemId);

        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, ct)
            ?? throw new NotFoundException("Produto", item.ProductId);

        if (product.Stock < request.Quantity)
            throw new InsufficientStockException(product.Name, request.Quantity, product.Stock);

        item.Quantity = request.Quantity;
        item.UnitPrice = product.Price;
        await _db.SaveChangesAsync(ct);

        cart = await LoadCartAsync(userId, ct);
        return CartDto.From(cart!);
    }

    public async Task<CartDto> RemoveItemAsync(Guid userId, Guid itemId, CancellationToken ct = default)
    {
        var cart = await LoadCartAsync(userId, ct)
            ?? throw new NotFoundException("Carrinho não encontrado");

        var item = cart.Items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new NotFoundException("Item do carrinho", itemId);

        _db.CartItems.Remove(item);
        await _db.SaveChangesAsync(ct);

        cart = await LoadCartAsync(userId, ct);
        return CartDto.From(cart!);
    }

    public async Task<CartDto> ClearAsync(Guid userId, CancellationToken ct = default)
    {
        var cart = await LoadCartAsync(userId, ct);
        if (cart is null) return await GetOrCreateAsync(userId, ct);

        _db.CartItems.RemoveRange(cart.Items);
        await _db.SaveChangesAsync(ct);

        cart = await LoadCartAsync(userId, ct);
        return CartDto.From(cart!);
    }

    private Task<Cart?> LoadCartAsync(Guid userId, CancellationToken ct) =>
        _db.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);

    private async Task<Cart> CreateCartAsync(Guid userId, CancellationToken ct)
    {
        var cart = new Cart { UserId = userId };
        _db.Carts.Add(cart);
        await _db.SaveChangesAsync(ct);
        return cart;
    }
}
