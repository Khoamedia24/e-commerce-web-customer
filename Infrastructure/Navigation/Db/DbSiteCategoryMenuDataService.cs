using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Data;
using e_commerce_web_customer.ViewModels.Shared;
using Microsoft.EntityFrameworkCore;

namespace e_commerce_web_customer.Infrastructure.Navigation.Db;

public sealed class DbSiteCategoryMenuDataService(EcommerceDbContext dbContext) : ISiteCategoryMenuDataService
{
    private static readonly HashSet<int> DoubleCategoryRows = [1, 4, 7, 8];

    public async Task<SiteCategoryMenuViewModel> GetMenuAsync(
        CancellationToken cancellationToken = default)
    {
        var rootCategories = await dbContext.Categories
            .AsNoTracking()
            .Where(category => category.IsActive && category.ParentId == null)
            .OrderBy(category => category.Position)
            .ThenBy(category => category.Id)
            .Select(category => new RootCategory(
                category.Name,
                category.Slug))
            .ToListAsync(cancellationToken);

        var items = BuildMenuItems(rootCategories);

        return new SiteCategoryMenuViewModel
        {
            Items = items
        };
    }

    private static IReadOnlyList<SiteCategoryMenuItemViewModel> BuildMenuItems(
        IReadOnlyList<RootCategory> categories)
    {
        var items = new List<SiteCategoryMenuItemViewModel>();
        var categoryIndex = 0;
        var rowNumber = 1;

        while (categoryIndex < categories.Count)
        {
            var rowSize = DoubleCategoryRows.Contains(rowNumber) ? 2 : 1;
            var rowCategories = categories
                .Skip(categoryIndex)
                .Take(rowSize)
                .ToArray();

            if (rowCategories.Length == 0)
            {
                break;
            }

            items.Add(new SiteCategoryMenuItemViewModel
            {
                Id = BuildItemId(rowNumber, rowCategories),
                Url = BuildCatalogUrl(rowCategories[0].Slug),
                Label = string.Join(", ", rowCategories.Select(category => category.Name)),
                Icon = ResolveIcon(rowCategories),
                IsHighlighted = IsHighlighted(rowCategories),
                CategoryLinks = rowCategories
                    .Select(category => new SiteCategoryMenuLinkViewModel
                    {
                        Label = category.Name,
                        Url = BuildCatalogUrl(category.Slug)
                    })
                    .ToList()
            });

            categoryIndex += rowCategories.Length;
            rowNumber++;
        }

        return items;
    }

    private static string BuildItemId(
        int rowNumber,
        IReadOnlyList<RootCategory> categories)
    {
        var firstSlug = categories[0].Slug;
        return $"site-cat-{rowNumber}-{firstSlug}";
    }

    private static string BuildCatalogUrl(string slug)
    {
        return $"/catalog?cat={Uri.EscapeDataString(slug)}";
    }

    private static bool IsHighlighted(IReadOnlyList<RootCategory> categories)
    {
        return categories.Any(category => ContainsAny(category.Slug, "khuyen-mai", "deal", "sale", "discount"));
    }

    private static string ResolveIcon(IReadOnlyList<RootCategory> categories)
    {
        var firstCategory = categories[0];
        var text = $"{firstCategory.Slug} {firstCategory.Name}";

        if (ContainsAny(text, "dien-thoai", "tablet", "phone", "smartphone"))
        {
            return "phone";
        }

        if (ContainsAny(text, "laptop"))
        {
            return "laptop";
        }

        if (ContainsAny(text, "am-thanh", "audio", "mic", "tai-nghe", "loa"))
        {
            return "audio";
        }

        if (ContainsAny(text, "dong-ho", "watch", "camera"))
        {
            return "watch";
        }

        if (ContainsAny(text, "do-gia-dung", "gia-dung", "lam-dep", "suc-khoe", "appliance", "home"))
        {
            return "home";
        }

        if (ContainsAny(text, "phu-kien", "accessory", "cable", "cap-sac"))
        {
            return "cable";
        }

        if (ContainsAny(text, "pc", "desktop", "may-tinh", "man-hinh", "may-in", "monitor", "printer"))
        {
            return "desktop";
        }

        if (ContainsAny(text, "tivi", "tv", "dien-may"))
        {
            return "tv";
        }

        if (ContainsAny(text, "thu-cu", "doi-moi", "trade"))
        {
            return "swap";
        }

        if (ContainsAny(text, "hang-cu", "used"))
        {
            return "history";
        }

        if (ContainsAny(text, "khuyen-mai", "deal", "sale", "discount"))
        {
            return "discount";
        }

        if (ContainsAny(text, "tin-cong-nghe", "news", "blog"))
        {
            return "news";
        }

        return "phone";
    }

    private static bool ContainsAny(string value, params string[] keywords)
    {
        return keywords.Any(keyword => value.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    private sealed record RootCategory(string Name, string Slug);
}
