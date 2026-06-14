using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Data;
using e_commerce_web_customer.ViewModels.Product;

namespace e_commerce_web_customer.Infrastructure.Products.Db;

public sealed class DbProductDetailDataService(EcommerceDbContext dbContext) : IProductDetailDataService
{
    public Task<ProductDetailViewModel?> CreateProductDetailAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        _ = dbContext;
        _ = slug;
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult<ProductDetailViewModel?>(null);
    }
}
