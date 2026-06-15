using e_commerce_web_customer.Application.Services;

namespace e_commerce_web_customer.Application.Contracts;

public interface ICartPersistenceService
{
    Task<IReadOnlyList<CartSessionItem>> LoadAsync(
        string userEmail,
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        string userEmail,
        IReadOnlyCollection<CartSessionItem> items,
        CancellationToken cancellationToken = default);

    Task ClearAsync(
        string userEmail,
        CancellationToken cancellationToken = default);
}
