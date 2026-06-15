using e_commerce_web_customer.Application.Home;
using e_commerce_web_customer.Application.Navigation;
using e_commerce_web_customer.Infrastructure.Home.Content;
using e_commerce_web_customer.ViewModels.Home;

namespace e_commerce_web_customer.Infrastructure.Home.Mock;

public sealed class MockHomePageViewModelFactory : IHomePageViewModelFactory
{
    private readonly ISiteCategoryMenuProvider _categoryMenuProvider;

    public MockHomePageViewModelFactory(ISiteCategoryMenuProvider categoryMenuProvider)
    {
        _categoryMenuProvider = categoryMenuProvider;
    }

    public async Task<HomeIndexViewModel> CreateAsync(
        CancellationToken cancellationToken = default)
    {
        var categoryMenu = await _categoryMenuProvider.GetMenuAsync(cancellationToken);

        return new HomeIndexViewModel
        {
            Hero = HomeHeroViewModelFactory.Create(categoryMenu.Items),
            FeaturedCategorySections = [PhoneCategorySectionFactory.Create()],
            AccessoryDirectory = AccessoryDirectoryFactory.Create(),
            AdditionalCategorySections =
            [
                ComputerCategorySectionFactory.Create(),
                AudioWearablesCategorySectionFactory.Create()
            ]
        };
    }
}
