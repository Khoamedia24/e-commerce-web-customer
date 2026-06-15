using System.Globalization;
using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Services;
using e_commerce_web_customer.Data;
using e_commerce_web_customer.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace e_commerce_web_customer.Infrastructure.Cart.Db;

public sealed class DbCartPersistenceService(
    EcommerceDbContext dbContext,
    ICartItemValidator cartItemValidator) : ICartPersistenceService
{
    public async Task<IReadOnlyList<CartSessionItem>> LoadAsync(
        string userEmail,
        CancellationToken cancellationToken = default)
    {
        var user = await FindUserAsync(userEmail, cancellationToken);
        if (user is null)
        {
            return [];
        }

        var storedItems = await dbContext.CartItems
            .AsNoTracking()
            .Where(item => item.UserId == user.Id)
            .Include(item => item.ProductVariant)
            .OrderBy(item => item.CreatedAt)
            .ToListAsync(cancellationToken);

        var items = new List<CartSessionItem>();
        foreach (var storedItem in storedItems)
        {
            if (storedItem.ProductVariant is null)
            {
                continue;
            }

            var variantKey = string.IsNullOrWhiteSpace(storedItem.ProductVariant.Code)
                ? storedItem.ProductVariantId.ToString(CultureInfo.InvariantCulture)
                : storedItem.ProductVariant.Code;

            try
            {
                items.Add(await cartItemValidator.ValidateAsync(
                    new CartSessionItem
                    {
                        Id = variantKey,
                        Quantity = storedItem.Quantity
                    },
                    cancellationToken));
            }
            catch (CartItemValidationException)
            {
                // Invalid or inactive variants are omitted from the restored cart.
            }
        }

        return items;
    }

    public async Task SaveAsync(
        string userEmail,
        IReadOnlyCollection<CartSessionItem> items,
        CancellationToken cancellationToken = default)
    {
        var user = await FindUserAsync(userEmail, cancellationToken);
        if (user is null)
        {
            return;
        }

        var requestedItems = items
            .Where(item => !string.IsNullOrWhiteSpace(item.Id))
            .GroupBy(item => item.Id.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => Math.Max(1, group.Sum(item => Math.Max(1, item.Quantity))),
                StringComparer.OrdinalIgnoreCase);

        var numericIds = requestedItems.Keys
            .Select(key => long.TryParse(key, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id)
                ? id
                : (long?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToHashSet();
        var codes = requestedItems.Keys
            .Where(key => !long.TryParse(key, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var variants = requestedItems.Count == 0
            ? []
            : await dbContext.ProductVariants
                .Where(variant =>
                    variant.IsActive
                    && (numericIds.Contains(variant.Id) || codes.Contains(variant.Code)))
                .ToListAsync(cancellationToken);

        var storedItems = await dbContext.CartItems
            .Where(item => item.UserId == user.Id)
            .ToListAsync(cancellationToken);
        var requestedVariantIds = variants.Select(variant => variant.Id).ToHashSet();
        var now = DateTime.UtcNow;

        foreach (var storedItem in storedItems.Where(item => !requestedVariantIds.Contains(item.ProductVariantId)))
        {
            dbContext.CartItems.Remove(storedItem);
        }

        foreach (var variant in variants)
        {
            var variantKey = requestedItems.ContainsKey(variant.Code)
                ? variant.Code
                : variant.Id.ToString(CultureInfo.InvariantCulture);
            var quantity = Math.Clamp(requestedItems[variantKey], 1, Math.Max(1, variant.Quantity));
            var storedItem = storedItems.FirstOrDefault(item => item.ProductVariantId == variant.Id);

            if (storedItem is null)
            {
                dbContext.CartItems.Add(new CartItem
                {
                    UserId = user.Id,
                    ProductVariantId = variant.Id,
                    Quantity = quantity,
                    UnitPrice = variant.Price,
                    DiscountValue = 0m,
                    CreatedAt = now
                });
                continue;
            }

            storedItem.Quantity = quantity;
            storedItem.UnitPrice = variant.Price;
            storedItem.DiscountValue = 0m;
            storedItem.UpdatedAt = now;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ClearAsync(
        string userEmail,
        CancellationToken cancellationToken = default)
    {
        var user = await FindUserAsync(userEmail, cancellationToken);
        if (user is null)
        {
            return;
        }

        var cartItems = await dbContext.CartItems
            .Where(item => item.UserId == user.Id)
            .ToListAsync(cancellationToken);

        if (cartItems.Count == 0)
        {
            return;
        }

        dbContext.CartItems.RemoveRange(cartItems);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private Task<User?> FindUserAsync(
        string userEmail,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = userEmail.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            return Task.FromResult<User?>(null);
        }

        return dbContext.Users.FirstOrDefaultAsync(
            user => user.IsActive && user.Email.ToLower() == normalizedEmail,
            cancellationToken);
    }
}
