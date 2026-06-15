using e_commerce_web_customer.ViewModels.Home;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Home.Content;

internal static class HomeHeroViewModelFactory
{
    public static HomeHeroViewModel Create(
        IReadOnlyList<SiteCategoryMenuItemViewModel> categories)
    {
        return new HomeHeroViewModel
        {
            Categories = categories,
            CampaignTabs = CreateCampaignTabs(),
            Slides = CreateSlides(),
            PromoTiles = CreatePromoTiles(),
            BenefitGroups = CreateBenefitGroups()
        };
    }

    private static IReadOnlyList<HomeHeroCampaignTabViewModel> CreateCampaignTabs() =>
    [
        new() { Url = "/catalog?brand=samsung", Title = "Galaxy S25 Ultra", Subtitle = "Siêu đỉnh AI", Icon = "phone", IsActive = true },
        new() { Url = "/product/iphone-16-pro-max", Title = "iPhone 16 Pro Max", Subtitle = "Nâng tầm Pro", Icon = "phone" },
        new() { Url = "/catalog?cat=laptop&tag=student", Title = "Laptop sinh viên", Subtitle = "Ưu đãi học tập", Icon = "laptop" },
        new() { Url = "/trade-in", Title = "Thu cũ đổi mới", Subtitle = "Trợ giá đến 3 triệu", Icon = "swap" }
    ];

    private static IReadOnlyList<HomeHeroSlideViewModel> CreateSlides() =>
    [
        new()
        {
            Theme = "flagship",
            BrandLabel = "TechStore",
            Kicker = "Công nghệ đỉnh cao",
            Title = "Bứt phá mọi giới hạn",
            Subtitle = "Đặt trước smartphone cao cấp, chính hãng VN/A.",
            PriceLabel = "Giá chỉ từ",
            Price = "29.990.000đ",
            ActionLabel = "Mua ngay",
            ActionUrl = "/product/iphone-16-pro-max",
            ImageUrl = "/images/home/hero-smartphones.webp",
            ImageAlt = "Bốn mẫu điện thoại cao cấp nhiều màu",
            IsPriority = true,
            Services =
            [
                new() { Icon = "shield", Label = "Bảo hành", StrongText = "12 tháng" },
                new() { Icon = "box", Label = "1 đổi 1", StrongText = "30 ngày" },
                new() { Icon = "percent", Label = "Trả góp", StrongText = "0% lãi suất" },
                new() { Icon = "truck", Label = "Giao nhanh", StrongText = "miễn phí" }
            ]
        },
        new()
        {
            Theme = "gaming",
            BrandLabel = "Flash sale",
            Kicker = "Hiệu năng cho mọi cuộc chơi",
            Title = "Laptop gaming giảm đến 30%",
            Subtitle = "ROG, Legion, Nitro và MSI chính hãng, giao nhanh toàn quốc.",
            PriceLabel = "Giá chỉ từ",
            Price = "15.990.000đ",
            ActionLabel = "Khám phá ngay",
            ActionUrl = "/catalog?cat=laptop&tag=sale",
            FeatureIcon = "laptop"
        },
        new()
        {
            Theme = "trade-in",
            BrandLabel = "Thu cũ đổi mới",
            Kicker = "Nâng cấp nhẹ nhàng",
            Title = "Định giá thiết bị trong 2 phút",
            Subtitle = "Thu mua giá tốt, trợ giá trực tiếp và hỗ trợ trả góp 0%.",
            PriceLabel = "Trợ giá đến",
            Price = "4.000.000đ",
            ActionLabel = "Định giá ngay",
            ActionUrl = "/trade-in",
            FeatureIcon = "swap"
        }
    ];

    private static IReadOnlyList<HomeHeroPromoTileViewModel> CreatePromoTiles() =>
    [
        new()
        {
            Url = "/catalog?cat=laptop&brand=apple",
            Eyebrow = "Mỏng nhẹ. Mạnh mẽ.",
            Title = "MacBook Air M3",
            Price = "Từ 25.490.000đ",
            Tone = "mint",
            ImageUrl = "/images/hero-banner.png",
            ImageAlt = "MacBook Air M3 và thiết bị công nghệ"
        },
        new()
        {
            Url = "/catalog?brand=samsung",
            Eyebrow = "Galaxy AI thông minh",
            Title = "Galaxy S25 | S25+",
            Price = "Giảm đến 4 triệu",
            Tone = "lilac",
            ImageUrl = "/images/home/hero-smartphones.webp",
            ImageAlt = "Bộ sưu tập điện thoại Galaxy"
        },
        new()
        {
            Url = "/catalog?cat=laptop&tag=student",
            Eyebrow = "Ưu đãi sinh viên",
            Title = "Laptop học tập",
            Price = "Giảm đến 3 triệu",
            Tone = "rose",
            FallbackIcon = "laptop"
        }
    ];

    private static IReadOnlyList<HomeHeroBenefitGroupViewModel> CreateBenefitGroups() =>
    [
        new()
        {
            Title = "Ưu đãi cho giáo dục",
            Items =
            [
                new() { Url = "/deals/education", Label = "Đăng ký nhận", StrongText = "ưu đãi", Icon = "graduation" },
                new() { Url = "/deals/student", Label = "Deal hot", StrongText = "học sinh sinh viên", Icon = "discount" },
                new() { Url = "/catalog?cat=laptop&tag=student", Label = "Laptop", StrongText = "ưu đãi khủng", Icon = "laptop" }
            ]
        },
        new()
        {
            Title = "Thu cũ lên đời giá hời",
            Items =
            [
                new() { Url = "/trade-in?brand=apple", Label = "iPhone trợ giá đến", StrongText = "3 triệu", Icon = "phone" },
                new() { Url = "/trade-in?brand=samsung", Label = "Samsung trợ giá đến", StrongText = "4 triệu", Icon = "phone" }
            ]
        },
        new()
        {
            Title = "Khách hàng doanh nghiệp",
            Items =
            [
                new() { Url = "/business/register", Label = "Đăng ký", StrongText = "S-Business", Icon = "briefcase" },
                new() { Url = "/business/offers", Label = "Chính sách", StrongText = "ưu đãi", Icon = "news" }
            ]
        }
    ];
}
