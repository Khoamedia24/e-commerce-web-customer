using e_commerce_web_customer.ViewModels.Search;

namespace e_commerce_web_customer.Application.Search;

public interface ISearchResultProvider
{
    Task<SearchResultPageViewModel> SearchAsync(
        SearchResultRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record SearchResultRequest(
    string? Query,
    string? Sort,
    string? Category = null);
