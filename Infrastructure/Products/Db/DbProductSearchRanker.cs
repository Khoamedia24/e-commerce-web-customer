using e_commerce_web_customer.Application.Search;
using e_commerce_web_customer.Models.Entities;

namespace e_commerce_web_customer.Infrastructure.Products.Db;

internal static class DbProductSearchRanker
{
    public static int CalculateRelevance(
        Product product,
        string normalizedQuery)
    {
        if (string.IsNullOrWhiteSpace(normalizedQuery))
        {
            return CalculatePopularity(product);
        }

        var productName = SearchTextNormalizer.Normalize(product.Name);
        var brand = SearchTextNormalizer.Normalize(product.Brand?.Name ?? string.Empty);
        var category = SearchTextNormalizer.Normalize(product.Category?.Name ?? string.Empty);
        var searchText = SearchTextNormalizer.Normalize(
            DbProductSearchMapper.BuildSearchText(product));
        var compactQuery = normalizedQuery.Replace(" ", string.Empty);
        var compactSearchText = searchText.Replace(" ", string.Empty);
        var score = 0;

        if (product.ProductVariants
            .Where(variant => variant.IsActive)
            .Select(variant => SearchTextNormalizer.Normalize(variant.Code))
            .Any(code => code == normalizedQuery))
        {
            score += 1_400;
        }

        if (productName == normalizedQuery)
        {
            score += 1_200;
        }
        else if (productName.StartsWith(normalizedQuery, StringComparison.Ordinal))
        {
            score += 900;
        }
        else if (productName.Contains(normalizedQuery, StringComparison.Ordinal))
        {
            score += 700;
        }

        if (brand == normalizedQuery)
        {
            score += 500;
        }

        if (category == normalizedQuery)
        {
            score += 450;
        }

        if (compactQuery.Length >= 3
            && compactSearchText.Contains(compactQuery, StringComparison.Ordinal))
        {
            score += 600;
        }

        foreach (var term in BuildTerms(normalizedQuery))
        {
            if (productName.Contains(term, StringComparison.Ordinal))
            {
                score += 120;
            }
            else if (searchText.Contains(term, StringComparison.Ordinal))
            {
                score += 50;
            }
        }

        return score;
    }

    public static int CalculateRelevance(
        Product product,
        ProductVariant variant,
        string normalizedQuery)
    {
        if (string.IsNullOrWhiteSpace(normalizedQuery))
        {
            return CalculatePopularity(product, variant);
        }

        var score = CalculateRelevance(product, normalizedQuery);
        var variantName = SearchTextNormalizer.Normalize(
            DbProductSearchMapper.BuildVariantName(product, variant));
        var variantSearchText = SearchTextNormalizer.Normalize(
            DbProductSearchMapper.BuildVariantSearchText(product, variant));
        var variantCode = SearchTextNormalizer.Normalize(variant.Code);
        var compactQuery = normalizedQuery.Replace(" ", string.Empty);
        var compactVariantText = variantSearchText.Replace(" ", string.Empty);

        if (!string.IsNullOrWhiteSpace(variantCode)
            && variantCode == normalizedQuery)
        {
            score += 1_500;
        }

        if (variantName == normalizedQuery)
        {
            score += 1_250;
        }
        else if (variantName.StartsWith(normalizedQuery, StringComparison.Ordinal))
        {
            score += 950;
        }
        else if (variantName.Contains(normalizedQuery, StringComparison.Ordinal))
        {
            score += 750;
        }

        if (compactQuery.Length >= 3
            && compactVariantText.Contains(compactQuery, StringComparison.Ordinal))
        {
            score += 650;
        }

        foreach (var term in BuildTerms(normalizedQuery))
        {
            if (variantName.Contains(term, StringComparison.Ordinal))
            {
                score += 140;
            }
            else if (variantSearchText.Contains(term, StringComparison.Ordinal))
            {
                score += 70;
            }
        }

        return score;
    }

    public static int CalculatePopularity(Product product)
    {
        var activeVariants = product.ProductVariants.Where(variant => variant.IsActive);
        return (product.IsFeatured ? 1_000_000 : 0)
            + Math.Min(activeVariants.Sum(variant => variant.SoldCount), 50_000) * 20
            + Math.Min(product.TotalSoldCount, 50_000) * 10
            + Math.Min(product.ViewsCount, 100_000)
            + (activeVariants.Any(variant => variant.Quantity > 0) ? 5_000 : 0);
    }

    public static int CalculatePopularity(Product product, ProductVariant variant)
    {
        return (product.IsFeatured ? 1_000_000 : 0)
            + Math.Min(variant.SoldCount, 50_000) * 25
            + Math.Min(product.TotalSoldCount, 50_000) * 10
            + Math.Min(product.ViewsCount, 100_000)
            + (variant.Quantity > 0 ? 5_000 : 0)
            + (variant.IsDefault ? 1_000 : 0);
    }

    public static IReadOnlyList<string> BuildTerms(
        string query,
        int maxTerms = 8)
    {
        var normalizedQuery = SearchTextNormalizer.Normalize(query);
        if (string.IsNullOrWhiteSpace(normalizedQuery))
        {
            return [];
        }

        return normalizedQuery
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(term => term.Length > 0)
            .Distinct(StringComparer.Ordinal)
            .Take(maxTerms)
            .ToList();
    }
}
