using e_commerce_web_customer.Application.Search;
using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.ViewModels.Search;

namespace e_commerce_web_customer.Infrastructure.Search.Db;

public sealed class DbSearchResultProvider(
    ISearchResultDataService searchResultDataService) : ISearchResultProvider
{
    public async Task<SearchResultPageViewModel> SearchAsync(
        SearchResultRequest request,
        CancellationToken cancellationToken = default)
    {
        return await searchResultDataService.SearchAsync(request, cancellationToken);
    }
}
