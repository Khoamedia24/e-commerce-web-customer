using e_commerce_web_customer.Application.Search;
using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Products;
using e_commerce_web_customer.ViewModels.Search;

namespace e_commerce_web_customer.Infrastructure.Search.Mock;

public sealed class MockSearchResultProvider(
    IProductCatalog productCatalog) : ISearchResultProvider
{
    private const string DefaultQuery = "samsung";
    private const int SamsungMockTotalCount = 4918;

    public async Task<SearchResultPageViewModel> SearchAsync(
        SearchResultRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = string.IsNullOrWhiteSpace(request.Query)
            ? DefaultQuery
            : request.Query.Trim();
        var sort = NormalizeSort(request.Sort);
        var products = await productCatalog.SearchAsync(
            new ProductCatalogSearchRequest(
                query,
                Scope: ProductCatalogSearchScope.Variants),
            cancellationToken);
        if (products.Count == 0 && IsSamsungQuery(query))
        {
            products = await productCatalog.SearchAsync(
                new ProductCatalogSearchRequest(
                    DefaultQuery,
                    Scope: ProductCatalogSearchScope.Variants),
                cancellationToken);
        }

        products = sort switch
        {
            "price-desc" => products.OrderByDescending(product => product.CurrentPrice).ToList(),
            "price-asc" => products.OrderBy(product => product.CurrentPrice).ToList(),
            _ => products
        };

        return new SearchResultPageViewModel
        {
            Query = query,
            TotalCount = IsSamsungQuery(query) ? SamsungMockTotalCount : products.Count,
            InitialProductCount = 25,
            Categories = CreateCategories(query),
            SortOptions = CreateSortOptions(query, sort),
            Products = products.Select(ProductViewModelMapper.ToProductCard).ToList()
        };
    }

    private static IReadOnlyList<SearchResultCategoryViewModel> CreateCategories(string query)
    {
        var categories = new[]
        {
            ("Tất cả", DefaultQuery),
            ("Samsung S24 Ultra cũ", "samsung s24 ultra cũ"),
            ("điện thoại samsung cũ giá rẻ", "điện thoại samsung cũ giá rẻ"),
            ("Samsung Galaxy S cũ", "Samsung Galaxy S cũ"),
            ("Tivi Samsung", "Tivi Samsung"),
            ("Điện thoại Samsung Galaxy", "Điện thoại Samsung Galaxy"),
            ("Samsung Z5 cũ", "Samsung Z5 cũ")
        };
        var normalizedQuery = SearchTextNormalizer.Normalize(query);

        return categories.Select(category => new SearchResultCategoryViewModel
        {
            Label = category.Item1,
            Url = $"/search?q={Uri.EscapeDataString(category.Item2)}",
            IsActive = string.Equals(category.Item1, "Tất cả", StringComparison.Ordinal)
                ? IsSamsungQuery(query) && normalizedQuery == DefaultQuery
                : SearchTextNormalizer.Normalize(category.Item2) == normalizedQuery
        }).ToList();
    }

    private static IReadOnlyList<SearchResultSortOptionViewModel> CreateSortOptions(
        string query,
        string activeSort)
    {
        return
        [
            SortOption("Liên quan", "relevance", "relevance", query, activeSort),
            SortOption("Giá cao", "price-desc", "sort-desc", query, activeSort),
            SortOption("Giá thấp", "price-asc", "sort-asc", query, activeSort)
        ];
    }

    private static SearchResultSortOptionViewModel SortOption(
        string label,
        string value,
        string icon,
        string query,
        string activeSort)
    {
        var url = value == "relevance"
            ? $"/search?q={Uri.EscapeDataString(query)}"
            : $"/search?q={Uri.EscapeDataString(query)}&sort={Uri.EscapeDataString(value)}";

        return new SearchResultSortOptionViewModel
        {
            Label = label,
            Value = value,
            Icon = icon,
            Url = url,
            IsActive = string.Equals(value, activeSort, StringComparison.OrdinalIgnoreCase)
        };
    }

    private static bool IsSamsungQuery(string query)
    {
        return SearchTextNormalizer.Normalize(query)
            .Contains("samsung", StringComparison.Ordinal);
    }

    private static string NormalizeSort(string? sort)
    {
        var normalizedSort = sort?.Trim().ToLowerInvariant();
        return normalizedSort is "price-desc" or "price-asc"
            ? normalizedSort
            : "relevance";
    }

}
