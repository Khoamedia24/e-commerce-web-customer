using System.Text.Json;
using e_commerce_web_customer.Application.Constants;
using e_commerce_web_customer.Application.Contracts;

namespace e_commerce_web_customer.Application.Services;

/// <summary>
/// Stores and retrieves the cart as a JSON blob in the session storage.
/// </summary>
public sealed class CartSessionService(ISessionStorage sessionStorage)
{
    // Write

    /// <summary>Persist the list of cart items to session.</summary>
    public void Save(IEnumerable<CartSessionItem> items)
    {
        var normalizedItems = items
            .Select(Normalize)
            .Where(IsValid)
            .ToList();

        if (normalizedItems.Count == 0)
        {
            Clear();
            return;
        }

        var json = JsonSerializer.Serialize(normalizedItems);
        sessionStorage.SetString(SessionKeys.CartSession, json);
    }

    public IReadOnlyList<CartSessionItem> AddOrUpdate(CartSessionItem item)
    {
        var normalizedItem = Normalize(item);

        if (!IsValid(normalizedItem))
        {
            throw new ArgumentException("Cart item is invalid.", nameof(item));
        }

        var items = Load()
            .Select(Normalize)
            .Where(IsValid)
            .ToList();

        var existingItem = items.FirstOrDefault(existing =>
            string.Equals(existing.Id, normalizedItem.Id, StringComparison.OrdinalIgnoreCase)
            && string.Equals(existing.Variant, normalizedItem.Variant, StringComparison.OrdinalIgnoreCase));

        if (existingItem is null)
        {
            items.Add(normalizedItem);
        }
        else
        {
            existingItem.Name = normalizedItem.Name;
            existingItem.ProductUrl = normalizedItem.ProductUrl;
            existingItem.ImageUrl = normalizedItem.ImageUrl;
            existingItem.ImageAlt = normalizedItem.ImageAlt;
            existingItem.UnitPrice = normalizedItem.UnitPrice;
            existingItem.Quantity += normalizedItem.Quantity;
        }

        Save(items);
        return Load();
    }

    // Read

    /// <summary>
    /// Returns cart items from session.
    /// Returns an empty list when the session has no cart data.
    /// </summary>
    public IReadOnlyList<CartSessionItem> Load()
    {
        var json = sessionStorage.GetString(SessionKeys.CartSession);
        if (string.IsNullOrWhiteSpace(json)) return [];

        try
        {
            return JsonSerializer.Deserialize<List<CartSessionItem>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public void Clear() => sessionStorage.Remove(SessionKeys.CartSession);

    // ── Buy Now (Separate Session) ────────────────────────────────────

    public void SaveBuyNow(CartSessionItem item)
    {
        var normalized = Normalize(item);
        if (!IsValid(normalized)) return;

        var json = JsonSerializer.Serialize(new List<CartSessionItem> { normalized });
        sessionStorage.SetString(SessionKeys.BuyNowSession, json);
    }

    public IReadOnlyList<CartSessionItem> LoadBuyNow()
    {
        var json = sessionStorage.GetString(SessionKeys.BuyNowSession);
        if (string.IsNullOrWhiteSpace(json)) return [];
        try
        {
            return JsonSerializer.Deserialize<List<CartSessionItem>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public void ClearBuyNow() => sessionStorage.Remove(SessionKeys.BuyNowSession);

    private static CartSessionItem Normalize(CartSessionItem item) => new()
    {
        Id = (item.Id ?? string.Empty).Trim(),
        Name = (item.Name ?? string.Empty).Trim(),
        ProductUrl = (item.ProductUrl ?? string.Empty).Trim(),
        ImageUrl = (item.ImageUrl ?? string.Empty).Trim(),
        ImageAlt = (item.ImageAlt ?? string.Empty).Trim(),
        Variant = (item.Variant ?? string.Empty).Trim(),
        UnitPrice = Math.Max(0m, item.UnitPrice),
        Quantity = Math.Max(1, item.Quantity),
    };

    private static bool IsValid(CartSessionItem item)
    {
        return !string.IsNullOrWhiteSpace(item.Id)
            && !string.IsNullOrWhiteSpace(item.Name)
            && item.UnitPrice > 0m;
    }
}

/// <summary>
/// Minimal cart item stored in session for cart and checkout flows.
/// </summary>
public sealed class CartSessionItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ProductUrl { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string ImageAlt { get; set; } = string.Empty;
    public string Variant { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; } = 1;
}
