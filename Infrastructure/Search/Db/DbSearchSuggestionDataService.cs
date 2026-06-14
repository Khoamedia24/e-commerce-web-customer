using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Products;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Search.Db;

public sealed class DbSearchSuggestionDataService(IProductCatalog productCatalog)
    : ISearchSuggestionDataService
{
    public async Task<HeaderSearchViewModel> GetInitialSuggestionsAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await productCatalog.SearchAsync(null, cancellationToken);

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
        var normalizedQuery = query?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalizedQuery))
        {
            return new SearchSuggestionResultsViewModel
            {
                Query = normalizedQuery
            };
        }

        var products = await productCatalog.SearchAsync(query, cancellationToken);

        return new SearchSuggestionResultsViewModel
        {
            Query = normalizedQuery,
            Products = products
                .Take(6)
                .Select(ProductViewModelMapper.ToSearchSuggestion)
                .ToList()
        };
    }
}
