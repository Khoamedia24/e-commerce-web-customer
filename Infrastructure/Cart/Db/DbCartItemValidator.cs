using System.Globalization;
using System.Text.RegularExpressions;
using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Services;
using e_commerce_web_customer.Data;
using e_commerce_web_customer.Models.Constants;
using e_commerce_web_customer.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace e_commerce_web_customer.Infrastructure.Cart.Db;

public sealed class DbCartItemValidator(EcommerceDbContext dbContext) : ICartItemValidator
{
    private const string FallbackImageUrl = "/images/logo-techstore-icon.svg";

    public async Task<CartSessionItem> ValidateAsync(
        CartSessionItem requestItem,
        CancellationToken cancellationToken = default)
    {
        var variantKey = requestItem.Id?.Trim();
        if (string.IsNullOrWhiteSpace(variantKey))
        {
            throw new CartItemValidationException("Biến thể sản phẩm không hợp lệ.");
        }

        var query = dbContext.ProductVariants
            .AsNoTracking()
            .Include(variant => variant.Product)
            .Include(variant => variant.ProductVariantImages)
            .Include(variant => variant.VariantAttributes)
                .ThenInclude(variantAttribute => variantAttribute.AttributeOption)
                    .ThenInclude(attributeOption => attributeOption!.Attribute)
            .Where(variant =>
                variant.IsActive
                && variant.Product != null
                && variant.Product.IsActive);

        ProductVariant? variant;
        if (long.TryParse(variantKey, NumberStyles.Integer, CultureInfo.InvariantCulture, out var variantId))
        {
            variant = await query.FirstOrDefaultAsync(
                item => item.Id == variantId,
                cancellationToken);
        }
        else
        {
            variant = await query.FirstOrDefaultAsync(
                item => item.Code == variantKey,
                cancellationToken);
        }

        if (variant?.Product is null)
        {
            throw new CartItemValidationException("Biến thể sản phẩm không còn tồn tại.");
        }

        if (variant.Quantity <= 0)
        {
            throw new CartItemValidationException("Biến thể sản phẩm hiện đã hết hàng.");
        }

        var quantity = Math.Clamp(requestItem.Quantity, 1, variant.Quantity);
        var image = variant.ProductVariantImages
            .OrderBy(item => item.Position)
            .ThenBy(item => item.Id)
            .FirstOrDefault();
        var variantIdentity = string.IsNullOrWhiteSpace(variant.Code)
            ? variant.Id.ToString(CultureInfo.InvariantCulture)
            : variant.Code;
        var productName = BuildProductName(variant.Product, variant);
        var variantLabel = BuildVariantLabel(variant);

        return new CartSessionItem
        {
            Id = variantIdentity,
            Name = productName,
            ProductUrl = $"/product/{Uri.EscapeDataString(variant.Product.Slug)}?variant={Uri.EscapeDataString(variantIdentity)}",
            ImageUrl = NormalizeImageUrl(image?.ImagePath),
            ImageAlt = string.IsNullOrWhiteSpace(image?.AltText)
                ? string.IsNullOrWhiteSpace(variant.ColorName)
                    ? productName
                    : $"{productName} {variant.ColorName}"
                : image.AltText,
            Variant = variantLabel,
            UnitPrice = variant.Price,
            Quantity = quantity
        };
    }

    private static string BuildProductName(Product product, ProductVariant variant)
    {
        var parts = new List<string> { product.Name };
        AddCapacity(parts, FindAttributeLabel(
            variant,
            CatalogAttributeCodes.Ram,
            CatalogAttributeCodes.RamCapacity));
        AddCapacity(parts, FindAttributeLabel(
            variant,
            CatalogAttributeCodes.Rom,
            CatalogAttributeCodes.Storage,
            CatalogAttributeCodes.InternalStorage));

        return string.Join(' ', parts);
    }

    private static string BuildVariantLabel(ProductVariant variant)
    {
        var parts = new List<string>();
        AddCapacity(parts, FindAttributeLabel(
            variant,
            CatalogAttributeCodes.Ram,
            CatalogAttributeCodes.RamCapacity));
        AddCapacity(parts, FindAttributeLabel(
            variant,
            CatalogAttributeCodes.Rom,
            CatalogAttributeCodes.Storage,
            CatalogAttributeCodes.InternalStorage));

        if (!string.IsNullOrWhiteSpace(variant.ColorName))
        {
            parts.Add(variant.ColorName.Trim());
        }

        return string.Join(" - ", parts);
    }

    private static string? FindAttributeLabel(
        ProductVariant variant,
        params string[] attributeCodes)
    {
        return variant.VariantAttributes
            .Select(attribute => attribute.AttributeOption)
            .Where(option => option?.Attribute is not null)
            .FirstOrDefault(option => attributeCodes.Any(code =>
                string.Equals(code, option!.Attribute!.Code, StringComparison.OrdinalIgnoreCase)))
            ?.Label;
    }

    private static void AddCapacity(List<string> parts, string? label)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            return;
        }

        var normalizedLabel = Regex.Replace(
            label.Trim(),
            "(\\d)\\s+(GB|TB|MB)\\b",
            "$1$2",
            RegexOptions.IgnoreCase);

        if (!parts.Contains(normalizedLabel, StringComparer.OrdinalIgnoreCase))
        {
            parts.Add(normalizedLabel);
        }
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
