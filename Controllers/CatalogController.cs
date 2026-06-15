using e_commerce_web_customer.Application.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace e_commerce_web_customer.Controllers;

[Route("catalog")]
public sealed class CatalogController(ICategoryPageViewModelFactory categoryPageFactory) : Controller
{
    [HttpGet("")]
    public Task<IActionResult> Index(
        [FromQuery] string? cat,
        [FromQuery] string? brand,
        [FromQuery] string? sort,
        [FromQuery] bool inStock,
        [FromQuery] bool isNew,
        CancellationToken cancellationToken)
    {
        return RenderCategoryAsync(cat, brand, sort, inStock, isNew, cancellationToken);
    }

    [HttpGet("{slug}")]
    public Task<IActionResult> Category(
        string slug,
        [FromQuery] string? brand,
        [FromQuery] string? sort,
        [FromQuery] bool inStock,
        [FromQuery] bool isNew,
        CancellationToken cancellationToken)
    {
        return RenderCategoryAsync(slug, brand, sort, inStock, isNew, cancellationToken);
    }

    private async Task<IActionResult> RenderCategoryAsync(
        string? slug,
        string? brand,
        string? sort,
        bool inStock,
        bool isNew,
        CancellationToken cancellationToken)
    {
        var normalizedSlug = string.IsNullOrWhiteSpace(slug)
            ? "phone"
            : slug.Trim().ToLowerInvariant();

        var filters = Request.Query
            .Where(item => item.Key.StartsWith("f_", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(
                item => item.Key[2..].Trim().ToLowerInvariant(),
                item => (IReadOnlyList<string>)item.Value
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .Select(value => value!.Trim().ToLowerInvariant())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList(),
                StringComparer.OrdinalIgnoreCase);

        var model = await categoryPageFactory.CreateAsync(
            new CategoryPageRequest(
                normalizedSlug,
                brand,
                sort,
                filters,
                inStock,
                isNew),
            cancellationToken);

        return model is null ? NotFound() : View("Category", model);
    }
}
