using Microsoft.EntityFrameworkCore;
using ShopFlow.Api.Auth;
using ShopFlow.Api.Domain;

namespace ShopFlow.Api.Persistence;

/// <summary>
/// Seed idempotente do banco. Roda no startup em Development.
/// Categorias têm valores fixos. Admin é criado APENAS se Seed:AdminEmail e Seed:AdminPassword
/// estiverem definidos via user-secrets ou env vars (nunca hardcoded).
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, IPasswordHasher hasher, IConfiguration config, ILogger logger)
    {
        await SeedCategoriesAsync(db, logger);
        await SeedAdminAsync(db, hasher, config, logger);
        await db.SaveChangesAsync();
    }

    private static async Task SeedCategoriesAsync(ApplicationDbContext db, ILogger logger)
    {
        var defaults = new[]
        {
            ("Eletrônicos", "eletronicos"),
            ("Livros", "livros"),
            ("Moda", "moda")
        };

        var existingSlugs = await db.Categories.Select(c => c.Slug).ToListAsync();
        var missing = defaults.Where(d => !existingSlugs.Contains(d.Item2)).ToArray();
        if (missing.Length == 0) return;

        foreach (var (name, slug) in missing)
        {
            db.Categories.Add(new Category { Name = name, Slug = slug });
        }
        logger.LogInformation("Seed: adicionadas {Count} categorias", missing.Length);
    }

    private static async Task SeedAdminAsync(ApplicationDbContext db, IPasswordHasher hasher, IConfiguration config, ILogger logger)
    {
        var email = config["Seed:AdminEmail"]?.Trim().ToLowerInvariant();
        var password = config["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogInformation("Seed: admin pulado — Seed:AdminEmail/AdminPassword não configurados em user-secrets");
            return;
        }

        if (await db.Users.AnyAsync(u => u.Email == email)) return;

        db.Users.Add(new User
        {
            Email = email,
            PasswordHash = hasher.Hash(password),
            Name = "Admin",
            Role = UserRole.Admin
        });
        logger.LogInformation("Seed: admin criado com email {Email}", email);
    }
}
