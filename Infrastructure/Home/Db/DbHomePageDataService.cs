using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Data;
using e_commerce_web_customer.ViewModels.Home;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Home.Db;

public sealed class DbHomePageDataService(EcommerceDbContext dbContext) : IHomePageDataService
{
    public Task<HomeIndexViewModel> CreateHomePageAsync(
        SiteCategoryMenuViewModel categoryMenu,
        CancellationToken cancellationToken = default)
    {
        _ = dbContext;
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new HomeIndexViewModel
        {
            Hero = new HomeHeroViewModel
            {
                Categories = categoryMenu.Items,
                CampaignTabs = [],
                Slides = [],
                PromoTiles = [],
                BenefitGroups = []
            },
            FeaturedCategorySections = [],
            AccessoryDirectory = new CategoryDirectoryViewModel
            {
                Id = "db-accessory-directory",
                Title = "Database accessory directory",
                ViewAllUrl = "",
                Items = []
            },
            AdditionalCategorySections = []
        });
    }
}
