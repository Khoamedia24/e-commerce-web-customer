using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Services;

namespace e_commerce_web_customer.Infrastructure.MockData;

/// <summary>
/// Mock implementation that implicitly trusts the client's payload.
/// Suitable ONLY for UI templating where no backend database exists yet.
/// </summary>
public sealed class MockCartItemValidator : ICartItemValidator
{
    public Task<CartSessionItem> ValidateAsync(CartSessionItem requestItem)
    {
        // In Mock mode, we just return the item exactly as the client sent it.
        // The client provides the price, name, and image.
        return Task.FromResult(requestItem);
    }
}
