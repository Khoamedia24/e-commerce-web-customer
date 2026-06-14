using e_commerce_web_customer.Application.Account;
using e_commerce_web_customer.Application.Catalog;
using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Home;
using e_commerce_web_customer.Application.Navigation;
using e_commerce_web_customer.Application.Product;
using e_commerce_web_customer.Application.Search;
using e_commerce_web_customer.Data;
using e_commerce_web_customer.Infrastructure.Account.Db;
using e_commerce_web_customer.Infrastructure.Account.Mock;
using e_commerce_web_customer.Infrastructure.Cart.Db;
using e_commerce_web_customer.Infrastructure.Cart.Mock;
using e_commerce_web_customer.Infrastructure.Catalog.Db;
using e_commerce_web_customer.Infrastructure.Catalog.Mock;
using e_commerce_web_customer.Infrastructure.Home.Db;
using e_commerce_web_customer.Infrastructure.Home.Mock;
using e_commerce_web_customer.Infrastructure.Navigation.Db;
using e_commerce_web_customer.Infrastructure.Navigation.Mock;
using e_commerce_web_customer.Infrastructure.Orders.Db;
using e_commerce_web_customer.Infrastructure.Orders.Mock;
using e_commerce_web_customer.Infrastructure.Products.Db;
using e_commerce_web_customer.Infrastructure.Products.Mock;
using e_commerce_web_customer.Infrastructure.Search.Db;
using e_commerce_web_customer.Infrastructure.Search.Mock;
using Microsoft.EntityFrameworkCore;

namespace e_commerce_web_customer.Infrastructure.DependencyInjection;

public static class StorefrontServiceCollectionExtensions
{
    public static IServiceCollection AddMockStorefrontServices(
        this IServiceCollection services)
    {
        services.AddSingleton<IProductCatalog, MockProductCatalog>();
        services.AddSingleton<ISiteCategoryMenuProvider, MockSiteCategoryMenuProvider>();
        services.AddSingleton<IProductDetailViewModelFactory, MockProductDetailViewModelFactory>();
        services.AddSingleton<ICategoryPageViewModelFactory, MockCategoryPageViewModelFactory>();
        services.AddSingleton<IHomePageViewModelFactory, MockHomePageViewModelFactory>();
        services.AddSingleton<ISearchSuggestionProvider, MockSearchSuggestionProvider>();
        services.AddSingleton<ISearchResultProvider, MockSearchResultProvider>();
        services.AddSingleton<IHeaderAccountProvider, MockHeaderAccountProvider>();
        services.AddSingleton<ICartDemoDataProvider, MockCartDemoDataProvider>();
        services.AddSingleton<IOrderService, MockOrderService>();
        services.AddScoped<IAccountService, MockAccountService>();
        services.AddScoped<ICartItemValidator, MockCartItemValidator>();

        return services;
    }

    public static IServiceCollection AddDatabaseStorefrontServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is required when UseMockData is false.");
        }

        services.AddDbContext<EcommerceDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddScoped<IProductCatalog, DbProductCatalog>();
        services.AddScoped<ISiteCategoryMenuDataService, DbSiteCategoryMenuDataService>();
        services.AddScoped<IHomePageDataService, DbHomePageDataService>();
        services.AddScoped<ICategoryPageDataService, DbCategoryPageDataService>();
        services.AddScoped<IProductDetailDataService, DbProductDetailDataService>();
        services.AddScoped<ISearchSuggestionDataService, DbSearchSuggestionDataService>();
        services.AddScoped<ISearchResultDataService, DbSearchResultDataService>();
        services.AddScoped<IHeaderAccountDataService, DbHeaderAccountDataService>();
        services.AddScoped<ISiteCategoryMenuProvider, DbSiteCategoryMenuProvider>();
        services.AddScoped<IProductDetailViewModelFactory, DbProductDetailViewModelFactory>();
        services.AddScoped<ICategoryPageViewModelFactory, DbCategoryPageViewModelFactory>();
        services.AddScoped<IHomePageViewModelFactory, DbHomePageViewModelFactory>();
        services.AddScoped<ISearchSuggestionProvider, DbSearchSuggestionProvider>();
        services.AddScoped<ISearchResultProvider, DbSearchResultProvider>();
        services.AddScoped<IHeaderAccountProvider, DbHeaderAccountProvider>();
        services.AddScoped<ICartDemoDataProvider, EmptyCartDemoDataProvider>();
        services.AddScoped<IOrderService, DbOrderService>();
        services.AddScoped<IAccountService, DbAccountService>();
        services.AddScoped<ICartItemValidator, DbCartItemValidator>();

        return services;
    }
}
