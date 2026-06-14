using e_commerce_web_customer.ViewModels.Home;

namespace e_commerce_web_customer.Infrastructure.Home.Mock;

internal static class HomeCategoryBannerFactory
{
    private const string ImageRoot = "/images/banners";

    public static CategoryProductBannerViewModel Create(
        string imageName,
        string imageAlt,
        string url)
    {
        return new CategoryProductBannerViewModel
        {
            ImageUrl = $"{ImageRoot}/{imageName}",
            ImageAlt = imageAlt,
            Url = url
        };
    }
}
