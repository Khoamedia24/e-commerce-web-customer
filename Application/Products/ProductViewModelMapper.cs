using System.Globalization;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Application.Products;

public static class ProductViewModelMapper
{
    private static readonly CultureInfo VietnameseCulture =
        CultureInfo.GetCultureInfo("vi-VN");

    public static ProductCardViewModel ToProductCard(ProductReadModel product)
    {
        return new ProductCardViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Url = product.ProductUrl,
            ImageUrl = product.ImageUrl,
            ImageAlt = product.ImageAlt,
            CurrentPrice = FormatPrice(product.CurrentPrice),
            OldPrice = product.OldPrice.HasValue
                ? FormatPrice(product.OldPrice.Value)
                : null,
            DiscountLabel = product.DiscountPercent > 0
                ? $"Giảm {product.DiscountPercent}%"
                : null,
            InstallmentLabel = "Trả góp 0%",
            StudentOffer = product.StudentPrice.HasValue
                ? $"Giá S-Student {FormatPrice(product.StudentPrice.Value)}"
                : null,
            PromotionNote = product.PromotionNote,
            AvailabilityLabel = product.AvailabilityLabel,
            Specifications = product.Specifications ?? [],
            DeliveryLabel = "Giao 2 giờ",
            Location = "Hồ Chí Minh",
            Rating = product.Rating
        };
    }

    public static SearchProductSuggestionViewModel ToSearchSuggestion(
        ProductReadModel product)
    {
        return new SearchProductSuggestionViewModel
        {
            Name = product.Name,
            Url = product.ProductUrl,
            ImageUrl = product.ImageUrl,
            ImageAlt = product.ImageAlt,
            PriceText = FormatPrice(product.CurrentPrice),
            OldPriceText = product.OldPrice.HasValue
                ? FormatPrice(product.OldPrice.Value)
                : null
        };
    }

    public static string FormatPrice(decimal price)
    {
        return price.ToString("#,0", VietnameseCulture) + "đ";
    }
}
