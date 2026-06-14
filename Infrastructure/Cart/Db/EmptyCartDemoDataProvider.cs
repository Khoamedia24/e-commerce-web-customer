using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.ViewModels.Cart;
using e_commerce_web_customer.ViewModels.Checkout;

namespace e_commerce_web_customer.Infrastructure.Cart.Db;

public sealed class EmptyCartDemoDataProvider : ICartDemoDataProvider
{
    public Task<IReadOnlyList<CartItemViewModel>> GetCartItemsAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<CartItemViewModel>>([]);
    }

    public Task<IReadOnlyList<CheckoutItemViewModel>> GetCheckoutItemsAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<CheckoutItemViewModel>>([]);
    }
}
