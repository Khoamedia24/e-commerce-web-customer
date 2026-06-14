using e_commerce_web_customer.Application.Search;
using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Search.Db;

public sealed class DbSearchSuggestionProvider(
    ISearchSuggestionDataService searchSuggestionDataService) : ISearchSuggestionProvider
{
    public Task<HeaderSearchViewModel> GetInitialSuggestionsAsync(
        CancellationToken cancellationToken = default)
    {
        return searchSuggestionDataService.GetInitialSuggestionsAsync(
            cancellationToken);
    }

    public Task<SearchSuggestionResultsViewModel> SearchAsync(
        string? query,
        CancellationToken cancellationToken = default)
    {
        return searchSuggestionDataService.SearchAsync(
            query,
            cancellationToken);
    }
}
