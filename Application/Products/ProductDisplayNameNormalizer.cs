using System.Text.RegularExpressions;

namespace e_commerce_web_customer.Application.Products;

public static class ProductDisplayNameNormalizer
{
    private static readonly Regex VariantCapacityPattern = new(
        @"\b\d+\s*(?:GB|TB)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly Regex WhitespacePattern = new(
        @"\s{2,}",
        RegexOptions.CultureInvariant);

    public static string ToBaseName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }

        var cleaned = VariantCapacityPattern.Replace(name, " ");
        cleaned = WhitespacePattern.Replace(cleaned, " ").Trim();
        cleaned = cleaned.Trim('-', '/', '|', ' ');

        return string.IsNullOrWhiteSpace(cleaned)
            ? name.Trim()
            : cleaned;
    }
}
