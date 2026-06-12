namespace e_commerce_web_customer.ViewModels.Checkout;

public sealed class CheckoutSuccessViewModel
{
    public string OrderCode { get; init; } = string.Empty;
    public string CustomerName { get; init; } = "Quý khách";
    public string Phone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string DeliveryAddress { get; init; } = string.Empty;
    public string ShippingMethodName { get; init; } = string.Empty;
    public string PaymentMethodName { get; init; } = string.Empty;
    public string PlacedAt { get; init; } = string.Empty;
    public string EstimatedDeliveryDateText { get; init; } = string.Empty;
    public string ItemCountText { get; init; } = string.Empty;
    public string SubtotalText { get; init; } = string.Empty;
    public string ShippingFeeText { get; init; } = string.Empty;
    public string DiscountText { get; init; } = string.Empty;
    public string TotalText { get; init; } = string.Empty;
    public List<CheckoutSuccessItemViewModel> Items { get; init; } = [];
}

public sealed class CheckoutSuccessItemViewModel
{
    public string Name { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public string ImageAlt { get; init; } = string.Empty;
    public string Variant { get; init; } = string.Empty;
    public int Quantity { get; init; } = 1;
    public string LineTotalText { get; init; } = string.Empty;
}
