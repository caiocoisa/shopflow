using System.ComponentModel.DataAnnotations;

namespace ShopFlow.Api.Auth.Dtos;

public class RegisterRequest
{
    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8), MaxLength(128)]
    public string Password { get; set; } = string.Empty;

    [Required, MinLength(2), MaxLength(128)]
    public string Name { get; set; } = string.Empty;
}
