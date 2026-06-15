using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Products;
using e_commerce_web_customer.Application.Search;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Search.Db;

public sealed class DbSearchSuggestionDataService(IProductCatalog productCatalog)
    : ISearchSuggestionDataService
{
    public async Task<HeaderSearchViewModel> GetInitialSuggestionsAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await productCatalog.SearchAsync(
            new ProductCatalogSearchRequest(null, 8),
            cancellationToken);

        return new HeaderSearchViewModel
        {
            TrendingSearches = products.Take(8).Select(product =>
                new SearchQuickLinkViewModel
                {
                    Label = product.Name,
                    Url = product.ProductUrl,
                    ImageUrl = product.ImageUrl,
                    ImageAlt = product.ImageAlt
                }).ToList()
        };
    }

    public async Task<SearchSuggestionResultsViewModel> SearchAsync(
        string? query,
        CancellationToken cancellationToken = default)
    {
        var normalizedQuery = SearchTextNormalizer.CleanQuery(query);
        if (SearchTextNormalizer.Normalize(normalizedQuery).Length < 2)
        {
            return new SearchSuggestionResultsViewModel
            {
                Query = normalizedQuery
            };
        }

        var products = await productCatalog.SearchAsync(
            new ProductCatalogSearchRequest(normalizedQuery, 6),
            cancellationToken);

        var normalizedSearch = SearchTextNormalizer.Normalize(normalizedQuery);
        var suggestions = products
            .Where(product => !string.Equals(
                SearchTextNormalizer.Normalize(product.Name),
                normalizedSearch,
                StringComparison.Ordinal))
            .DistinctBy(
                product => SearchTextNormalizer.Normalize(product.Name))
            .Take(4)
            .Select(product => new SearchQuickLinkViewModel
            {
                Label = product.Name,
                Url = $"/search?q={Uri.EscapeDataString(product.Name)}",
                ImageUrl = product.ImageUrl,
                ImageAlt = product.ImageAlt
            })
            .ToList();

        return new SearchSuggestionResultsViewModel
        {
            Query = normalizedQuery,
            Suggestions = suggestions,
            Products = products
                .Take(6)
                .Select(ProductViewModelMapper.ToSearchSuggestion)
                .ToList()
        };
    }
}
