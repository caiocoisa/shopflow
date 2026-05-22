using Microsoft.EntityFrameworkCore;
using ShopFlow.Api.Auth.Dtos;
using ShopFlow.Api.Domain;
using ShopFlow.Api.Persistence;

namespace ShopFlow.Api.Auth;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtService _jwt;

    public AuthService(ApplicationDbContext db, IPasswordHasher hasher, IJwtService jwt)
    {
        _db = db;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _db.Users.AnyAsync(u => u.Email == email, ct))
            throw new EmailAlreadyRegisteredException(email);

        var user = new User
        {
            Email = email,
            PasswordHash = _hasher.Hash(request.Password),
            Name = request.Name.Trim(),
            Role = UserRole.Customer
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        var token = _jwt.GenerateToken(user);
        return new AuthResponse(token.Token, token.ExpiresAt, UserDto.From(user));
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

        if (user is null || !_hasher.Verify(request.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        var token = _jwt.GenerateToken(user);
        return new AuthResponse(token.Token, token.ExpiresAt, UserDto.From(user));
    }
}
