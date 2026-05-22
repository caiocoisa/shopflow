namespace ShopFlow.Api.Common;

/// <summary>
/// Base para exceções de aplicação que mapeiam diretamente para uma resposta HTTP.
/// O ExceptionHandlingMiddleware lê StatusCode e Title para montar o ProblemDetails.
/// </summary>
public abstract class AppException : Exception
{
    public abstract int StatusCode { get; }
    public abstract string Title { get; }

    protected AppException(string message) : base(message) { }
}
