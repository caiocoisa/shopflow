using ShopFlow.Api.Common;
using ShopFlow.Api.Products.Dtos;

namespace ShopFlow.Api.Products;

public interface IProductService
{
    Task<PagedResult<ProductDto>> ListAsync(int page, int pageSize, string? search, string? categorySlug, CancellationToken ct = default);
    Task<ProductDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProductDto> CreateAsync(UpsertProductRequest request, CancellationToken ct = default);
    Task<ProductDto> UpdateAsync(Guid id, UpsertProductRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
