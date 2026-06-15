using e_commerce_web_customer.ViewModels.Product;

namespace e_commerce_web_customer.Application.Contracts;

public interface IProductDetailDataService
{
    Task<ProductDetailViewModel?> CreateProductDetailAsync(
        string slug,
        string? variantKey = null,
        CancellationToken cancellationToken = default);
}
