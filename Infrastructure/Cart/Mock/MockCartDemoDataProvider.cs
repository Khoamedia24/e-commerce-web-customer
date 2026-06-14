using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.ViewModels.Cart;
using e_commerce_web_customer.ViewModels.Checkout;

namespace e_commerce_web_customer.Infrastructure.Cart.Mock;

public sealed class MockCartDemoDataProvider : ICartDemoDataProvider
{
    public Task<IReadOnlyList<CartItemViewModel>> GetCartItemsAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<CartItemViewModel> items = MockCartData.GetCartDemoItems();
        return Task.FromResult(items);
    }

    public Task<IReadOnlyList<CheckoutItemViewModel>> GetCheckoutItemsAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<CheckoutItemViewModel> items = MockCartData.GetCheckoutDemoItems();
        return Task.FromResult(items);
    }
}
