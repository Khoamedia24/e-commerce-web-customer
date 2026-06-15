using e_commerce_web_customer.Application.Products;
using e_commerce_web_customer.Models.Constants;
using e_commerce_web_customer.Models.Entities;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Home.Db;

internal static class DbHomeProductCardMapper
{
    private const string FallbackImageUrl = "/images/logo-techstore-icon.svg";

    public static ProductCardViewModel? ToProductCard(ProductVariant variant)
    {
        var product = variant.Product;

        if (product is null)
        {
            return null;
        }

        var image = variant.ProductVariantImages
            .OrderBy(item => item.Position)
            .FirstOrDefault();
        var productSlug = string.IsNullOrWhiteSpace(product.Slug)
            ? product.Id.ToString()
            : product.Slug;
        var variantKey = string.IsNullOrWhiteSpace(variant.Code)
            ? variant.Id.ToString()
            : variant.Code;

        var displayName = BuildVariantName(product, variant);

        return new ProductCardViewModel
        {
            Id = variantKey,
            Name = displayName,
            Url = $"/product/{productSlug}?variant={Uri.EscapeDataString(variantKey)}",
            ImageUrl = NormalizeImageUrl(image?.ImagePath),
            ImageAlt = string.IsNullOrWhiteSpace(image?.AltText)
                ? displayName
                : image.AltText,
            CurrentPrice = ProductViewModelMapper.FormatPrice(variant.Price),
            InstallmentLabel = "Trả góp 0%",
            AvailabilityLabel = variant.Quantity <= 0 ? "Tạm hết hàng" : null,
            DeliveryLabel = "Giao 2 giờ",
            Location = "Hồ Chí Minh",
            Rating = product.RatingAverage > 0 ? product.RatingAverage : null
        };
    }

    private static string BuildVariantName(Product product, ProductVariant variant)
    {
        var variantParts = variant.VariantAttributes
            .Where(attribute => !string.Equals(
                attribute.AttributeOption?.Attribute?.Code,
                CatalogAttributeCodes.Color,
                StringComparison.OrdinalIgnoreCase))
            .Select(attribute => new
            {
                Code = attribute.AttributeOption?.Attribute?.Code,
                Name = attribute.AttributeOption?.Attribute?.Name,
                Value = attribute.AttributeOption?.Value,
                Label = attribute.AttributeOption?.Label,
                OptionId = attribute.AttributeOptionId
            })
            .Where(attribute => !string.IsNullOrWhiteSpace(attribute.Label))
            .OrderBy(attribute => GetAttributeDisplayOrder(
                attribute.Code,
                attribute.Name,
                attribute.Value,
                attribute.Label))
            .ThenBy(attribute => attribute.OptionId)
            .Select(attribute => attribute.Label!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return variantParts.Count == 0
            ? product.Name
            : $"{product.Name} {string.Join(" ", variantParts)}";
    }

    private static int GetAttributeDisplayOrder(
        string? code,
        string? name,
        string? value,
        string? label)
    {
        var normalizedCode = NormalizeCode(code);
        if (normalizedCode is CatalogAttributeCodes.Ram or CatalogAttributeCodes.RamCapacity)
        {
            return 0;
        }

        if (normalizedCode is CatalogAttributeCodes.Rom
            or CatalogAttributeCodes.Storage
            or CatalogAttributeCodes.InternalStorage)
        {
            return 1;
        }

        var searchableText = string.Join(
            ' ',
            code,
            name,
            value,
            label).ToLowerInvariant();

        if (ContainsAny(
            searchableText,
            CatalogAttributeCodes.Ram,
            "bo nho ram",
            "bộ nhớ ram"))
        {
            return 0;
        }

        if (ContainsAny(
            searchableText,
            CatalogAttributeCodes.Rom,
            CatalogAttributeCodes.Storage,
            CatalogAttributeCodes.InternalStorage,
            "internal-storage",
            "capacity",
            "dung luong",
            "dung lượng",
            "luu tru",
            "lưu trữ",
            "bo nho trong",
            "bộ nhớ trong"))
        {
            return 1;
        }

        return 100;
    }

    private static string NormalizeCode(string? code)
    {
        return code?.Trim().ToLowerInvariant() ?? string.Empty;
    }

    private static bool ContainsAny(string text, params string[] values)
    {
        return values.Any(value => text.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeImageUrl(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return FallbackImageUrl;
        }

        if (imagePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || imagePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            || imagePath.StartsWith('/'))
        {
            return imagePath;
        }

        return "/" + imagePath.TrimStart('/');
    }
}
