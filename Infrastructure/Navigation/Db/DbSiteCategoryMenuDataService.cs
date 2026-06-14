using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Data;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Navigation.Db;

public sealed class DbSiteCategoryMenuDataService(EcommerceDbContext dbContext) : ISiteCategoryMenuDataService
{
    public Task<SiteCategoryMenuViewModel> GetMenuAsync(
        CancellationToken cancellationToken = default)
    {
        _ = dbContext;
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new SiteCategoryMenuViewModel
        {
            Items = []
        });
    }
}
