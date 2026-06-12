using System.Globalization;

namespace e_commerce_web_customer.ViewModels.Cart;

public sealed class CartIndexViewModel
{
    private static readonly CultureInfo VietnameseCulture = CultureInfo.GetCultureInfo("vi-VN");

    public IReadOnlyList<CartItemViewModel> Items { get; init; } = [];
    public decimal ShippingFee { get; init; }
    public decimal Tax { get; init; }

    public int ItemCount => Items.Sum(item => item.Quantity);
    public decimal Subtotal => Items.Sum(item => item.UnitPrice * item.Quantity);
    public decimal Total => Subtotal + ShippingFee + Tax;
    public bool IsEmpty => Items.Count == 0;

    public static string FormatPrice(decimal value)
    {
        return $"{value.ToString("N0", VietnameseCulture)}đ";
    }
}
