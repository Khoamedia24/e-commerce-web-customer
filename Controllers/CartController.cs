using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Constants;
using e_commerce_web_customer.Application.Services;
using e_commerce_web_customer.ViewModels.Cart;
using Microsoft.AspNetCore.Mvc;

namespace e_commerce_web_customer.Controllers;

public sealed class CartController(
    CartSessionService cartSession,
    ICartItemValidator cartItemValidator,
    ICartPersistenceService cartPersistenceService,
    ICartDemoDataProvider demoDataProvider) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(
        bool demo = false,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<CartItemViewModel> items;
        if (demo)
        {
            items = await demoDataProvider.GetCartItemsAsync(cancellationToken);
        }
        else
        {
            var sessionItems = cartSession.Load();
            var userEmail = GetLoggedInUserEmail();

            if (sessionItems.Count == 0 && userEmail is not null)
            {
                sessionItems = await cartPersistenceService.LoadAsync(
                    userEmail,
                    cancellationToken);
                cartSession.Save(sessionItems);
            }
            else if (sessionItems.Count > 0 && userEmail is not null)
            {
                await cartPersistenceService.SaveAsync(
                    userEmail,
                    sessionItems,
                    cancellationToken);
            }

            items = BuildCartItems(sessionItems);
        }

        return View(new CartIndexViewModel
        {
            Items = items
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddItem(
        [FromBody] CartSessionItem? item,
        CancellationToken cancellationToken)
    {
        var userEmail = GetLoggedInUserEmail();

        if (item is null)
        {
            return BadRequest(new { error = "Cart item is invalid." });
        }

        try
        {
            if (userEmail is not null)
            {
                await EnsureCartLoadedAsync(userEmail, cancellationToken);
            }

            var validatedItem = await cartItemValidator.ValidateAsync(
                item,
                cancellationToken);
            var updatedItems = cartSession.AddOrUpdate(validatedItem);
            var items = await ValidateCartItemsAsync(
                updatedItems,
                cancellationToken);
            cartSession.Save(items);
            await PersistCartForLoggedInUserAsync(items, cancellationToken);
            return Ok(new { count = CountQuantity(items) });
        }
        catch (CartItemValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BuyNow(
        [FromBody] CartSessionItem? item,
        CancellationToken cancellationToken)
    {
        if (item is null)
        {
            return BadRequest(new { error = "Cart item is invalid." });
        }

        try
        {
            var validatedItem = await cartItemValidator.ValidateAsync(
                item,
                cancellationToken);
            cartSession.SaveBuyNow(validatedItem);

            var items = cartSession.Load(); // Main cart count doesn't change, we just need it for UI
            return Ok(new
            {
                count = CountQuantity(items),
                redirectUrl = Url.Action("Index", "Checkout", new { mode = "buynow" })
            });
        }
        catch (CartItemValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveSession(
        [FromBody] List<CartSessionItem>? items,
        CancellationToken cancellationToken)
    {
        if (items is null)
        {
            return BadRequest(new { error = "Cart is empty." });
        }

        if (items.Count == 0)
        {
            cartSession.Clear();
            await ClearCartForLoggedInUserAsync(cancellationToken);
            return Ok(new { saved = 0, count = 0 });
        }

        var validatedItems = new List<CartSessionItem>();
        foreach (var item in items)
        {
            try
            {
                validatedItems.Add(await cartItemValidator.ValidateAsync(
                    item,
                    cancellationToken));
            }
            catch (CartItemValidationException)
            {
                // Skip invalid items
            }
        }

        cartSession.Save(validatedItems);
        var savedItems = cartSession.Load();

        if (savedItems.Count == 0)
        {
            await ClearCartForLoggedInUserAsync(cancellationToken);
            return BadRequest(new { error = "Cart is empty or all items were invalid." });
        }

        await PersistCartForLoggedInUserAsync(savedItems, cancellationToken);

        return Ok(new
        {
            saved = savedItems.Count,
            count = CountQuantity(savedItems)
        });
    }

    private IReadOnlyList<CartItemViewModel> BuildCartItems(IEnumerable<CartSessionItem> items)
    {
        return items.Select(item => new CartItemViewModel
        {
            Id = item.Id,
            Name = item.Name,
            ProductUrl = ResolveProductUrl(item),
            ImageUrl = item.ImageUrl,
            ImageAlt = item.ImageAlt,
            Variant = item.Variant,
            UnitPrice = item.UnitPrice,
            Quantity = Math.Max(1, item.Quantity)
        }).ToList();
    }

    private string ResolveProductUrl(CartSessionItem item)
    {
        if (!string.IsNullOrWhiteSpace(item.ProductUrl))
        {
            return item.ProductUrl;
        }

        return Url.Action("Details", "Product", new { slug = item.Id }) ?? $"/product/{item.Id}";
    }

    private static int CountQuantity(IEnumerable<CartSessionItem> items)
    {
        return items.Sum(item => Math.Max(1, item.Quantity));
    }

    private async Task<IReadOnlyList<CartSessionItem>> ValidateCartItemsAsync(
        IEnumerable<CartSessionItem> items,
        CancellationToken cancellationToken)
    {
        var validatedItems = new List<CartSessionItem>();

        foreach (var item in items)
        {
            try
            {
                validatedItems.Add(await cartItemValidator.ValidateAsync(
                    item,
                    cancellationToken));
            }
            catch (CartItemValidationException)
            {
                // Invalid variants are removed from the cart.
            }
        }

        return validatedItems;
    }

    private async Task PersistCartForLoggedInUserAsync(
        IReadOnlyCollection<CartSessionItem> items,
        CancellationToken cancellationToken)
    {
        var userEmail = GetLoggedInUserEmail();
        if (userEmail is null)
        {
            return;
        }

        await cartPersistenceService.SaveAsync(
            userEmail,
            items,
            cancellationToken);
    }

    private async Task EnsureCartLoadedAsync(
        string userEmail,
        CancellationToken cancellationToken)
    {
        if (cartSession.Load().Count > 0)
        {
            return;
        }

        var persistedItems = await cartPersistenceService.LoadAsync(
            userEmail,
            cancellationToken);
        cartSession.Save(persistedItems);
    }

    private async Task ClearCartForLoggedInUserAsync(
        CancellationToken cancellationToken)
    {
        var userEmail = GetLoggedInUserEmail();
        if (userEmail is null)
        {
            return;
        }

        await cartPersistenceService.ClearAsync(
            userEmail,
            cancellationToken);
    }

    private string? GetLoggedInUserEmail()
    {
        if (HttpContext.Session.GetString(SessionKeys.IsLoggedIn) != "true")
        {
            return null;
        }

        var email = HttpContext.Session.GetString(SessionKeys.UserEmail);
        return string.IsNullOrWhiteSpace(email) ? null : email.Trim();
    }
}
