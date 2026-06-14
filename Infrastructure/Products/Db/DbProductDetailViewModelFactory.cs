using e_commerce_web_customer.Application.Product;
using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.ViewModels.Product;

namespace e_commerce_web_customer.Infrastructure.Products.Db;

public sealed class DbProductDetailViewModelFactory(
    IProductDetailDataService productDetailDataService) : IProductDetailViewModelFactory
{
    public Task<ProductDetailViewModel?> CreateAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        return productDetailDataService.CreateProductDetailAsync(
            slug,
            cancellationToken);
    }
}
