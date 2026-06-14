using e_commerce_web_customer.Application.Catalog;
using e_commerce_web_customer.Infrastructure.Home.Mock;
using e_commerce_web_customer.ViewModels.Catalog;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Catalog.Mock;

public sealed class MockCategoryPageViewModelFactory : ICategoryPageViewModelFactory
{
    private const string PhoneImageRoot = "/images/products/phone";
    private const string AudioImageRoot = "/images/products/audio-wearables";

    public Task<CategoryPageViewModel?> CreateAsync(
        CategoryPageRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        CategoryPageViewModel? model = request.Slug.Trim().ToLowerInvariant() switch
        {
            "phone" or "mobile" => CreatePhonePage(request),
            "audio" or "am-thanh" or "thiet-bi-am-thanh" or "speaker" or "microphone" => CreateAudioPage(request),
            _ => CreateGenericPage(request)
        };

        return Task.FromResult<CategoryPageViewModel?>(model);
    }

    private static CategoryPageViewModel CreatePhonePage(CategoryPageRequest request)
    {
        var products = CreatePhoneProducts();

        return new CategoryPageViewModel
        {
            Slug = "phone",
            Title = "Điện thoại",
            MetaDescription = "Mua điện thoại chính hãng, giá tốt, trả góp 0%, giao nhanh tại TechStore.",
            LayoutMode = CategoryPageLayoutMode.FilterListing,
            Breadcrumbs =
            [
                new() { Label = "Trang chủ", Url = "/" },
                new() { Label = "Điện thoại", Url = "/catalog?cat=phone", IsCurrent = true }
            ],
            PromotionBanners =
            [
                new()
                {
                    Id = "iphone-17-pro",
                    Kicker = "Thu cũ lên đời",
                    Title = "iPhone 17 Pro Max",
                    PriceText = "Lên đời từ 32,95 triệu",
                    ImageUrl = $"{PhoneImageRoot}/phone-orange-cutout.png",
                    ImageAlt = "iPhone 17 Pro Max màu cam",
                    Url = "/product/iphone-17-pro-max-256gb",
                    Theme = "peach"
                },
                new()
                {
                    Id = "galaxy-ai",
                    Kicker = "Galaxy AI",
                    Title = "Samsung Galaxy S26 Ultra",
                    PriceText = "Ưu đãi đến 7 triệu",
                    ImageUrl = $"{PhoneImageRoot}/phone-violet-cutout.png",
                    ImageAlt = "Samsung Galaxy S26 Ultra màu tím",
                    Url = "/product/iphone-17-pro-max-256gb",
                    Theme = "violet"
                },
                new()
                {
                    Id = "gaming-phone",
                    Kicker = "Hiệu năng dẫn đầu",
                    Title = "Điện thoại gaming",
                    PriceText = "Giá chỉ từ 6,99 triệu",
                    ImageUrl = $"{PhoneImageRoot}/phone-gaming-cutout.png",
                    ImageAlt = "Điện thoại gaming màu đen",
                    Url = "/catalog?cat=phone&need=gaming",
                    Theme = "graphite"
                }
            ],
            Brands = CreateBrands(),
            QuickLinks =
            [
                QuickLink("Điện thoại chơi game", "need=gaming", "phone-gaming-cutout.png"),
                QuickLink("Điện thoại pin trâu", "need=battery", "phone-rose-cutout.png"),
                QuickLink("Điện thoại 5G", "network=5g", "phone-violet-cutout.png"),
                QuickLink("Điện thoại chụp ảnh đẹp", "need=camera", "phone-camera-cutout.png"),
                QuickLink("Điện thoại gập", "type=foldable", "phone-orange-cutout.png"),
                QuickLink("Điện thoại AI", "feature=ai", "phone-violet-cutout.png"),
                QuickLink("Điện thoại phổ thông", "type=feature-phone", "phone-gaming-cutout.png")
            ],
            HotSale = new CategoryHotSaleViewModel
            {
                Title = "Hot sale cuối tuần",
                EndsAt = DateTimeOffset.UtcNow.AddHours(25).AddMinutes(10),
                Products = products.Skip(5).Take(7).Select(ToHotSaleCard).ToList()
            },
            Filter = CreateFilter(request),
            Products = products,
            InitialProductCount = 20,
            SeoContent = new CategorySeoContentViewModel
            {
                Title = "Điện thoại chính hãng tại TechStore",
                Paragraphs =
                [
                    "TechStore cung cấp điện thoại chính hãng từ Apple, Samsung, OPPO, Xiaomi, HONOR và nhiều thương hiệu khác. Sản phẩm có thông tin giá, ưu đãi và tình trạng giao hàng rõ ràng.",
                    "Bạn có thể chọn điện thoại theo nhu cầu, cấu hình, dung lượng, camera hoặc mức giá. Bộ lọc luôn nằm dưới header khi cuộn để việc so sánh sản phẩm thuận tiện hơn.",
                    "Chính sách trả góp, thu cũ đổi mới và giao nhanh được cập nhật theo từng sản phẩm và khu vực."
                ]
            },
            QuestionAnswer = CreateQuestionAnswer()
        };
    }

    private static CategoryPageViewModel CreateAudioPage(CategoryPageRequest request)
    {
        var sections = CreateAudioSections(request);
        var products = sections.SelectMany(section => section.Products).ToList();

        return new CategoryPageViewModel
        {
            Slug = "audio",
            Title = "Thiết bị âm thanh",
            MetaDescription = "Mua tai nghe, loa và phụ kiện âm thanh chính hãng tại TechStore. Giao nhanh, trả góp 0%, nhiều ưu đãi theo từng nhóm sản phẩm.",
            LayoutMode = CategoryPageLayoutMode.Sectioned,
            Breadcrumbs =
            [
                new() { Label = "Trang chủ", Url = "/" },
                new() { Label = "Âm thanh", Url = "/catalog?cat=audio", IsCurrent = true }
            ],
            PromotionBanners =
            [
                new()
                {
                    Id = "audio-sony-sale",
                    Kicker = "Săn deal slay",
                    Title = "Nghe cực cháy",
                    PriceText = "Tai nghe từ 790K",
                    ImageUrl = $"{AudioImageRoot}/audio-01.webp",
                    ImageAlt = "Tai nghe chụp tai màu đen",
                    Url = "#section-headphones",
                    Theme = "graphite"
                },
                new()
                {
                    Id = "audio-jbl-speaker",
                    Kicker = "JBL dare to listen",
                    Title = "Loa Bluetooth",
                    PriceText = "Giá chỉ từ 1,09 triệu",
                    ImageUrl = $"{AudioImageRoot}/audio-02.webp",
                    ImageAlt = "Loa Bluetooth màu đen",
                    Url = "#section-speakers",
                    Theme = "peach"
                },
                new()
                {
                    Id = "audio-mic-live",
                    Kicker = "Livestream rõ tiếng",
                    Title = "Mic thu âm",
                    PriceText = "Ưu đãi đến 38%",
                    ImageUrl = $"{AudioImageRoot}/audio-03.webp",
                    ImageAlt = "Micro thu âm để bàn",
                    Url = "#section-audio-accessories",
                    Theme = "violet"
                }
            ],
            Brands = [],
            QuickLinks =
            [
                AudioQuickLink("Tai nghe", "type=headphones", "audio-01.webp"),
                AudioQuickLink("Loa", "type=speakers", "audio-02.webp"),
                AudioQuickLink("Mic thu âm", "type=recording-mic", "audio-03.webp"),
                AudioQuickLink("Mic Karaoke", "type=karaoke-mic", "audio-04.webp"),
                AudioQuickLink("Đầu đĩa than", "type=turntable", "audio-10.webp")
            ],
            HotSale = new CategoryHotSaleViewModel
            {
                Title = "Hot sale cuối tuần",
                EndsAt = DateTimeOffset.UtcNow.AddHours(25).AddMinutes(8),
                Products = products.Take(8).Select(ToHotSaleCard).ToList()
            },
            Filter = new CategoryFilterViewModel
            {
                Title = "Chọn theo tiêu chí",
                PrimaryItems = [],
                SecondaryItems = [],
                SortOptions = []
            },
            Products = products,
            InitialProductCount = 10,
            SectionTabs =
            [
                new() { Id = "headphones", Label = "Tai nghe", Url = "#section-headphones", IsActive = true },
                new() { Id = "speakers", Label = "Loa", Url = "#section-speakers" },
                new() { Id = "audio-accessories", Label = "Phụ kiện âm thanh", Url = "#section-audio-accessories" }
            ],
            ProductSections = sections,
            SeoContent = new CategorySeoContentViewModel
            {
                Title = "Thiết bị âm thanh chính hãng tại TechStore",
                Paragraphs =
                [
                    "Danh mục âm thanh được chia theo từng nhu cầu rõ ràng: tai nghe, loa và phụ kiện âm thanh. Mỗi nhóm hiển thị các sản phẩm nổi bật để người dùng so sánh nhanh mà không phải lọc lại từ đầu.",
                    "Các section dùng chung cấu trúc dữ liệu nên có thể lấy từ mock data hoặc database thật. Backend chỉ cần trả danh sách section, banner, danh mục con và sản phẩm theo cùng view model."
                ]
            },
            QuestionAnswer = CreateAudioQuestionAnswer()
        };
    }

    private static IReadOnlyList<CategoryBrandViewModel> CreateBrands()
    {
        string[] brands =
        [
            "iPhone", "Samsung", "OPPO", "Xiaomi", "Tecno", "HONOR", "Nubia",
            "Sony", "Nokia", "Infinix", "Nothing", "Masstel", "realme", "itel",
            "Huawei", "Meizu", "vivo", "OnePlus", "TCL", "benco", "ASUS"
        ];

        return brands.Select(label => new CategoryBrandViewModel
        {
            Id = Slugify(label),
            Label = label,
            Url = $"/catalog?cat=phone&brand={Uri.EscapeDataString(label.ToLowerInvariant())}"
        }).ToList();
    }

    private static CategoryFilterViewModel CreateFilter(CategoryPageRequest request)
    {
        var activeSort = string.IsNullOrWhiteSpace(request.Sort)
            ? "popular"
            : request.Sort.Trim().ToLowerInvariant();

        return new CategoryFilterViewModel
        {
            Title = "Chọn theo tiêu chí",
            PrimaryItems =
            [
                FilterItem("Bộ lọc", "filter", icon: "filter", emphasized: true),
                FilterItem("Sẵn hàng", "availability=in-stock", icon: "truck"),
                FilterItem("Hàng mới về", "tag=new", icon: "box"),
                FilterItem("Xem theo giá", "filter=price", icon: "price"),
                FilterItem("Nhu cầu sử dụng", "filter=need", dropdown: true),
                FilterItem("Chip xử lí", "filter=processor", dropdown: true),
                FilterItem("Loại điện thoại", "filter=type", dropdown: true),
                FilterItem("Dung lượng RAM", "filter=ram", dropdown: true)
            ],
            SecondaryItems =
            [
                FilterItem("Bộ nhớ trong", "filter=storage", dropdown: true),
                FilterItem("Tính năng đặc biệt", "filter=special", dropdown: true),
                FilterItem("Tính năng camera", "filter=camera", dropdown: true),
                FilterItem("Tần số quét", "filter=refresh-rate", dropdown: true),
                FilterItem("Kiểu màn hình", "filter=display", dropdown: true),
                FilterItem("Công nghệ NFC", "filter=nfc", dropdown: true),
                FilterItem("Hỗ trợ mạng", "filter=network", dropdown: true)
            ],
            SortOptions =
            [
                SortItem("Phổ biến", "popular", "star", activeSort),
                SortItem("Khuyến mãi HOT", "promotion", "discount", activeSort),
                SortItem("Giá Thấp - Cao", "price-asc", "sort-up", activeSort),
                SortItem("Giá Cao - Thấp", "price-desc", "sort-down", activeSort)
            ]
        };
    }

    internal static IReadOnlyList<ProductCardViewModel> CreatePhoneProducts()
    {
        var definitions = new[]
        {
            Product("iphone-17-pro-256gb", "iPhone 17 Pro 256GB | Chính hãng", "phone-orange-cutout.png", "33.790.000đ", "34.990.000đ", "Giảm 3%", ["6.3 inches", "256 GB"], "Smember giảm đến 338.000đ", "Trả góp 0% - 0đ phụ phí - 0đ trả trước - kỳ hạn đến 6 tháng", null, 5m),
            Product("iphone-17-pro-max-256gb", "iPhone 17 Pro Max 256GB | Chính hãng", "phone-orange.webp", "36.990.000đ", "37.990.000đ", "Giảm 3%", ["6.9 inches", "256 GB"], "Smember giảm đến 370.000đ", "Trả góp 0% - 0đ phụ phí - 0đ trả trước - kỳ hạn đến 6 tháng", null, 5m),
            Product("samsung-galaxy-s26-ultra", "Samsung Galaxy S26 Ultra 5G 12GB 256GB", "phone-violet-cutout.png", "29.990.000đ", "36.990.000đ", "Giảm 19%", ["6.9 inches", "12 GB", "256 GB"], "Smember giảm đến 300.000đ", "Không phí chuyển đổi khi trả góp 0% qua thẻ tín dụng kỳ hạn 3-6 tháng", "S-Student giảm thêm 500.000đ", 5m),
            Product("samsung-galaxy-s26", "Samsung Galaxy S26 5G 12GB 256GB", "phone-violet.webp", "21.490.000đ", "25.990.000đ", "Giảm 17%", ["6.3 inches", "12 GB", "256 GB"], "Smember giảm đến 215.000đ", "Không phí chuyển đổi khi trả góp 0% qua thẻ tín dụng kỳ hạn 3-6 tháng", "S-Student giảm thêm 500.000đ", 5m),
            Product("oppo-find-x9s", "OPPO Find X9s 12GB 256GB", "phone-camera-cutout.png", "23.790.000đ", "24.990.000đ", "Giảm 5%", ["6.59 inches", "12 GB", "256 GB"], "Smember giảm đến 238.000đ", "Chỉ thêm 700K - Nhận ngay dịch vụ Bảo hành VIP 12 tháng 1 đổi 1", "S-Student giảm thêm 300.000đ", 5m),
            Product("iphone-17-256gb", "iPhone 17 256GB | Chính hãng", "phone-rose-cutout.png", "23.990.000đ", "24.990.000đ", "Giảm 4%", ["6.3 inches", "256 GB"], "Smember giảm đến 240.000đ", "Trả góp 0% - 0đ phụ phí - 0đ trả trước - kỳ hạn đến 6 tháng", null, 5m),
            Product("samsung-galaxy-a17", "Samsung Galaxy A17 5G 8GB 128GB", "phone-violet-cutout.png", "5.990.000đ", "6.390.000đ", "Giảm 6%", ["6.7 inches", "8 GB", "128 GB"], "Smember giảm đến 60.000đ", "Không phí chuyển đổi khi trả góp 0% qua thẻ tín dụng kỳ hạn 3-6 tháng", "S-Student giảm thêm 299.500đ", 4.5m),
            Product("iphone-air-256gb", "iPhone Air 256GB | Chính hãng", "phone-camera-cutout.png", "22.890.000đ", "31.990.000đ", "Giảm 28%", ["6.5 inches", "256 GB"], "Smember giảm đến 229.000đ", "Trả góp 0% - 0đ phụ phí - 0đ trả trước - kỳ hạn đến 6 tháng", null, 4.9m),
            Product("nubia-neo-5", "Nubia Neo 5 5G 8GB 128GB", "phone-gaming-cutout.png", "6.990.000đ", "7.490.000đ", "Giảm 7%", ["6.8 inches", "8 GB", "128 GB"], "Smember giảm đến 70.000đ", "Trả góp 0% lãi suất, không trả trước, không phụ phí", "S-Student giảm thêm 300.000đ", 4.8m),
            Product("xiaomi-17t-pro", "Xiaomi 17T Pro 5G 12GB 512GB", "phone-camera.webp", "21.490.000đ", "24.990.000đ", "Giảm 14%", ["6.83 inches", "12 GB", "512 GB"], "Smember giảm đến 205.000đ", "Không phí chuyển đổi khi trả góp 0% qua thẻ tín dụng", "S-Student giảm thêm 300.000đ", 5m, "Hàng mới về"),
            Product("iphone-15-128gb", "iPhone 15 128GB | Chính hãng VN/A", "phone-rose.webp", "16.990.000đ", "18.990.000đ", "Giảm 10%", ["6.1 inches", "128 GB"], "Smember giảm đến 170.000đ", "Trả góp 0% - kỳ hạn đến 6 tháng", null, 4.9m),
            Product("oppo-find-n6", "OPPO Find N6 16GB 512GB", "phone-orange-cutout.png", "31.990.000đ", "32.690.000đ", "Giảm 2%", ["7.1 inches", "16 GB", "512 GB"], "Smember giảm đến 320.000đ", "Tặng gói bảo hành màn hình 12 tháng", null, 4.8m),
            Product("honor-600", "HONOR 600 5G 8GB 256GB", "phone-orange.webp", "11.490.000đ", "12.890.000đ", "Giảm 11%", ["6.7 inches", "8 GB", "256 GB"], "Smember giảm đến 115.000đ", "Giảm thêm khi thu cũ đổi mới", "S-Student giảm thêm 300.000đ", 5m),
            Product("xiaomi-17t", "Xiaomi 17T 5G 12GB 512GB", "phone-camera-cutout.png", "16.490.000đ", "19.990.000đ", "Giảm 17%", ["6.83 inches", "12 GB", "512 GB"], "Smember giảm đến 165.000đ", "Không phí chuyển đổi khi trả góp", "S-Student giảm thêm 300.000đ", 4.8m),
            Product("oppo-reno15", "OPPO Reno15 F 5G 12GB 256GB", "phone-violet.webp", "12.490.000đ", "12.790.000đ", "Giảm 2%", ["6.57 inches", "12 GB", "256 GB"], "Smember giảm đến 125.000đ", "Tặng gói data khi mua máy", null, 4.7m),
            Product("samsung-galaxy-a57", "Samsung Galaxy A57 5G 8GB 128GB", "phone-violet-cutout.png", "11.190.000đ", "12.490.000đ", "Giảm 10%", ["6.7 inches", "8 GB", "128 GB"], "Smember giảm đến 112.000đ", "Không phí chuyển đổi khi trả góp 0%", "S-Student giảm thêm 500.000đ", 4.9m),
            Product("samsung-galaxy-s25-ultra", "Samsung Galaxy S25 Ultra 12GB 256GB", "phone-violet.webp", "26.190.000đ", "33.380.000đ", "Giảm 22%", ["6.9 inches", "12 GB", "256 GB"], "Smember giảm đến 262.000đ", "Không phí chuyển đổi khi trả góp 0%", "S-Student giảm thêm 500.000đ", 4.7m),
            Product("xiaomi-redmi-note-14-pro-plus", "Xiaomi Redmi Note 14 Pro Plus 5G 8GB 256GB", "phone-orange-cutout.png", "7.790.000đ", "10.800.000đ", "Giảm 28%", ["6.67 inches", "8 GB", "256 GB"], "Smember giảm đến 78.000đ", "Không phí chuyển đổi khi trả góp 0%", "S-Student giảm thêm 300.000đ", 5m),
            Product("honor-x9d", "HONOR X9d 5G 12GB 256GB", "phone-camera.webp", "10.690.000đ", "11.990.000đ", "Giảm 11%", ["6.79 inches", "12 GB", "256 GB"], "Smember giảm đến 107.000đ", "Tặng bảo hành Honor Care Plus", "S-Student giảm thêm 500.000đ", 4.8m),
            Product("iphone-17e", "iPhone 17e 256GB | Chính hãng", "phone-rose-cutout.png", "17.390.000đ", "17.990.000đ", "Giảm 3%", ["6.1 inches", "256 GB"], "Smember giảm đến 174.000đ", "Trả góp 0% - kỳ hạn đến 6 tháng", null, 5m),
            Product("poco-x8-pro-max", "POCO X8 Pro Max 12GB 256GB", "phone-gaming-cutout.png", "14.290.000đ", "16.990.000đ", "Giảm 16%", ["6.8 inches", "12 GB", "256 GB"], "Smember giảm đến 143.000đ", "Tặng bộ quà gaming khi mua máy", null, 4.8m),
            Product("honor-400-pro", "HONOR 400 Pro 5G 12GB 512GB", "phone-camera-cutout.png", "14.990.000đ", "18.660.000đ", "Giảm 20%", ["6.7 inches", "12 GB", "512 GB"], "Smember giảm đến 150.000đ", "Giảm thêm khi thu cũ đổi mới", null, 5m),
            Product("samsung-galaxy-a37", "Samsung Galaxy A37 5G 8GB 128GB", "phone-violet-cutout.png", "9.490.000đ", "10.790.000đ", "Giảm 12%", ["6.7 inches", "8 GB", "128 GB"], "Smember giảm đến 95.000đ", "Không phí chuyển đổi khi trả góp 0%", null, 4.9m),
            Product("realme-gt-neo", "realme GT Neo 12GB 256GB", "phone-gaming.webp", "12.990.000đ", "14.990.000đ", "Giảm 13%", ["6.78 inches", "12 GB", "256 GB"], "Smember giảm đến 130.000đ", "Tặng tai nghe khi mua máy", null, 4.7m),
            Product("vivo-v50", "vivo V50 5G 12GB 256GB", "phone-camera.webp", "11.990.000đ", "13.490.000đ", "Giảm 11%", ["6.77 inches", "12 GB", "256 GB"], "Smember giảm đến 120.000đ", "Giảm thêm khi thanh toán online", null, 4.8m)
        };

        return definitions;
    }

    internal static IReadOnlyList<CategoryProductSectionViewModel> CreateAudioSections(CategoryPageRequest request)
    {
        var sortOptions = CreateAudioSortOptions(request);

        return
        [
            new()
            {
                Id = "section-headphones",
                Title = "Tai nghe",
                Description = "Tai nghe Bluetooth, có dây, chụp tai và tai nghe thể thao được chọn theo mức giá dễ so sánh.",
                ViewAllUrl = "/catalog?cat=audio&type=headphones",
                Banner = new CategorySectionBannerViewModel
                {
                    Title = "Tai nghe",
                    Subtitle = "Bảo hành chính hãng",
                    ImageUrl = $"{AudioImageRoot}/audio-01.webp",
                    ImageAlt = "Tai nghe chụp tai",
                    Theme = "rose"
                },
                Subcategories =
                [
                    AudioPill("Bluetooth", "type=bluetooth-headphones", "audio-06.webp", true),
                    AudioPill("Có dây", "type=wired-headphones", "audio-07.webp"),
                    AudioPill("Chụp tai", "type=over-ear", "audio-01.webp"),
                    AudioPill("Gaming", "type=gaming-headphones", "audio-08.webp"),
                    AudioPill("Thể thao", "type=sport-headphones", "audio-09.webp")
                ],
                SortOptions = sortOptions,
                Products =
                [
                    AudioProduct("airpods-4", "Tai nghe Bluetooth Apple AirPods 4 | Chính hãng Apple", "audio-06.webp", "2.990.000đ", "3.790.000đ", "Giảm 21%", ["Tai nghe 5h", "Hộp sạc 30h", "Chip H2"], "Smember giảm đến 60.000đ", "Trả góp 0% - 0đ phụ phí - 0đ trả trước - kỳ hạn đến 9 tháng", null, 5m),
                    AudioProduct("sony-wh-1000xm6", "Tai nghe Bluetooth chụp tai Sony WH-1000XM6", "audio-01.webp", "9.390.000đ", "11.990.000đ", "Giảm 22%", ["Tai nghe 40h", "Bluetooth 5.3", "Chống ồn ANC"], "Smember giảm đến 94.000đ", "Giảm ngay 15% Tai nghe SONY khi mua kèm Điện thoại hoặc Laptop", "S-Student giảm thêm 300.000đ", 5m),
                    AudioProduct("galaxy-buds-4", "Tai nghe Samsung Galaxy Buds 4", "audio-07.webp", "4.090.000đ", "4.990.000đ", "Giảm 18%", ["Tai nghe 6 giờ", "Bluetooth 6.1", "Trợ lý AI"], "Smember giảm đến 41.000đ", "Giảm thêm 500K khi mua kèm Samsung S26, S25, S24 Ultra", null, 5m),
                    AudioProduct("bose-qc-earbuds", "Tai nghe bluetooth Bose Quietcomfort Earbuds", "audio-08.webp", "3.950.000đ", "4.400.000đ", "Giảm 10%", ["8.5h", "Bluetooth 5.3", "IPX4"], "Smember giảm đến 40.000đ", null, null, 4.8m),
                    AudioProduct("jbl-live-pro-2", "Tai nghe Bluetooth True Wireless JBL Live Pro 2", "audio-09.webp", "2.200.000đ", "3.990.000đ", "Giảm 45%", ["Tai nghe 10h", "Hộp sạc 30h", "IPX5"], "Smember giảm đến 22.000đ", null, "S-Student giảm thêm 110.000đ", 4.7m),
                    AudioProduct("oppo-enco-buds3", "Tai nghe Bluetooth True Wireless OPPO Enco Buds3", "audio-06.webp", "870.000đ", "970.000đ", "Giảm 10%", ["Tai nghe 12h", "Hộp sạc 42h", "IP55"], "Smember giảm đến 9.000đ", null, null, 4.8m),
                    AudioProduct("soundpeats-t3-pro", "Tai nghe Bluetooth True Wireless SoundPEATS T3 Pro", "audio-07.webp", "290.000đ", "500.000đ", "Giảm 42%", ["Tai nghe 7h", "Hộp sạc 28h", "Bluetooth 5.4"], "Smember giảm đến 10.000đ", "Không phí chuyển đổi khi trả góp qua thẻ tín dụng", null, 5m),
                    AudioProduct("havit-h630bt-pro", "Tai nghe Bluetooth chụp tai Havit H630BT Pro", "audio-01.webp", "350.000đ", "430.000đ", "Giảm 19%", ["Driver 40mm", "Bluetooth 5.3", "ANC"], "Smember giảm đến 5.000đ", null, null, 4.6m, "Hàng mới về"),
                    AudioProduct("baseus-bowie-mp1", "Tai nghe Bluetooth True Wireless Baseus Bowie MP1", "audio-08.webp", "980.000đ", "1.390.000đ", "Giảm 29%", ["Driver 11mm", "Bluetooth 6.0", "IP55"], "Smember giảm đến 20.000đ", null, null, 4.9m),
                    AudioProduct("anker-r50i", "Tai nghe Bluetooth Soundcore R50i NC", "audio-09.webp", "690.000đ", "990.000đ", "Giảm 30%", ["ANC", "10h", "Bluetooth 5.4"], "Smember giảm đến 8.000đ", null, null, 4.8m),
                    AudioProduct("marshall-major-v", "Tai nghe chụp tai Marshall Major V", "audio-01.webp", "3.290.000đ", "3.990.000đ", "Giảm 18%", ["100h", "Bluetooth LE", "Sạc nhanh"], "Smember giảm đến 33.000đ", null, null, 4.9m),
                    AudioProduct("xiaomi-buds-6", "Tai nghe Xiaomi Buds 6 Pro", "audio-06.webp", "1.990.000đ", "2.490.000đ", "Giảm 20%", ["Hi-Res", "ANC", "IP54"], "Smember giảm đến 20.000đ", null, "S-Student giảm thêm 100.000đ", 4.8m)
                ]
            },
            new()
            {
                Id = "section-speakers",
                Title = "Loa",
                Description = "Loa Bluetooth, loa soundbar và loa karaoke cho phòng khách, bàn làm việc hoặc chuyến đi ngắn.",
                ViewAllUrl = "/catalog?cat=audio&type=speakers",
                Banner = new CategorySectionBannerViewModel
                {
                    Title = "Loa Bluetooth",
                    Subtitle = "Bảo hành chính hãng",
                    ImageUrl = $"{AudioImageRoot}/audio-02.webp",
                    ImageAlt = "Loa Bluetooth",
                    Theme = "orange"
                },
                Subcategories =
                [
                    AudioPill("Loa Bluetooth", "type=bluetooth-speaker", "audio-02.webp", true),
                    AudioPill("Loa Soundbar", "type=soundbar", "audio-10.webp"),
                    AudioPill("Loa Karaoke", "type=karaoke-speaker", "audio-05.webp"),
                    AudioPill("Loa vi tính", "type=computer-speaker", "audio-02.webp")
                ],
                SortOptions = sortOptions,
                Products =
                [
                    AudioProduct("jbl-flip-6", "Loa Bluetooth JBL Flip 6", "audio-02.webp", "2.270.000đ", "2.935.000đ", "Giảm 23%", ["30W", "12 giờ", "IP67"], "Smember giảm đến 23.000đ", null, "S-Student giảm thêm 113.500đ", 5m),
                    AudioProduct("anker-boom-go-3i", "Loa Bluetooth Anker Soundcore Boom Go 3i", "audio-05.webp", "1.090.000đ", "1.990.000đ", "Giảm 45%", ["15W", "Bluetooth 6.0", "Đèn RGB"], "Smember giảm đến 11.000đ", null, null, 4.8m),
                    AudioProduct("harman-onyx-studio-9", "Loa Bluetooth Harman Kardon Onyx Studio 9", "audio-10.webp", "6.490.000đ", "7.990.000đ", "Giảm 19%", ["50W", "8h", "Bluetooth 5.3"], "Smember giảm đến 65.000đ", "Đăng nhập ngay để nhận ưu đãi giá và chương trình khuyến mãi", "S-Student giảm thêm 300.000đ", 5m),
                    AudioProduct("sony-srs-xv500", "Loa Karaoke Sony SRS-XV500", "audio-05.webp", "7.490.000đ", "9.990.000đ", "Giảm 25%", ["160W", "25h", "Mega Bass"], "Smember giảm đến 75.000đ", "Giảm ngay 15% Loa SONY khi mua kèm Điện thoại hoặc Laptop", "S-Student giảm thêm 300.000đ", 4.9m),
                    AudioProduct("jbl-charge-6", "Loa Bluetooth JBL Charge 6", "audio-02.webp", "4.790.000đ", "5.990.000đ", "Giảm 20%", ["45W", "28h", "Bluetooth 5.4"], "Smember giảm đến 44.000đ", "Đăng nhập ngay để nhận ưu đãi giá và chương trình khuyến mãi", "S-Student giảm thêm 219.500đ", 5m),
                    AudioProduct("marshall-emberton-iii", "Loa Bluetooth Marshall Emberton III", "audio-10.webp", "4.190.000đ", "4.990.000đ", "Giảm 16%", ["32h", "IP67", "Stereo"], "Smember giảm đến 42.000đ", null, null, 4.8m),
                    AudioProduct("tribit-stormbox-mini", "Loa Bluetooth Tribit StormBox Mini Portable Speaker", "audio-02.webp", "535.000đ", "790.000đ", "Giảm 32%", ["12 giờ", "IPX7", "Không nước"], "Smember giảm đến 8.000đ", null, null, 5m),
                    AudioProduct("stargo-p20", "Loa Bluetooth StarGO P20", "audio-05.webp", "845.000đ", "1.490.000đ", "Giảm 43%", ["30W", "12 giờ", "Đèn RGB"], "Smember giảm đến 10.000đ", null, null, 4.9m),
                    AudioProduct("lg-xboom-go", "Loa Bluetooth LG XBOOM Go XG2", "audio-02.webp", "1.690.000đ", "2.390.000đ", "Giảm 29%", ["IP67", "18h", "Bass Boost"], "Smember giảm đến 17.000đ", null, null, 4.7m),
                    AudioProduct("bose-soundlink-flex", "Loa Bluetooth Bose SoundLink Flex II", "audio-10.webp", "3.490.000đ", "4.190.000đ", "Giảm 17%", ["12h", "IP67", "USB-C"], "Smember giảm đến 35.000đ", null, null, 4.9m),
                    AudioProduct("xiaomi-sound-pocket", "Loa Bluetooth Xiaomi Sound Pocket", "audio-02.webp", "490.000đ", "690.000đ", "Giảm 29%", ["5W", "10h", "IP67"], "Smember giảm đến 6.000đ", null, null, 4.6m),
                    AudioProduct("edifier-mp230", "Loa Bluetooth Edifier MP230 Classic", "audio-10.webp", "1.490.000đ", "1.990.000đ", "Giảm 25%", ["Bluetooth 5.0", "16h", "AUX"], "Smember giảm đến 15.000đ", null, null, 4.8m)
                ]
            },
            new()
            {
                Id = "section-audio-accessories",
                Title = "Phụ kiện âm thanh",
                Description = "Micro thu âm, mic karaoke, cáp âm thanh và phụ kiện livestream cho nhu cầu học tập, làm việc và giải trí.",
                ViewAllUrl = "/catalog?cat=audio&type=audio-accessories",
                Banner = new CategorySectionBannerViewModel
                {
                    Title = "Mic Karaoke",
                    Subtitle = "Bảo hành chính hãng",
                    ImageUrl = $"{AudioImageRoot}/audio-04.webp",
                    ImageAlt = "Micro karaoke",
                    Theme = "rose"
                },
                Subcategories =
                [
                    AudioPill("Micro thu âm", "type=recording-mic", "audio-03.webp", true),
                    AudioPill("Mic Karaoke", "type=karaoke-mic", "audio-04.webp"),
                    AudioPill("Cáp âm thanh", "type=audio-cable", "audio-10.webp")
                ],
                SortOptions = sortOptions,
                Products =
                [
                    AudioProduct("dji-mic-mini-2-mic", "Microphone không dây DJI Mini (TX+TX+RX) 2 Mic", "audio-03.webp", "2.300.000đ", "3.690.000đ", "Giảm 38%", ["2 Mic", "USB-C", "48h"], "Smember giảm đến 23.000đ", null, null, 5m),
                    AudioProduct("dji-mic-mini-1-mic", "Microphone không dây DJI Mic Mini 2 (1 TX + 1 Mobile)", "audio-03.webp", "1.745.000đ", "2.190.000đ", "Giảm 20%", ["1 Mic", "USB-C", "Bluetooth"], "Smember giảm đến 17.000đ", null, null, 4.8m, "Hàng mới về"),
                    AudioProduct("dji-mic-mini-mobile", "Microphone không dây DJI Mic Mini 2 (2 TX + 1 Mobile)", "audio-03.webp", "2.340.000đ", "3.290.000đ", "Giảm 29%", ["2 TX", "1 Mobile", "48h"], "Smember giảm đến 23.000đ", null, null, 4.9m, "Hàng mới về"),
                    AudioProduct("gocheck-ultra-s24", "Microphone thu âm không dây Gocheck Ultra S24 - S2401", "audio-04.webp", "890.000đ", "1.190.000đ", "Giảm 25%", ["USB-C", "Khử ồn", "Livestream"], "Smember giảm đến 9.000đ", null, null, 4.7m),
                    AudioProduct("gocheck-ultra-s24-plus", "Microphone thu âm không dây Gocheck Ultra S24 - S2402", "audio-04.webp", "1.440.000đ", "1.790.000đ", "Giảm 20%", ["2 Mic", "Type-C", "Pin 8h"], "Smember giảm đến 14.000đ", null, null, 4.8m),
                    AudioProduct("havit-sk208-pro", "Loa vi tính Havit SK208 Pro", "audio-05.webp", "250.000đ", "350.000đ", "Giảm 29%", ["5W x 2", "Jack 3.5", "Đèn RGB"], "Smember giảm đến 5.000đ", null, null, 4.5m),
                    AudioProduct("maono-au-pm461", "Micro thu âm Maono AU-PM461TR USB", "audio-03.webp", "690.000đ", "990.000đ", "Giảm 30%", ["USB", "Cardioid", "Plug and play"], "Smember giảm đến 7.000đ", null, null, 4.8m),
                    AudioProduct("takstar-pcm-1200", "Micro thu âm Takstar PCM-1200", "audio-03.webp", "1.290.000đ", "1.790.000đ", "Giảm 28%", ["Condenser", "USB-C", "Podcast"], "Smember giảm đến 13.000đ", null, null, 4.6m),
                    AudioProduct("ugreen-audio-cable", "Cáp âm thanh Ugreen 3.5mm dài 2m", "audio-10.webp", "129.000đ", "190.000đ", "Giảm 32%", ["3.5mm", "2m", "Nylon"], "Smember giảm đến 2.000đ", null, null, 5m),
                    AudioProduct("orico-sound-card", "Sound card livestream Orico SC2", "audio-10.webp", "490.000đ", "690.000đ", "Giảm 29%", ["USB", "2 cổng mic", "Monitor"], "Smember giảm đến 5.000đ", null, null, 4.6m),
                    AudioProduct("rode-wireless-me", "Micro không dây Rode Wireless ME", "audio-04.webp", "2.990.000đ", "3.590.000đ", "Giảm 17%", ["2.4GHz", "USB-C", "32h"], "Smember giảm đến 30.000đ", null, null, 4.9m),
                    AudioProduct("hyperx-solocast", "Micro thu âm HyperX SoloCast USB", "audio-03.webp", "1.190.000đ", "1.490.000đ", "Giảm 20%", ["USB", "Tap mute", "Gaming"], "Smember giảm đến 12.000đ", null, null, 4.8m)
                ]
            }
        ];
    }

    private static ProductCardViewModel Product(
        string id,
        string name,
        string imageName,
        string price,
        string oldPrice,
        string discount,
        IReadOnlyList<string> specifications,
        string memberOffer,
        string promotionNote,
        string? studentOffer,
        decimal rating,
        string? availability = null)
    {
        return HomeProductCardFactory.Create(
            id,
            name,
            $"{PhoneImageRoot}/{imageName}",
            price,
            oldPrice,
            discount,
            memberOffer,
            promotionNote,
            rating,
            studentOffer,
            name);
    }

    private static ProductCardViewModel AudioProduct(
        string id,
        string name,
        string imageName,
        string price,
        string oldPrice,
        string discount,
        IReadOnlyList<string> specifications,
        string memberOffer,
        string? promotionNote,
        string? studentOffer,
        decimal rating,
        string? availability = null)
    {
        return HomeProductCardFactory.Create(
            id,
            name,
            $"{AudioImageRoot}/{imageName}",
            price,
            oldPrice,
            discount,
            memberOffer,
            promotionNote ?? string.Empty,
            rating,
            studentOffer,
            name);
    }

    private static ProductCardViewModel ToHotSaleCard(ProductCardViewModel product)
    {
        return new ProductCardViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Url = product.Url,
            ImageUrl = product.ImageUrl,
            ImageAlt = product.ImageAlt,
            CurrentPrice = product.CurrentPrice,
            OldPrice = product.OldPrice,
            DiscountLabel = product.DiscountLabel,
            InstallmentLabel = product.InstallmentLabel,
            Rating = product.Rating
        };
    }

    private static CategoryQuickLinkViewModel QuickLink(
        string label,
        string query,
        string imageName)
    {
        return new CategoryQuickLinkViewModel
        {
            Label = label,
            Url = $"/catalog?cat=phone&{query}",
            ImageUrl = $"{PhoneImageRoot}/{imageName}",
            ImageAlt = label
        };
    }

    private static CategoryQuickLinkViewModel AudioQuickLink(
        string label,
        string query,
        string imageName)
    {
        return new CategoryQuickLinkViewModel
        {
            Label = label,
            Url = $"/catalog?cat=audio&{query}",
            ImageUrl = $"{AudioImageRoot}/{imageName}",
            ImageAlt = label
        };
    }

    private static CategorySectionPillViewModel AudioPill(
        string label,
        string query,
        string imageName,
        bool isActive = false)
    {
        return new CategorySectionPillViewModel
        {
            Label = label,
            Url = $"/catalog?cat=audio&{query}",
            ImageUrl = $"{AudioImageRoot}/{imageName}",
            ImageAlt = label,
            IsActive = isActive
        };
    }

    private static CategoryFilterItemViewModel FilterItem(
        string label,
        string query,
        string? icon = null,
        bool dropdown = false,
        bool emphasized = false)
    {
        return new CategoryFilterItemViewModel
        {
            Label = label,
            Url = $"/catalog?cat=phone&{query}",
            Icon = icon,
            HasDropdown = dropdown,
            IsEmphasized = emphasized
        };
    }

    private static CategorySortOptionViewModel SortItem(
        string label,
        string value,
        string icon,
        string activeSort,
        string category = "phone")
    {
        return new CategorySortOptionViewModel
        {
            Label = label,
            Url = $"/catalog?cat={category}&sort={value}",
            Icon = icon,
            IsActive = string.Equals(value, activeSort, StringComparison.OrdinalIgnoreCase)
        };
    }

    private static IReadOnlyList<CategorySortOptionViewModel> CreateAudioSortOptions(CategoryPageRequest request)
    {
        var activeSort = string.IsNullOrWhiteSpace(request.Sort)
            ? "popular"
            : request.Sort.Trim().ToLowerInvariant();

        return
        [
            SortItem("Phổ biến", "popular", "star", activeSort, "audio"),
            SortItem("Khuyến mãi HOT", "promotion", "discount", activeSort, "audio"),
            SortItem("Giá Thấp - Cao", "price-asc", "sort-up", activeSort, "audio"),
            SortItem("Giá Cao - Thấp", "price-desc", "sort-down", activeSort, "audio")
        ];
    }

    private static CategoryPageViewModel CreateGenericPage(CategoryPageRequest request)
    {
        var slug = NormalizeCategorySlug(request.Slug);
        var title = ResolveCategoryTitle(slug);
        var panel = ResolveHomeCategoryPanel(slug);
        var products = panel?.Products ?? ResolveFallbackProducts(slug);
        var activeSort = string.IsNullOrWhiteSpace(request.Sort)
            ? "popular"
            : request.Sort.Trim().ToLowerInvariant();

        products = activeSort switch
        {
            "price-asc" => products.OrderBy(product => ParsePrice(product.CurrentPrice)).ToList(),
            "price-desc" => products.OrderByDescending(product => ParsePrice(product.CurrentPrice)).ToList(),
            _ => products
        };

        return new CategoryPageViewModel
        {
            Slug = slug,
            Title = title,
            MetaDescription = $"Khám phá {title.ToLowerInvariant()} chính hãng, giá tốt và giao nhanh tại TechStore.",
            LayoutMode = CategoryPageLayoutMode.FilterListing,
            Breadcrumbs =
            [
                new() { Label = "Trang chủ", Url = "/" },
                new() { Label = title, Url = $"/catalog?cat={Uri.EscapeDataString(slug)}", IsCurrent = true }
            ],
            PromotionBanners = panel?.Banners.Select((banner, index) => new CategoryPromotionBannerViewModel
            {
                Id = $"{slug}-banner-{index + 1}",
                Kicker = "Ưu đãi nổi bật",
                Title = title,
                PriceText = "Xem sản phẩm",
                ImageUrl = banner.ImageUrl,
                ImageAlt = banner.ImageAlt,
                Url = banner.Url,
                Theme = index % 2 == 0 ? "peach" : "violet"
            }).ToList() ?? [],
            Brands = panel?.Brands.Select(brand => new CategoryBrandViewModel
            {
                Id = Slugify(brand.Label),
                Label = brand.Label,
                Url = brand.Url
            }).ToList() ?? [],
            QuickLinks = panel?.QuickLinks.Select(link => new CategoryQuickLinkViewModel
            {
                Label = link.Label,
                Url = link.Url,
                ImageUrl = link.ImageUrl,
                ImageAlt = link.Label
            }).ToList() ?? [],
            HotSale = new CategoryHotSaleViewModel
            {
                Title = "Sản phẩm nổi bật",
                Products = products.Take(8).Select(ToHotSaleCard).ToList()
            },
            Filter = CreateGenericFilter(slug, activeSort),
            Products = products,
            InitialProductCount = 20,
            SeoContent = new CategorySeoContentViewModel
            {
                Title = $"{title} chính hãng tại TechStore",
                Paragraphs =
                [
                    $"Danh mục {title.ToLowerInvariant()} đang sử dụng dữ liệu mẫu hiện có để bạn có thể xem danh sách, mở chi tiết sản phẩm và thử đầy đủ luồng mua hàng.",
                    "Giá bán, ưu đãi và tình trạng sản phẩm trong chế độ mock chỉ mang tính minh họa."
                ]
            },
            QuestionAnswer = CreateGenericQuestionAnswer(title)
        };
    }

    private static e_commerce_web_customer.ViewModels.Home.CategoryProductPanelViewModel? ResolveHomeCategoryPanel(
        string slug)
    {
        var tab = slug switch
        {
            "tablet" => PhoneCategorySectionFactory.Create().Tabs.FirstOrDefault(item => item.Id == "tablets"),
            "laptop" => ComputerCategorySectionFactory.Create().Tabs.FirstOrDefault(item => item.Id == "laptops"),
            "monitor" => ComputerCategorySectionFactory.Create().Tabs.FirstOrDefault(item => item.Id == "monitors"),
            "desktop" or "pc" or "printer" => ComputerCategorySectionFactory.Create().Tabs.FirstOrDefault(item => item.Id == "desktop-pcs"),
            "computer-accessories" => ComputerCategorySectionFactory.Create().Tabs.FirstOrDefault(item => item.Id == "computer-accessories"),
            "smartwatch" or "watch" or "camera" or "camera-accessories" => AudioWearablesCategorySectionFactory.Create().Tabs.FirstOrDefault(item => item.Id == "watches"),
            "tv" or "home-electronics" or "entertainment" => ComputerCategorySectionFactory.Create().Tabs.FirstOrDefault(item => item.Id == "monitors"),
            _ => null
        };

        return tab?.Panel;
    }

    private static IReadOnlyList<ProductCardViewModel> ResolveFallbackProducts(string slug)
    {
        if (slug is "accessories" or "appliances" or "beauty" or "health")
        {
            return CreateAccessoryProducts();
        }

        return CreatePhoneProducts();
    }

    private static IReadOnlyList<ProductCardViewModel> CreateAccessoryProducts()
    {
        return AccessoryDirectoryFactory.Create().Items.Select((item, index) =>
        {
            var typeMarker = "type=";
            var markerIndex = item.Url.IndexOf(typeMarker, StringComparison.OrdinalIgnoreCase);
            var type = markerIndex >= 0
                ? item.Url[(markerIndex + typeMarker.Length)..]
                : $"item-{index + 1}";

            return new ProductCardViewModel
            {
                Id = $"accessory-{type}",
                Name = item.Label,
                Url = $"/product/accessory-{type}",
                ImageUrl = item.ImageUrl,
                ImageAlt = item.ImageAlt,
                CurrentPrice = $"{199_000m + (index * 50_000m):N0}đ",
                InstallmentLabel = null,
                DeliveryLabel = "Giao 2 giờ",
                Location = "Hồ Chí Minh",
                Rating = 4.8m
            };
        }).ToList();
    }

    private static CategoryFilterViewModel CreateGenericFilter(string slug, string activeSort)
    {
        return new CategoryFilterViewModel
        {
            Title = "Chọn theo tiêu chí",
            PrimaryItems =
            [
                new()
                {
                    Label = "Bộ lọc",
                    Url = $"/catalog?cat={Uri.EscapeDataString(slug)}&filter=all",
                    Icon = "filter",
                    IsEmphasized = true
                },
                new()
                {
                    Label = "Sẵn hàng",
                    Url = $"/catalog?cat={Uri.EscapeDataString(slug)}&availability=in-stock",
                    Icon = "truck"
                },
                new()
                {
                    Label = "Xem theo giá",
                    Url = $"/catalog?cat={Uri.EscapeDataString(slug)}&filter=price",
                    Icon = "price"
                }
            ],
            SecondaryItems = [],
            SortOptions =
            [
                SortItem("Phổ biến", "popular", "star", activeSort, slug),
                SortItem("Giá Thấp - Cao", "price-asc", "sort-up", activeSort, slug),
                SortItem("Giá Cao - Thấp", "price-desc", "sort-down", activeSort, slug)
            ]
        };
    }

    private static QuestionAnswerSectionViewModel CreateGenericQuestionAnswer(string categoryTitle)
    {
        return new QuestionAnswerSectionViewModel
        {
            Title = "Hỏi và đáp",
            FormTitle = $"Bạn cần tư vấn về {categoryTitle.ToLowerInvariant()}?",
            Description = "Hãy gửi nhu cầu và ngân sách dự kiến. TechStore sẽ gợi ý sản phẩm phù hợp.",
            Placeholder = "Viết câu hỏi của bạn tại đây",
            SubmitLabel = "Gửi câu hỏi",
            VisibleThreadLimit = 2,
            Threads =
            [
                QuestionThread(
                    "Minh Anh",
                    "M",
                    "2 ngày trước",
                    $"Danh mục {categoryTitle.ToLowerInvariant()} có hỗ trợ trả góp không?",
                    "Chào bạn, các sản phẩm đủ điều kiện sẽ hiển thị tùy chọn trả góp trong trang chi tiết. Chính sách cụ thể phụ thuộc sản phẩm và phương thức thanh toán."),
                QuestionThread(
                    "Quốc Bảo",
                    "B",
                    "1 tuần trước",
                    "Mua online có được giao nhanh không?",
                    "Chào bạn, thời gian giao dự kiến sẽ được xác nhận theo khu vực và tình trạng hàng khi đặt mua.")
            ]
        };
    }

    private static string NormalizeCategorySlug(string slug)
    {
        return slug.Trim().ToLowerInvariant() switch
        {
            "mobile" => "phone",
            "am-thanh" or "thiet-bi-am-thanh" or "speaker" or "microphone" => "audio",
            "pc" => "desktop",
            "watch" => "smartwatch",
            _ => slug.Trim().ToLowerInvariant()
        };
    }

    private static string ResolveCategoryTitle(string slug)
    {
        return slug switch
        {
            "tablet" => "Máy tính bảng",
            "laptop" => "Laptop",
            "monitor" => "Màn hình máy tính",
            "desktop" => "PC và máy tính để bàn",
            "computer-accessories" => "Linh kiện máy tính",
            "smartwatch" => "Đồng hồ thông minh",
            "camera" or "camera-accessories" => "Camera và thiết bị quay",
            "accessories" => "Phụ kiện",
            "appliances" => "Đồ gia dụng",
            "beauty" => "Thiết bị làm đẹp",
            "health" => "Thiết bị chăm sóc sức khỏe",
            "tv" => "Tivi",
            "home-electronics" => "Điện máy",
            "entertainment" => "Thiết bị giải trí",
            "printer" => "Máy in và thiết bị văn phòng",
            "used" => "Hàng cũ",
            "trade-in" => "Thu cũ đổi mới",
            "deals" => "Sản phẩm khuyến mãi",
            "tech" => "Sản phẩm công nghệ",
            _ => HumanizeSlug(slug)
        };
    }

    private static string HumanizeSlug(string slug)
    {
        var words = slug.Split('-', StringSplitOptions.RemoveEmptyEntries);
        return words.Length == 0
            ? "Sản phẩm"
            : string.Join(' ', words.Select(word => char.ToUpperInvariant(word[0]) + word[1..]));
    }

    private static decimal ParsePrice(string price)
    {
        var digits = new string(price.Where(char.IsDigit).ToArray());
        return decimal.TryParse(digits, out var value) ? value : 0m;
    }

    private static QuestionAnswerSectionViewModel CreateQuestionAnswer()
    {
        return new QuestionAnswerSectionViewModel
        {
            Title = "Hỏi và đáp",
            FormTitle = "Bạn cần tư vấn chọn điện thoại?",
            Description = "TechStore sẽ phản hồi trong vòng 1 giờ trong thời gian làm việc. Hãy cho chúng tôi biết nhu cầu, ngân sách hoặc mẫu máy bạn đang quan tâm.",
            Placeholder = "Viết câu hỏi của bạn tại đây",
            SubmitLabel = "Gửi câu hỏi",
            VisibleThreadLimit = 3,
            AdditionalCommentCount = 18,
            Threads =
            [
                QuestionThread(
                    "Trần Minh Hòa",
                    "H",
                    "2 ngày trước",
                    "Ngân sách khoảng 15 triệu thì nên chọn máy nào để chơi game và dùng lâu dài?",
                    "Chào anh Hòa, trong tầm giá này anh có thể tham khảo POCO X8 Pro Max hoặc HONOR 400 Pro. Nếu ưu tiên hiệu năng, POCO phù hợp hơn. Nếu cần camera và thiết kế cân bằng, HONOR là lựa chọn dễ dùng hơn."),
                QuestionThread(
                    "Nguyễn Khánh Linh",
                    "L",
                    "5 ngày trước",
                    "Mua điện thoại online có được kiểm tra máy trước khi nhận không?",
                    "Chào chị Linh, đơn giao nhanh được kiểm tra tình trạng hộp và sản phẩm theo chính sách giao nhận. Nhân viên sẽ liên hệ xác nhận thời gian và hướng dẫn chi tiết trước khi giao."),
                QuestionThread(
                    "Phạm Quốc Bảo",
                    "B",
                    "1 tuần trước",
                    "Máy Samsung trong danh sách có hỗ trợ trả góp 0% bằng thẻ tín dụng không?",
                    "Chào anh Bảo, nhiều mẫu Samsung đang hỗ trợ trả góp 0% qua thẻ tín dụng với kỳ hạn 3-6 tháng. Điều kiện cụ thể phụ thuộc ngân hàng và chương trình tại thời điểm đặt hàng."),
                QuestionThread(
                    "Lê Hoài An",
                    "A",
                    "2 tuần trước",
                    "Có thể giữ hàng tại cửa hàng gần nhất trước khi đến xem máy không?",
                    "Chào anh/chị, TechStore có hỗ trợ giữ hàng theo tình trạng tồn kho. Anh/chị chọn sản phẩm và khu vực, nhân viên sẽ kiểm tra cửa hàng thuận tiện nhất trước khi xác nhận.")
            ]
        };
    }

    private static QuestionAnswerSectionViewModel CreateAudioQuestionAnswer()
    {
        return new QuestionAnswerSectionViewModel
        {
            Title = "Hỏi và đáp",
            FormTitle = "Bạn cần tư vấn thiết bị âm thanh?",
            Description = "Cho TechStore biết bạn cần tai nghe, loa hay micro cho nhu cầu nào. Nhân viên sẽ gợi ý mẫu phù hợp theo ngân sách và cách sử dụng.",
            Placeholder = "Viết câu hỏi của bạn tại đây",
            SubmitLabel = "Gửi câu hỏi",
            VisibleThreadLimit = 3,
            AdditionalCommentCount = 12,
            Threads =
            [
                QuestionThread(
                    "Hoàng Minh Đức",
                    "Đ",
                    "1 ngày trước",
                    "Tai nghe khoảng 3 triệu nên chọn AirPods 4 hay JBL Live Pro 2 để nghe nhạc và gọi điện?",
                    "Chào anh Đức, nếu anh dùng iPhone và cần kết nối nhanh thì AirPods 4 rất hợp. Nếu ưu tiên chống ồn, pin lâu và chất âm nhiều bass hơn thì JBL Live Pro 2 đáng cân nhắc hơn."),
                QuestionThread(
                    "Nguyễn Thu Hà",
                    "H",
                    "3 ngày trước",
                    "Loa Bluetooth dùng ngoài trời thì nên chọn mẫu nào pin tốt và chống nước ổn?",
                    "Chào chị Hà, chị có thể tham khảo JBL Flip 6 hoặc JBL Charge 6. Flip 6 gọn hơn, còn Charge 6 pin lâu và âm lượng lớn hơn cho không gian ngoài trời."),
                QuestionThread(
                    "Lê Quốc Bảo",
                    "B",
                    "6 ngày trước",
                    "Mic thu âm cho livestream bằng điện thoại có cần thêm sound card không?",
                    "Chào anh Bảo, nếu dùng micro không dây có cổng USB-C tương thích thì chưa cần sound card. Sound card sẽ hữu ích khi anh cần chỉnh echo, monitor hoặc ghép nhiều nguồn âm."),
                QuestionThread(
                    "Phạm Linh Chi",
                    "C",
                    "1 tuần trước",
                    "Mua loa karaoke Sony có hỗ trợ trả góp không?",
                    "Chào chị Chi, nhiều mẫu loa Sony đang hỗ trợ trả góp 0% theo kỳ hạn của ngân hàng. Chị chọn sản phẩm và khu vực giao hàng, hệ thống sẽ hiển thị phương án phù hợp.")
            ]
        };
    }

    private static QuestionThreadViewModel QuestionThread(
        string author,
        string initial,
        string timeAgo,
        string question,
        string reply)
    {
        return new QuestionThreadViewModel
        {
            Author = author,
            Initial = initial,
            TimeAgo = timeAgo,
            Question = question,
            Replies =
            [
                new QuestionReplyViewModel
                {
                    Author = "Quản Trị Viên",
                    Badge = "QTV",
                    TimeAgo = timeAgo,
                    Content = reply
                }
            ]
        };
    }

    private static string Slugify(string value)
    {
        return value.Trim().ToLowerInvariant().Replace(" ", "-", StringComparison.Ordinal);
    }
}
