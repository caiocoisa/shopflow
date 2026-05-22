using Microsoft.AspNetCore.Http;
using ShopFlow.Api.Common;

namespace ShopFlow.Api.Auth;

public class EmailAlreadyRegisteredException : AppException
{
    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string Title => "Email já cadastrado";

    public EmailAlreadyRegisteredException(string email)
        : base($"Email já cadastrado: {email}") { }
}

public class InvalidCredentialsException : AppException
{
    public override int StatusCode => StatusCodes.Status401Unauthorized;
    public override string Title => "Credenciais inválidas";

    public InvalidCredentialsException() : base("Email ou senha inválidos") { }
}
