using e_commerce_web_customer.Application.Search;
using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Products;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Search.Mock;

public sealed class MockSearchSuggestionProvider(
    IProductCatalog productCatalog) : ISearchSuggestionProvider
{
    private static readonly IReadOnlyList<SearchQuickSeed> RecentSearches =
    [
        new("iphone", "/search?q=iphone", null, null, "iphone apple ios")
    ];

    private static readonly IReadOnlyList<SearchQuickSeed> TrendingSearches =
    [
        new("iPhone 17 Series", "/search?q=iphone%2017", "/images/products/phone/phone-orange-cutout.png", "iPhone 17 Series", "iphone apple ios dien thoai"),
        new("Xiaomi 17T | 17T Pro", "/search?q=xiaomi%2017t", "/images/products/phone/phone-camera-cutout.png", "Xiaomi 17T", "xiaomi 17t 17t pro dien thoai"),
        new("MacBook Neo", "/search?q=macbook%20neo", "/images/products/computing/laptop-08.webp", "MacBook Neo", "macbook laptop apple may tinh xach tay"),
        new("OPPO Find X9s | X9 Ultra", "/search?q=oppo%20find%20x9", "/images/products/phone/phone-camera.webp", "OPPO Find X9", "oppo find x9 x9s ultra dien thoai"),
        new("Loa Sony ULT Field 1", "/search?q=loa%20sony", "/images/products/audio-wearables/audio-04.webp", "Loa Sony", "loa sony bluetooth am thanh"),
        new("Huawei Watch Fit 5", "/search?q=huawei%20watch", "/images/products/audio-wearables/watch-01.webp", "Huawei Watch Fit 5", "huawei watch dong ho smartwatch"),
        new("Máy chơi game Sony", "/search?q=may%20choi%20game%20sony", "/images/categories/accessories/gaming-playstation.webp", "Máy chơi game Sony", "sony playstation may choi game gaming"),
        new("Tivi giá rẻ", "/search?q=tivi%20gia%20re", "/images/home/hero-smartphones.webp", "Tivi giá rẻ", "tivi tv gia re"),
        new("Quạt cầm tay", "/search?q=quat%20cam%20tay", "/images/categories/accessories/phone-pouches.webp", "Quạt cầm tay", "quat cam tay phu kien"),
        new("Camera IP 360 độ 5MP IMOU", "/search?q=camera%20imou", "/images/categories/accessories/security-camera.webp", "Camera IMOU", "camera imou ip 360 5mp")
    ];

    public Task<HeaderSearchViewModel> GetInitialSuggestionsAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new HeaderSearchViewModel
        {
            RecentSearches = RecentSearches.Select(ToViewModel).ToList(),
            TrendingSearches = TrendingSearches.Select(ToViewModel).ToList()
        });
    }

    public async Task<SearchSuggestionResultsViewModel> SearchAsync(
        string? query,
        CancellationToken cancellationToken = default)
    {
        var normalizedQuery = SearchTextNormalizer.Normalize(query ?? string.Empty);
        if (normalizedQuery.Length < 2)
        {
            return new SearchSuggestionResultsViewModel
            {
                Query = query?.Trim() ?? string.Empty
            };
        }

        var products = await productCatalog.SearchAsync(
            new ProductCatalogSearchRequest(query, 6),
            cancellationToken);

        return new SearchSuggestionResultsViewModel
        {
            Query = query?.Trim() ?? string.Empty,
            Suggestions = products
                .DistinctBy(product => SearchTextNormalizer.Normalize(product.Name))
                .Take(4)
                .Select(product => new SearchQuickLinkViewModel
                {
                    Label = product.Name,
                    Url = $"/search?q={Uri.EscapeDataString(product.Name)}",
                    ImageUrl = product.ImageUrl,
                    ImageAlt = product.ImageAlt
                })
                .ToList(),
            Products = products
                .Take(6)
                .Select(ProductViewModelMapper.ToSearchSuggestion)
                .ToList()
        };
    }

    private static SearchQuickLinkViewModel ToViewModel(SearchQuickSeed item)
    {
        return new SearchQuickLinkViewModel
        {
            Label = item.Label,
            Url = item.Url,
            ImageUrl = item.ImageUrl,
            ImageAlt = item.ImageAlt
        };
    }

    private sealed record SearchQuickSeed(
        string Label,
        string Url,
        string? ImageUrl,
        string? ImageAlt,
        string SearchText);
}
