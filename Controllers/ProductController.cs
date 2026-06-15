using e_commerce_web_customer.Application.Product;
using Microsoft.AspNetCore.Mvc;

namespace e_commerce_web_customer.Controllers;

[Route("product")]
public sealed class ProductController : Controller
{
    private readonly IProductDetailViewModelFactory _productDetailViewModelFactory;

    public ProductController(IProductDetailViewModelFactory productDetailViewModelFactory)
    {
        _productDetailViewModelFactory = productDetailViewModelFactory;
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> Details(
        string slug,
        [FromQuery] string? variant,
        CancellationToken cancellationToken)
    {
        var viewModel = await _productDetailViewModelFactory.CreateAsync(
            slug,
            variant,
            cancellationToken);

        if (viewModel is null)
        {
            return NotFound();
        }

        return View(viewModel);
    }
}
