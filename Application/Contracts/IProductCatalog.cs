using e_commerce_web_customer.Application.Products;

namespace e_commerce_web_customer.Application.Contracts;

public interface IProductCatalog
{
    Task<IReadOnlyList<ProductReadModel>> SearchAsync(
        ProductCatalogSearchRequest request,
        CancellationToken cancellationToken = default);

    Task<ProductReadModel?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default);
}

public sealed record ProductCatalogSearchRequest(
    string? Query,
    int? Limit = null,
    ProductCatalogSearchScope Scope = ProductCatalogSearchScope.Products);

public enum ProductCatalogSearchScope
{
    Products,
    Variants
}
