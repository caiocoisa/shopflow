using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ShopFlow.Api.Common;

/// <summary>
/// Captura exceções não tratadas e devolve um ProblemDetails (RFC 7807).
/// AppException carrega seu próprio StatusCode/Title. Exceções inesperadas viram 500
/// sem vazar a mensagem real em produção.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "Exceção de aplicação tratada: {Message}", ex.Message);
            await WriteProblemAsync(context, ex.StatusCode, ex.Title, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exceção não tratada");
            var detail = _env.IsDevelopment() ? ex.ToString() : "Ocorreu um erro inesperado.";
            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError, "Erro interno", detail);
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, int statusCode, string title, string detail)
    {
        if (context.Response.HasStarted) return;

        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path,
            Extensions = { ["traceId"] = traceId }
        };

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionHandlingMiddleware>();
}
