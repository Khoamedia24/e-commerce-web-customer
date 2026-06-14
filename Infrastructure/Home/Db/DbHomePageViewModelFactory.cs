using e_commerce_web_customer.Application.Home;
using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Navigation;
using e_commerce_web_customer.ViewModels.Home;

namespace e_commerce_web_customer.Infrastructure.Home.Db;

public sealed class DbHomePageViewModelFactory(
    ISiteCategoryMenuProvider categoryMenuProvider,
    IHomePageDataService homePageDataService) : IHomePageViewModelFactory
{
    public async Task<HomeIndexViewModel> CreateAsync(
        CancellationToken cancellationToken = default)
    {
        var categoryMenu = await categoryMenuProvider.GetMenuAsync(cancellationToken);
        return await homePageDataService.CreateHomePageAsync(
            categoryMenu,
            cancellationToken);
    }
}
