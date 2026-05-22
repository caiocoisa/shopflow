namespace ShopFlow.Api.Domain;

public enum OrderStatus
{
    Pending = 0,
    Paid = 1,
    Failed = 2,
    Cancelled = 3,
    Shipped = 4,
    Delivered = 5
}
