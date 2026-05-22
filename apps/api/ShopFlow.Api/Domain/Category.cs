namespace ShopFlow.Api.Domain;

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public List<Product> Products { get; set; } = new();
}
