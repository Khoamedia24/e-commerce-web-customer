using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Products;
using e_commerce_web_customer.Data;

namespace e_commerce_web_customer.Infrastructure.Products.Db;

public sealed class DbProductCatalog(
    EcommerceDbContext dbContext) : IProductCatalog
{
    public Task<IReadOnlyList<ProductReadModel>> SearchAsync(
        string? query,
        CancellationToken cancellationToken = default)
    {
        _ = dbContext;
        _ = query;
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult<IReadOnlyList<ProductReadModel>>([]);
    }

    public Task<ProductReadModel?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        _ = dbContext;
        _ = id;
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult<ProductReadModel?>(null);
    }
}
