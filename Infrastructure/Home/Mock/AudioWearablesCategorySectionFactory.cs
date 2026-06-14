using e_commerce_web_customer.ViewModels.Home;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Home.Mock;

internal static class AudioWearablesCategorySectionFactory
{
    private const string ImageRoot = "/images/products/audio-wearables";

    public static CategoryProductsViewModel Create()
    {
        return new CategoryProductsViewModel
        {
            Id = "audio-wearable-products",
            Rows = 1,
            EnableTabSwitching = true,
            ShowPagination = false,
            Tabs =
            [
                Tab("watches", "Đồng hồ", "/catalog?cat=smartwatch", CreateWatchPanel(), true),
                Tab("audio", "Âm thanh", "/catalog?cat=audio", CreateAudioPanel())
            ]
        };
    }

    private static CategoryProductPanelViewModel CreateWatchPanel()
    {
        return Panel(
            "/catalog?cat=smartwatch",
            [
                QuickLink("Đồng hồ thể thao", "smartwatch", "usage=sport", "watch-01.webp"),
                QuickLink("Đồng hồ thông minh", "smartwatch", "type=smart", "watch-02.webp"),
                QuickLink("Theo dõi sức khỏe", "smartwatch", "feature=health", "watch-03.webp"),
                QuickLink("Đồng hồ trẻ em", "smartwatch", "usage=kids", "watch-04.webp"),
                QuickLink("Dây đeo thời trang", "smartwatch", "style=fashion", "watch-05.webp")
            ],
            Brands("smartwatch", "Apple", "Samsung", "Huawei", "Xiaomi", "Garmin", "Amazfit"),
            HomeProductCardFactory.AddVariants(
            [
                Product("smartwatch-sport-pro", "Đồng hồ thông minh Sport Pro GPS 46mm", "watch-01.webp",
                    "5.990.000đ", "6.990.000đ", "Giảm 14%", "Smember giảm đến 120.000đ",
                    "Theo dõi luyện tập, giấc ngủ và nhịp tim liên tục."),
                Product("smartwatch-series-silver", "Đồng hồ thông minh Series Silver LTE 44mm", "watch-02.webp",
                    "8.490.000đ", "9.990.000đ", "Giảm 15%", "Smember giảm đến 170.000đ",
                    "Kết nối LTE độc lập, nghe gọi ngay trên đồng hồ."),
                Product("smartwatch-classic-moon", "Đồng hồ thông minh Classic Moon 47mm", "watch-03.webp",
                    "7.290.000đ", "8.590.000đ", "Giảm 15%", "Smember giảm đến 146.000đ",
                    "Thiết kế cổ điển, đo sức khỏe chuyên sâu."),
                Product("smartwatch-fit-square", "Đồng hồ thông minh Fit Square AMOLED", "watch-04.webp",
                    "2.490.000đ", "2.990.000đ", "Giảm 17%", "Smember giảm đến 50.000đ",
                    "Màn hình AMOLED sáng rõ, pin dùng nhiều ngày."),
                Product("smartwatch-rose-active", "Đồng hồ thông minh Rose Active 42mm", "watch-05.webp",
                    "4.590.000đ", "5.490.000đ", "Giảm 16%", "Smember giảm đến 92.000đ",
                    "Phong cách thanh lịch, hỗ trợ hơn 100 chế độ tập.")
            ],
            [
                new(0, "smartwatch-sport-pro-titanium", "Đồng hồ Sport Pro GPS bản Titanium"),
                new(1, "smartwatch-series-silver-46mm", "Đồng hồ Series Silver LTE 46mm"),
                new(2, "smartwatch-classic-moon-leather", "Đồng hồ Classic Moon dây da"),
                new(3, "smartwatch-fit-square-lte", "Đồng hồ Fit Square AMOLED LTE"),
                new(4, "smartwatch-rose-active-lte", "Đồng hồ Rose Active LTE 42mm")
            ]));
    }

    private static CategoryProductPanelViewModel CreateAudioPanel()
    {
        return Panel(
            "/catalog?cat=audio",
            [
                QuickLink("Tai nghe", "audio", "type=headphones", "audio-01.webp"),
                QuickLink("Loa", "audio", "type=speaker", "audio-02.webp"),
                QuickLink("Mic thu âm", "audio", "type=recording-mic", "audio-03.webp"),
                QuickLink("Mic Karaoke", "audio", "type=karaoke-mic", "audio-04.webp"),
                QuickLink("Loa Karaoke", "audio", "type=karaoke-speaker", "audio-05.webp")
            ],
            Brands("audio", "Tai nghe Bluetooth", "Tai nghe có dây", "Tai nghe gaming", "Tai nghe thể thao", "Loa Bluetooth", "Loa Karaoke"),
            HomeProductCardFactory.AddVariants(
            [
                Product("headphone-wireless-anc", "Tai nghe không dây chống ồn chủ động ANC", "audio-01.webp",
                    "7.490.000đ", "7.990.000đ", "Giảm 6%", "Smember giảm đến 75.000đ",
                    "Giảm ngay khi mua kèm laptop hoặc máy tính bảng."),
                Product("earbuds-galaxy-buds-4-pro", "Tai nghe Samsung Galaxy Buds 4 Pro", "audio-06.webp",
                    "6.790.000đ", "7.590.000đ", "Giảm 11%", "Smember giảm đến 68.000đ",
                    "Tặng sạc nhanh và gói bảo hành rơi vỡ."),
                Product("earbuds-silver-ai", "Tai nghe không dây Silver AI Buds Pro", "audio-07.webp",
                    "4.590.000đ", "4.990.000đ", "Giảm 8%", "Smember giảm đến 46.000đ",
                    "Giảm giá cho khách hàng thành viên."),
                Product("earbuds-sport-violet", "Tai nghe không dây thể thao Sport Clip Pro", "audio-08.webp",
                    "4.590.000đ", "5.790.000đ", "Giảm 21%", "Smember giảm đến 46.000đ",
                    "Thiết kế bám tai chắc chắn, kháng nước khi tập luyện."),
                Product("earbuds-white-pro", "Tai nghe Bluetooth Pro 4 | Chính hãng", "audio-09.webp",
                    "2.990.000đ", "3.790.000đ", "Giảm 21%", "Smember giảm đến 60.000đ",
                    "Trả góp 0%, không phụ phí, trả trước từ 0đ.")
            ],
            [
                new(0, "headphone-wireless-anc-max", "Tai nghe không dây ANC Max"),
                new(1, "earbuds-galaxy-buds-4-pro-white", "Tai nghe Galaxy Buds 4 Pro màu trắng"),
                new(2, "earbuds-silver-ai-plus", "Tai nghe Silver AI Buds Pro Plus"),
                new(3, "earbuds-sport-violet-2026", "Tai nghe Sport Clip Pro 2026"),
                new(4, "earbuds-white-pro-usbc", "Tai nghe Bluetooth Pro 4 USB-C")
            ]));
    }

    private static CategoryTabViewModel Tab(
        string id,
        string label,
        string url,
        CategoryProductPanelViewModel panel,
        bool isActive = false)
    {
        return new CategoryTabViewModel
        {
            Id = id,
            Label = label,
            Url = url,
            IsActive = isActive,
            Panel = panel
        };
    }

    private static CategoryProductPanelViewModel Panel(
        string viewAllUrl,
        IReadOnlyList<CategoryQuickLinkViewModel> quickLinks,
        IReadOnlyList<CategoryBrandViewModel> brands,
        IReadOnlyList<ProductCardViewModel> products)
    {
        return new CategoryProductPanelViewModel
        {
            ViewAllUrl = viewAllUrl,
            Banners = GetBanners(viewAllUrl),
            QuickLinks = quickLinks,
            Brands = brands,
            Products = products
        };
    }

    private static IReadOnlyList<CategoryProductBannerViewModel> GetBanners(string viewAllUrl)
    {
        return viewAllUrl switch
        {
            "/catalog?cat=smartwatch" =>
            [
                HomeCategoryBannerFactory.Create(
                    "banner_apple_watch.png",
                    "Ưu đãi Apple Watch nổi bật",
                    "/catalog?cat=smartwatch&brand=apple")
            ],
            "/catalog?cat=audio" =>
            [
                HomeCategoryBannerFactory.Create(
                    "banner_airpod.png",
                    "Ưu đãi AirPods nổi bật",
                    "/catalog?cat=audio&brand=apple")
            ],
            _ => []
        };
    }

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
            ImageUrl = $"{ImageRoot}/{imageName}"
        };
    }

    private static IReadOnlyList<CategoryBrandViewModel> Brands(
        string category,
        params string[] labels)
    {
        return labels
            .Select(label => new CategoryBrandViewModel
            {
                Label = label,
                Url = $"/catalog?cat={category}&brand={Uri.EscapeDataString(label.ToLowerInvariant())}"
            })
            .ToArray();
    }

    private static ProductCardViewModel Product(
        string id,
        string name,
        string imageName,
        string price,
        string oldPrice,
        string discount,
        string memberOffer,
        string note)
    {
        return HomeProductCardFactory.Create(
            id,
            name,
            $"{ImageRoot}/{imageName}",
            price,
            oldPrice,
            discount,
            memberOffer,
            note);
    }
}
