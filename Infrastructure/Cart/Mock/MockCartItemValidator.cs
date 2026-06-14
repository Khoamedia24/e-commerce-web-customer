using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Services;

namespace e_commerce_web_customer.Infrastructure.Cart.Mock;

/// <summary>
/// Uses the server-side mock catalog when a product is known.
/// Unknown mock-only products are normalized so existing UI fixtures remain usable.
/// </summary>
public sealed class MockCartItemValidator(
    IProductCatalog productCatalog) : ICartItemValidator
{
    public async Task<CartSessionItem> ValidateAsync(
        CartSessionItem requestItem,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(requestItem.Id))
        {
            throw new CartItemValidationException("Sản phẩm không hợp lệ.");
        }

        var product = await productCatalog.GetByIdAsync(
            requestItem.Id,
            cancellationToken);
        var quantity = Math.Clamp(requestItem.Quantity, 1, 10);

        if (product is not null)
        {
            return new CartSessionItem
            {
                Id = product.Id,
                Name = product.Name,
                ProductUrl = product.ProductUrl,
                ImageUrl = product.ImageUrl,
                ImageAlt = product.ImageAlt,
                Variant = requestItem.Variant?.Trim() ?? string.Empty,
                UnitPrice = product.CurrentPrice,
                Quantity = quantity
            };
        }

        if (string.IsNullOrWhiteSpace(requestItem.Name) || requestItem.UnitPrice <= 0)
        {
            throw new CartItemValidationException("Sản phẩm không tồn tại trong catalog mock.");
        }

        return new CartSessionItem
        {
            Id = requestItem.Id.Trim(),
            Name = requestItem.Name.Trim(),
            ProductUrl = requestItem.ProductUrl?.Trim() ?? string.Empty,
            ImageUrl = requestItem.ImageUrl?.Trim() ?? string.Empty,
            ImageAlt = requestItem.ImageAlt?.Trim() ?? requestItem.Name.Trim(),
            Variant = requestItem.Variant?.Trim() ?? string.Empty,
            UnitPrice = requestItem.UnitPrice,
            Quantity = quantity
        };
    }
}
