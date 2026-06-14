using e_commerce_web_customer.Application.Navigation;
using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Navigation.Db;

public sealed class DbSiteCategoryMenuProvider(
    ISiteCategoryMenuDataService siteCategoryMenuDataService) : ISiteCategoryMenuProvider
{
    public Task<SiteCategoryMenuViewModel> GetMenuAsync(
        CancellationToken cancellationToken = default)
    {
        return siteCategoryMenuDataService.GetMenuAsync(cancellationToken);
    }
}
