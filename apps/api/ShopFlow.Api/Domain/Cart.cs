namespace ShopFlow.Api.Domain;

public class Cart
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public List<CartItem> Items { get; set; } = new();

    public decimal Total => Items.Sum(i => i.Subtotal);
}
