using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Products;
using e_commerce_web_customer.Application.Search;
using e_commerce_web_customer.ViewModels.Search;

namespace e_commerce_web_customer.Infrastructure.Search.Db;

public sealed class DbSearchResultDataService(IProductCatalog productCatalog)
    : ISearchResultDataService
{
    public async Task<SearchResultPageViewModel> SearchAsync(
        SearchResultRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = request.Query?.Trim() ?? string.Empty;
        var sort = NormalizeSort(request.Sort);
        var products = await productCatalog.SearchAsync(query, cancellationToken);

        products = sort switch
        {
            "price-desc" => products.OrderByDescending(item => item.CurrentPrice).ToList(),
            "price-asc" => products.OrderBy(item => item.CurrentPrice).ToList(),
            _ => products
        };

        return new SearchResultPageViewModel
        {
            Query = query,
            TotalCount = products.Count,
            InitialProductCount = 25,
            Categories =
            [
                new()
                {
                    Label = "Tat ca",
                    Url = $"/search?q={Uri.EscapeDataString(query)}",
                    IsActive = true
                }
            ],
            SortOptions =
            [
                CreateSortOption("Lien quan", "relevance", query, sort),
                CreateSortOption("Gia cao", "price-desc", query, sort),
                CreateSortOption("Gia thap", "price-asc", query, sort)
            ],
            Products = products.Select(ProductViewModelMapper.ToProductCard).ToList()
        };
    }

    private static SearchResultSortOptionViewModel CreateSortOption(
        string label,
        string value,
        string query,
        string activeSort)
    {
        var url = value == "relevance"
            ? $"/search?q={Uri.EscapeDataString(query)}"
            : $"/search?q={Uri.EscapeDataString(query)}&sort={value}";

        return new SearchResultSortOptionViewModel
        {
            Label = label,
            Value = value,
            Url = url,
            IsActive = value == activeSort
        };
    }

    private static string NormalizeSort(string? sort)
    {
        return sort?.Trim().ToLowerInvariant() switch
        {
            "price-desc" => "price-desc",
            "price-asc" => "price-asc",
            _ => "relevance"
        };
    }
}
