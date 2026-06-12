using e_commerce_web_customer.ViewModels.Cart;
using e_commerce_web_customer.ViewModels.Checkout;

namespace e_commerce_web_customer.Infrastructure.MockData;

/// <summary>
/// Centralized provider for cart-related mock/demo data to ensure consistency and avoid duplication.
/// </summary>
public static class MockCartData
{
    /// <summary>
    /// Generates the standard demo cart items for the Cart index page.
    /// </summary>
    public static List<CartItemViewModel> GetCartDemoItems() =>
    [
        new CartItemViewModel
        {
            Id = "iphone-17-pro-max-256gb",
            Name = "iPhone 17 Pro Max 256GB | Chính hãng",
            ProductUrl = "/product/iphone-17-pro-max-256gb",
            ImageUrl = "/images/products/phone/phone-orange.webp",
            ImageAlt = "iPhone 17 Pro Max màu cam",
            Variant = "Màu Cam, 256GB",
            UnitPrice = 36_990_000m,
            OldPrice = 37_990_000m,
            Quantity = 1
        },
        new CartItemViewModel
        {
            Id = "headphone-wireless-pro",
            Name = "Tai nghe Bluetooth chụp tai Wireless Pro",
            ProductUrl = "/product/headphone-wireless-pro",
            ImageUrl = "/images/products/audio-wearables/audio-01.webp",
            ImageAlt = "Tai nghe Bluetooth chụp tai màu đen",
            Variant = "Màu Đen",
            UnitPrice = 2_790_000m,
            OldPrice = 3_290_000m,
            Quantity = 1
        }
    ];

    /// <summary>
    /// Generates the standard demo checkout items mapping from the cart items to ensure details stay in sync.
    /// </summary>
    public static List<CheckoutItemViewModel> GetCheckoutDemoItems()
    {
        return GetCartDemoItems().Select(item => new CheckoutItemViewModel
        {
            Name = item.Name,
            ImageUrl = item.ImageUrl,
            ImageAlt = item.ImageAlt,
            Variant = item.Variant,
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity
        }).ToList();
    }
}
