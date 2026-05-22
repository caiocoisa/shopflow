using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopFlow.Api.Carts;
using ShopFlow.Api.Carts.Dtos;
using ShopFlow.Api.Common;

namespace ShopFlow.Api.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _service;
    public CartController(ICartService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartDto>> Get(CancellationToken ct)
        => Ok(await _service.GetOrCreateAsync(User.GetUserId(), ct));

    [HttpPost("items")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CartDto>> AddItem([FromBody] AddToCartRequest request, CancellationToken ct)
        => Ok(await _service.AddItemAsync(User.GetUserId(), request, ct));

    [HttpPut("items/{itemId:guid}")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CartDto>> UpdateItem(Guid itemId, [FromBody] UpdateCartItemRequest request, CancellationToken ct)
        => Ok(await _service.UpdateItemAsync(User.GetUserId(), itemId, request, ct));

    [HttpDelete("items/{itemId:guid}")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CartDto>> RemoveItem(Guid itemId, CancellationToken ct)
        => Ok(await _service.RemoveItemAsync(User.GetUserId(), itemId, ct));

    [HttpDelete]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartDto>> Clear(CancellationToken ct)
        => Ok(await _service.ClearAsync(User.GetUserId(), ct));
}
