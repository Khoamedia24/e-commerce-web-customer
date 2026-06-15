using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Services;

namespace e_commerce_web_customer.Infrastructure.Cart.Mock;

public sealed class NoOpCartPersistenceService : ICartPersistenceService
{
    public Task<IReadOnlyList<CartSessionItem>> LoadAsync(
        string userEmail,
        CancellationToken cancellationToken = default)
    {
        _ = userEmail;
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<CartSessionItem>>([]);
    }

    public Task SaveAsync(
        string userEmail,
        IReadOnlyCollection<CartSessionItem> items,
        CancellationToken cancellationToken = default)
    {
        _ = userEmail;
        _ = items;
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }

    public Task ClearAsync(
        string userEmail,
        CancellationToken cancellationToken = default)
    {
        _ = userEmail;
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}
