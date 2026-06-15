using e_commerce_web_customer.ViewModels.Home;

namespace e_commerce_web_customer.Infrastructure.Home.Content;

internal static class HomeAdditionalCategorySectionContent
{
    public static IReadOnlyList<CategoryProductBannerViewModel> CreateBanners(
        string categorySlug)
    {
        return categorySlug switch
        {
            "laptop" =>
            [
                Banner("banner_laptop.png", "Ưu đãi laptop nổi bật", "laptop"),
                Banner("banner_mac.png", "Ưu đãi MacBook dành cho sinh viên", "laptop", "brand=apple")
            ],
            "pc" =>
            [
                Banner("banner_pc_acer.png", "Ưu đãi máy tính Acer", "pc", "brand=acer"),
                Banner("banner_pc_asus.png", "Ưu đãi máy tính ASUS", "pc", "brand=asus")
            ],
            "man-hinh" =>
            [
                Banner("banner_asus_monitor.png", "Ưu đãi màn hình ASUS", "man-hinh", "brand=asus"),
                Banner("banner_acer_monitor.png", "Ưu đãi màn hình Acer", "man-hinh", "brand=acer")
            ],
            "may-in" =>
            [
                Banner("banner_amd.png", "Linh kiện máy tính AMD nổi bật", "may-in", "brand=amd"),
                Banner("banner_nvidia.png", "Linh kiện máy tính NVIDIA nổi bật", "may-in", "brand=nvidia")
            ],
            "dong-ho" =>
            [
                Banner("banner_apple_watch.png", "Ưu đãi Apple Watch nổi bật", "dong-ho", "brand=apple")
            ],
            "am-thanh" =>
            [
                Banner("banner_airpod.png", "Ưu đãi AirPods nổi bật", "am-thanh", "brand=apple")
            ],
            _ => []
        };
    }

    private static CategoryProductBannerViewModel Banner(
        string imageName,
        string imageAlt,
        string categorySlug,
        string? query = null)
    {
        var url = $"/catalog?cat={Uri.EscapeDataString(categorySlug)}";
        if (!string.IsNullOrWhiteSpace(query))
        {
            url += $"&{query}";
        }

        return HomeCategoryBannerFactory.Create(imageName, imageAlt, url);
    }
}
