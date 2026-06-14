using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Services;
using e_commerce_web_customer.Data;

namespace e_commerce_web_customer.Infrastructure.Cart.Db;

public sealed class DbCartItemValidator(EcommerceDbContext dbContext) : ICartItemValidator
{
    public Task<CartSessionItem> ValidateAsync(
        CartSessionItem requestItem,
        CancellationToken cancellationToken = default)
    {
        _ = dbContext;
        _ = requestItem;
        cancellationToken.ThrowIfCancellationRequested();

        throw new CartItemValidationException(
            "Database cart item validation is not implemented yet.");
    }
}
