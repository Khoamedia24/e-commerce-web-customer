using e_commerce_web_customer.ViewModels.Home;

namespace e_commerce_web_customer.Infrastructure.Home.Content;

internal static class HomePhoneTabletSectionContent
{
    private const string PhoneImageRoot = "/images/products/phone";

    public static IReadOnlyList<CategoryProductBannerViewModel> CreatePhoneBanners() =>
    [
        HomeCategoryBannerFactory.Create(
            "banner_iphone.png",
            "Ưu đãi iPhone nổi bật",
            "/catalog?cat=phone&brand=apple"),
        HomeCategoryBannerFactory.Create(
            "banner_samsung_phone.png",
            "Ưu đãi điện thoại Samsung",
            "/catalog?cat=phone&brand=samsung")
    ];

    public static IReadOnlyList<CategoryProductBannerViewModel> CreateTabletBanners() =>
    [
        HomeCategoryBannerFactory.Create(
            "banner_ipad.png",
            "Ưu đãi iPad nổi bật",
            "/catalog?cat=tablet&brand=apple"),
        HomeCategoryBannerFactory.Create(
            "banner_galaxy_tab.png",
            "Ưu đãi Samsung Galaxy Tab",
            "/catalog?cat=tablet&brand=samsung")
    ];

    public static IReadOnlyList<CategoryQuickLinkViewModel> CreatePhoneQuickLinks() =>
    [
        QuickLink("Điện thoại chơi game", "phone", "usage=gaming", "phone-gaming.webp"),
        QuickLink("Điện thoại pin trâu", "phone", "usage=battery", "phone-orange.webp"),
        QuickLink("Điện thoại 5G", "phone", "network=5g", "phone-violet.webp"),
        QuickLink("Điện thoại chụp ảnh đẹp", "phone", "usage=camera", "phone-camera.webp"),
        QuickLink("Điện thoại gập", "phone", "design=fold", "phone-rose.webp"),
        QuickLink("Điện thoại cao cấp", "phone", "tier=flagship", "phone-orange.webp")
    ];

    public static IReadOnlyList<CategoryQuickLinkViewModel> CreateTabletQuickLinks() =>
    [
        QuickLink("iPad cho học tập", "tablet", "usage=study", "phone-orange.webp"),
        QuickLink("Máy tính bảng Android", "tablet", "os=android", "phone-violet.webp"),
        QuickLink("Máy tính bảng 5G", "tablet", "network=5g", "phone-camera.webp"),
        QuickLink("Máy tính bảng có bút", "tablet", "feature=stylus", "phone-rose.webp"),
        QuickLink("Màn hình lớn", "tablet", "feature=large-display", "phone-gaming.webp"),
        QuickLink("Máy tính bảng cao cấp", "tablet", "tier=flagship", "phone-orange.webp")
    ];

    private static CategoryQuickLinkViewModel QuickLink(
        string label,
        string category,
        string query,
        string imageName)
    {
        return new CategoryQuickLinkViewModel
        {
            Label = label,
            Url = $"/catalog?cat={category}&{query}",
            ImageUrl = $"{PhoneImageRoot}/{imageName}"
        };
    }
}
