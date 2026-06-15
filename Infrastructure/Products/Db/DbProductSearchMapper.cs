using e_commerce_web_customer.Application.Products;
using e_commerce_web_customer.Models.Constants;
using e_commerce_web_customer.Models.Entities;

namespace e_commerce_web_customer.Infrastructure.Products.Db;

internal static class DbProductSearchMapper
{
    private const string FallbackImageUrl = "/images/logo-techstore-icon.svg";

    public static ProductReadModel Map(Product product)
    {
        var activeVariants = product.ProductVariants
            .Where(variant => variant.IsActive)
            .ToList();
        var representativeVariant = activeVariants
            .OrderByDescending(variant => variant.IsDefault)
            .ThenByDescending(variant => variant.Quantity > 0)
            .ThenByDescending(variant => variant.ProductVariantImages.Count > 0)
            .ThenByDescending(variant => variant.SoldCount)
            .ThenBy(variant => variant.Price)
            .ThenBy(variant => variant.Id)
            .First();
        var image = representativeVariant.ProductVariantImages
            .OrderBy(item => item.Position)
            .ThenBy(item => item.Id)
            .FirstOrDefault();
        var productSlug = string.IsNullOrWhiteSpace(product.Slug)
            ? product.Id.ToString()
            : product.Slug;
        var currentPrice = activeVariants.Min(variant => variant.Price);

        return new ProductReadModel(
            product.Id.ToString(),
            ProductDisplayNameNormalizer.ToBaseName(product.Name),
            $"/product/{productSlug}",
            NormalizeImageUrl(image?.ImagePath),
            string.IsNullOrWhiteSpace(image?.AltText)
                ? ProductDisplayNameNormalizer.ToBaseName(product.Name)
                : image.AltText,
            currentPrice,
            null,
            0,
            null,
            null,
            BuildSearchText(product),
            new[] { product.Slug }
                .Concat(activeVariants.Select(variant => variant.Code))
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList(),
            product.Category?.Slug,
            product.Category?.Name,
            activeVariants.Any(variant => variant.Quantity > 0)
                ? null
                : "Tạm hết hàng",
            product.RatingAverage > 0 ? product.RatingAverage : null,
            [],
            DbProductSearchRanker.CalculatePopularity(product));
    }

    public static ProductReadModel MapVariant(Product product, ProductVariant variant)
    {
        var image = variant.ProductVariantImages
            .OrderBy(item => item.Position)
            .ThenBy(item => item.Id)
            .FirstOrDefault();
        var productSlug = string.IsNullOrWhiteSpace(product.Slug)
            ? product.Id.ToString()
            : product.Slug;
        var variantKey = string.IsNullOrWhiteSpace(variant.Code)
            ? variant.Id.ToString()
            : variant.Code;
        var displayName = BuildVariantName(product, variant);
        var variantUrl = $"/product/{productSlug}?variant={Uri.EscapeDataString(variantKey)}";

        return new ProductReadModel(
            variantKey,
            displayName,
            variantUrl,
            NormalizeImageUrl(image?.ImagePath),
            string.IsNullOrWhiteSpace(image?.AltText) ? displayName : image.AltText,
            variant.Price,
            null,
            0,
            null,
            null,
            BuildVariantSearchText(product, variant),
            new[] { product.Slug, variant.Code, variant.Code.Replace("-", string.Empty) }
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList(),
            product.Category?.Slug,
            product.Category?.Name,
            variant.Quantity > 0 ? null : "Tạm hết hàng",
            product.RatingAverage > 0 ? product.RatingAverage : null,
            [],
            DbProductSearchRanker.CalculatePopularity(product, variant));
    }

    public static string BuildSearchText(Product product)
    {
        var specifications = product.ProductSpecifications
            .Select(item => $"{item.Specification?.Name} {item.Value}");
        var variants = product.ProductVariants.Where(variant => variant.IsActive);
        var variantValues = variants.SelectMany(variant => new[]
        {
            variant.Code,
            variant.ColorName
        });
        var attributes = variants
            .SelectMany(variant => variant.VariantAttributes)
            .Select(attribute =>
                $"{attribute.AttributeOption?.Attribute?.Name} {attribute.AttributeOption?.Label}");

        return string.Join(
            ' ',
            new[]
            {
                product.Name,
                product.Slug,
                product.Description,
                product.Brand?.Name,
                product.Brand?.Slug,
                product.Category?.Name,
                product.Category?.Slug
            }
            .Concat(specifications)
            .Concat(variantValues)
            .Concat(attributes)
            .Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    public static string BuildVariantName(Product product, ProductVariant variant)
    {
        var baseName = product.Name.Trim();
        var normalizedBaseName = NormalizeDisplayToken(baseName);
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
            .Select(attribute => attribute.Label!.Trim())
            .Where(label => !normalizedBaseName.Contains(
                NormalizeDisplayToken(label),
                StringComparison.Ordinal))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return variantParts.Count == 0
            ? baseName
            : $"{baseName} {string.Join(" ", variantParts)}";
    }

    public static string BuildVariantSearchText(Product product, ProductVariant variant)
    {
        var attributes = variant.VariantAttributes
            .Select(attribute =>
                $"{attribute.AttributeOption?.Attribute?.Name} {attribute.AttributeOption?.Label} {attribute.AttributeOption?.Value}");

        return string.Join(
            ' ',
            new[]
            {
                BuildSearchText(product),
                BuildVariantName(product, variant),
                variant.Code,
                variant.Code.Replace("-", string.Empty),
                variant.ColorName
            }
            .Concat(attributes)
            .Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    private static int GetAttributeDisplayOrder(
        string? code,
        string? name,
        string? value,
        string? label)
    {
        var normalizedCode = code?.Trim().ToLowerInvariant() ?? string.Empty;
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

    private static bool ContainsAny(string text, params string[] values)
    {
        return values.Any(value => text.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeDisplayToken(string value)
    {
        return value
            .Trim()
            .ToLowerInvariant()
            .Replace(" ", string.Empty, StringComparison.Ordinal);
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
