using e_commerce_web_customer.Application.Search;
using Microsoft.AspNetCore.Mvc;

namespace e_commerce_web_customer.Controllers;

[Route("search")]
public sealed class SearchController(
    ISearchSuggestionProvider searchSuggestionProvider,
    ISearchResultProvider searchResultProvider) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(
        [FromQuery] string? q,
        [FromQuery] string? sort,
        [FromQuery] string? category,
        CancellationToken cancellationToken)
    {
        var model = await searchResultProvider.SearchAsync(
            new SearchResultRequest(q, sort, category),
            cancellationToken);

        return View(model);
    }

    [HttpGet("suggest")]
    public async Task<IActionResult> Suggest(
        [FromQuery] string? q,
        CancellationToken cancellationToken)
    {
        var model = await searchSuggestionProvider.SearchAsync(
            q,
            cancellationToken);
        return Json(model);
    }
}
