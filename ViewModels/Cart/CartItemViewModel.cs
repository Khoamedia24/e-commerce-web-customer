namespace e_commerce_web_customer.ViewModels.Cart;

public sealed class CartItemViewModel
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string ProductUrl { get; init; }
    public required string ImageUrl { get; init; }
    public required string ImageAlt { get; init; }
    public required string Variant { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal? OldPrice { get; init; }
    public int Quantity { get; init; } = 1;
    public int MaxQuantity { get; init; } = 10;
}
