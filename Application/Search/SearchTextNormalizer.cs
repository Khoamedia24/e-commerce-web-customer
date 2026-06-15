using System.Globalization;
using System.Text;

namespace e_commerce_web_customer.Application.Search;

public static class SearchTextNormalizer
{
    public const int MaxQueryLength = 120;

    public static string CleanQuery(string? value)
    {
        var query = value?.Trim() ?? string.Empty;
        return query.Length <= MaxQueryLength
            ? query
            : query[..MaxQueryLength];
    }

    public static string Normalize(string value)
    {
        var text = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(text.Length);

        foreach (var character in text)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            var normalizedCharacter = character == 'đ' ? 'd' : character;
            if (char.IsLetterOrDigit(normalizedCharacter) || char.IsWhiteSpace(normalizedCharacter))
            {
                builder.Append(normalizedCharacter);
            }
        }

        return string.Join(' ', builder.ToString()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}
