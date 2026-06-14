using e_commerce_web_customer.Application.Catalog;
using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.ViewModels.Catalog;

namespace e_commerce_web_customer.Infrastructure.Catalog.Db;

public sealed class DbCategoryPageViewModelFactory(
    ICategoryPageDataService categoryPageDataService) : ICategoryPageViewModelFactory
{
    public Task<CategoryPageViewModel?> CreateAsync(
        CategoryPageRequest request,
        CancellationToken cancellationToken = default)
    {
        return categoryPageDataService.CreateCategoryPageAsync(request, cancellationToken);
    }
}
