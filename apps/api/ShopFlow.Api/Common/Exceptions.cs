using Microsoft.AspNetCore.Http;

namespace ShopFlow.Api.Common;

public class NotFoundException : AppException
{
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string Title => "Recurso não encontrado";

    public NotFoundException(string entity, object id) : base($"{entity} não encontrado: {id}") { }
    public NotFoundException(string message) : base(message) { }
}

public class BadRequestException : AppException
{
    public override int StatusCode => StatusCodes.Status400BadRequest;
    public override string Title => "Requisição inválida";

    public BadRequestException(string message) : base(message) { }
}

public class ForbiddenException : AppException
{
    public override int StatusCode => StatusCodes.Status403Forbidden;
    public override string Title => "Acesso negado";

    public ForbiddenException(string message = "Acesso negado") : base(message) { }
}

public class InsufficientStockException : AppException
{
    public override int StatusCode => StatusCodes.Status400BadRequest;
    public override string Title => "Estoque insuficiente";

    public InsufficientStockException(string productName, int requested, int available)
        : base($"Estoque insuficiente para '{productName}': pedido {requested}, disponível {available}") { }
}
