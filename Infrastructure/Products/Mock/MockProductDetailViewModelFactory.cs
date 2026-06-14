using e_commerce_web_customer.Infrastructure.Catalog.Mock;
using e_commerce_web_customer.Application.Product;
using System.Globalization;
using System.Text;
using e_commerce_web_customer.Application.Catalog;
using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Infrastructure.Home.Mock;
using e_commerce_web_customer.Application.Products;
using e_commerce_web_customer.Application.Search;
using e_commerce_web_customer.ViewModels.Product;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Products.Mock;

public sealed class MockProductDetailViewModelFactory(
    IProductCatalog productCatalog) : IProductDetailViewModelFactory
{
    private const string PhoneImageRoot = "/images/products/phone";
    private static readonly Lazy<IReadOnlyList<ProductCardViewModel>> AllMockProducts =
        new(CreateAllMockProducts);

    public async Task<ProductDetailViewModel?> CreateAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        var normalizedSlug = slug.Trim().ToLowerInvariant();

        if (normalizedSlug.StartsWith("iphone-17-pro-max", StringComparison.Ordinal))
        {
            return CreateIphone17ProMax(normalizedSlug);
        }

        var catalogProduct = await productCatalog.GetByIdAsync(
            normalizedSlug,
            cancellationToken);
        var productCard = catalogProduct is null
            ? FindMockProduct(normalizedSlug)
            : ProductViewModelMapper.ToProductCard(catalogProduct);

        return CreateGenericProduct(normalizedSlug, productCard);
    }

    private static ProductDetailViewModel CreateGenericProduct(
        string slug,
        ProductCardViewModel? product)
    {
        var name = product?.Name ?? HumanizeProductSlug(slug);
        var imageUrl = product?.ImageUrl ?? ResolveFallbackImage(slug);
        var currentPrice = ParsePrice(product?.CurrentPrice) ?? ResolveFallbackPrice(slug);
        var oldPrice = ParsePrice(product?.OldPrice);
        var category = ResolveProductCategory(slug, imageUrl);
        var brand = ResolveBrand(name);

        return new ProductDetailViewModel
        {
            Slug = slug,
            Name = name,
            Brand = brand,
            MainImageUrl = imageUrl,
            MainImageAlt = product?.ImageAlt ?? name,
            CurrentPrice = currentPrice,
            OldPrice = oldPrice,
            Rating = product?.Rating ?? 4.8m,
            ReviewCount = 12,
            Breadcrumbs =
            [
                new() { Label = "Trang chủ", Url = "/" },
                new() { Label = category.Label, Url = $"/catalog?cat={category.Slug}" },
                new() { Label = name }
            ],
            QuickLinks =
            [
                new() { Label = "Yêu thích", IconId = "product-card-icon-heart" },
                new() { Label = "Hỏi đáp", IconId = "hero-icon-news", Url = "#block-comment-cps" },
                new() { Label = "Thông số", IconId = "hero-icon-phone" },
                new() { Label = "So sánh", IconId = "hero-icon-swap" }
            ],
            GalleryItems =
            [
                new()
                {
                    Label = "Sản phẩm",
                    ImageUrl = imageUrl,
                    ImageAlt = product?.ImageAlt ?? name
                }
            ],
            StorageOptions =
            [
                new()
                {
                    Label = "Phiên bản tiêu chuẩn",
                    Url = $"/product/{slug}",
                    IsActive = true
                }
            ],
            ColorOptions =
            [
                new()
                {
                    Name = "Mặc định",
                    ImageUrl = imageUrl,
                    ImageAlt = product?.ImageAlt ?? name,
                    Price = currentPrice,
                    IsActive = true
                }
            ],
            TechnicalSpecSections = CreateGenericTechnicalSpecs(slug, category.Label, brand),
            RelatedProductGroups = CreateGenericRelatedProductGroups(slug, category.Slug),
            ReviewSummary = CreateGenericReviewSummary(name),
            QuestionAnswerSection = CreateGenericQuestionAnswer(name)
        };
    }

    private static ProductDetailViewModel CreateIphone17ProMax(string slug)
    {
        var activeStorage = ResolveActiveStorage(slug);
        const decimal basePrice = 36_990_000m;
        const decimal oldPrice = 37_990_000m;

        return new ProductDetailViewModel
        {
            Slug = slug,
            Name = $"iPhone 17 Pro Max {activeStorage} | Chính hãng",
            Brand = "Apple",
            MainImageUrl = $"{PhoneImageRoot}/phone-orange-cutout.png",
            MainImageAlt = "iPhone 17 Pro Max màu cam vũ trụ",
            CurrentPrice = basePrice,
            OldPrice = oldPrice,
            Rating = 5m,
            ReviewCount = 34,
            Breadcrumbs =
            [
                new() { Label = "Trang chủ", Url = "/" },
                new() { Label = "Điện thoại", Url = "/catalog?cat=phone" },
                new() { Label = "Apple", Url = "/catalog?cat=phone&brand=apple" },
                new() { Label = "Điện thoại iPhone 17", Url = "/catalog?cat=phone&brand=apple&series=iphone-17" },
                new() { Label = $"iPhone 17 Pro Max {activeStorage} | Chính hãng" }
            ],
            QuickLinks =
            [
                new() { Label = "Yêu thích", IconId = "product-card-icon-heart" },
                new() { Label = "Hỏi đáp", IconId = "hero-icon-news", Url = "#block-comment-cps" },
                new() { Label = "Thông số", IconId = "hero-icon-phone" },
                new() { Label = "So sánh", IconId = "hero-icon-swap" }
            ],
            GalleryItems =
            [
                new()
                {
                    Label = "Cam Vũ Trụ",
                    ImageUrl = $"{PhoneImageRoot}/phone-orange-cutout.png",
                    ImageAlt = "iPhone 17 Pro Max màu cam vũ trụ"
                },
                new()
                {
                    Label = "Mặt trước",
                    ImageUrl = $"{PhoneImageRoot}/phone-gaming-cutout.png",
                    ImageAlt = "Mặt trước iPhone 17 Pro Max"
                },
                new()
                {
                    Label = "Camera",
                    ImageUrl = $"{PhoneImageRoot}/phone-camera-cutout.png",
                    ImageAlt = "Cụm camera iPhone 17 Pro Max"
                },
                new()
                {
                    Label = "Cạnh máy",
                    ImageUrl = $"{PhoneImageRoot}/phone-rose-cutout.png",
                    ImageAlt = "Cạnh máy iPhone 17 Pro Max"
                },
                new()
                {
                    Label = "Ảnh thực tế",
                    ImageUrl = "/images/home/hero-smartphones.webp",
                    ImageAlt = "Ảnh thực tế các màu iPhone 17"
                },
                new()
                {
                    Label = "Tính năng nổi bật",
                    ImageUrl = "/images/home/hero-smartphones.webp",
                    ImageAlt = "Các tính năng nổi bật của iPhone 17",
                    IsFeatureHighlight = true
                }
            ],
            StorageOptions =
            [
                Storage("iPhone 17 Pro Max 256GB", "/product/iphone-17-pro-max-256gb", slug),
                Storage("iPhone 17 Pro Max 512GB", "/product/iphone-17-pro-max-512gb", slug),
                Storage("iPhone 17 Pro Max 1TB", "/product/iphone-17-pro-max-1tb", slug),
                Storage("iPhone 17 Pro 256GB", "/product/iphone-17-pro-256gb", slug),
                Storage("iPhone 17 Pro 512GB", "/product/iphone-17-pro-512gb", slug),
                Storage("iPhone 17 256GB", "/product/iphone-17-256gb", slug, isInitiallyHidden: true),
                Storage("iPhone 17 Plus 256GB", "/product/iphone-17-plus-256gb", slug, isInitiallyHidden: true)
            ],
            ColorOptions =
            [
                new()
                {
                    Name = "Tím Cobalt",
                    ImageUrl = $"{PhoneImageRoot}/phone-violet-cutout.png",
                    ImageAlt = "iPhone 17 Pro Max màu tím cobalt",
                    Price = basePrice
                },
                new()
                {
                    Name = "Đen Classic",
                    ImageUrl = $"{PhoneImageRoot}/phone-gaming-cutout.png",
                    ImageAlt = "iPhone 17 Pro Max màu đen classic",
                    Price = basePrice
                },
                new()
                {
                    Name = "Xanh Sky Blue",
                    ImageUrl = $"{PhoneImageRoot}/phone-camera-cutout.png",
                    ImageAlt = "iPhone 17 Pro Max màu xanh sky blue",
                    Price = basePrice
                },
                new()
                {
                    Name = "Trắng Classic",
                    ImageUrl = $"{PhoneImageRoot}/phone-rose-cutout.png",
                    ImageAlt = "iPhone 17 Pro Max màu trắng classic",
                    Price = basePrice,
                    IsActive = true
                }
            ],
            TechnicalSpecSections = CreateTechnicalSpecSections(activeStorage),
            RelatedProductGroups = CreateRelatedProductGroups(),
            ReviewSummary = CreateReviewSummary($"iPhone 17 Pro Max {activeStorage}"),
            QuestionAnswerSection = CreateQuestionAnswerSection(activeStorage)
        };
    }

    private static ProductDetailStorageOptionViewModel Storage(
        string label,
        string url,
        string slug,
        bool isInitiallyHidden = false)
    {
        return new ProductDetailStorageOptionViewModel
        {
            Label = label,
            Url = url,
            IsActive = url.EndsWith($"/{slug}", StringComparison.OrdinalIgnoreCase),
            IsInitiallyHidden = isInitiallyHidden
        };
    }

    private static IReadOnlyList<ProductTechnicalSpecSectionViewModel> CreateTechnicalSpecSections(string activeStorage)
    {
        return
        [
            Section("cau-hinh-bo-nho", "Cấu hình & Bộ nhớ",
            [
                Row("Hệ điều hành", "iOS 20", isHighlighted: true),
                Row("Chipset", "Apple A21 Pro", isHighlighted: true),
                Row("Dung lượng RAM", "12 GB", isHighlighted: true),
                Row("Bộ nhớ trong", activeStorage, isHighlighted: true),
                Row("Loại CPU", "8 nhân"),
                Row("GPU", "Apple GPU thế hệ mới", isHighlighted: true)
            ]),
            Section("man-hinh", "Màn hình",
            [
                Row("Kích thước màn hình", "6.9 inches", isHighlighted: true),
                Row("Công nghệ màn hình", "Super Retina XDR OLED", isHighlighted: true),
                Row("Độ phân giải màn hình", "2868 x 1320 pixels"),
                Row("Tính năng màn hình", "Always-On display\nDynamic Island\nĐộ sáng tối đa 2600 nits"),
                Row("Tần số quét", "120Hz", isHighlighted: true),
                Row("Kiểu màn hình", "Đục lỗ Dynamic Island")
            ]),
            Section("camera-sau", "Camera sau",
            [
                Row("Camera sau", "Góc rộng 48 MP, f/1.8\nGóc siêu rộng 48 MP, f/2.2\nTele 48 MP, f/2.8"),
                Row("Quay video", "4K Dolby Vision@120fps\n4K@60fps\n1080p@240fps"),
                Row("Tính năng camera", "Zoom quang học\nZoom kỹ thuật số\nTự động lấy nét (AF)\nChống rung quang học (OIS)\nBan đêm\nChế độ quay phim 10-bit HDR\nKhoá đường chân trời chuẩn điện ảnh")
            ]),
            Section("camera-truoc", "Camera trước",
            [
                Row("Camera trước", "24 MP, f/1.9")
            ]),
            Section("pin-sac", "Pin & công nghệ sạc",
            [
                Row("Pin", "5000 mAh", isHighlighted: true),
                Row("Công nghệ sạc", "Sạc ngược không dây\nSạc không dây MagSafe\nSạc nhanh qua USB-C"),
                Row("Cổng sạc", "USB Type-C", isHighlighted: true)
            ]),
            Section("giao-tiep-ket-noi", "Giao tiếp & kết nối",
            [
                Row("Hỗ trợ mạng", "5G", isHighlighted: true),
                Row("Thẻ SIM", "2 eSIM hoặc 1 Nano SIM + 1 eSIM", isHighlighted: true),
                Row("Wi-Fi", "Wi-Fi Direct\nWi-Fi 7\n802.11a/b/g/n/ac/ax/be\n2.4GHz + 5GHz + 6GHz\nMIMO", isHighlighted: true),
                Row("Bluetooth", "v6.0", isHighlighted: true),
                Row("Công nghệ NFC", "Có", isHighlighted: true),
                Row("GPS", "GPS\nGLONASS\nBEIDOU\nGALILEO\nQZSS", isHighlighted: true)
            ]),
            Section("cong-nghe-tien-ich", "Công nghệ & Tiện ích",
            [
                Row("Cảm biến vân tay", "Face ID"),
                Row("Các loại cảm biến", "Cảm biến gia tốc, Cảm biến tiệm cận, Cảm biến ánh sáng,\nCon quay hồi chuyển, La bàn, Cảm biến áp kế", isHighlighted: true),
                Row("Chỉ số kháng nước, bụi", "IP68"),
                Row("Công nghệ - Tiện ích", "Khoanh tròn để tìm kiếm\nPrivacy Display\nLọc cuộc gọi tự động\nKhông gian sáng tạo\nGợi ý thông minh\nChỉnh sửa ảnh thế hệ mới\nAirDrop\nApple Intelligence"),
                Row("Tiện ích khác", "Ghi âm mặc định\nUWB\nNFC"),
                Row("Công nghệ âm thanh", "Hỗ trợ âm thanh nổi")
            ]),
            Section("thiet-ke-chat-lieu", "Thiết kế & Chất liệu",
            [
                Row("Chất liệu khung viền", "Titanium"),
                Row("Chất liệu mặt lưng", "Kính cường lực Ceramic Shield"),
                Row("Kích thước", "163.6 x 78.1 x 7.9mm"),
                Row("Trọng lượng", "214g"),
                Row("Thời điểm ra mắt", "09/2026")
            ])
        ];
    }

    private static IReadOnlyList<ProductRelatedProductGroupViewModel> CreateRelatedProductGroups()
    {
        return
        [
            new()
            {
                Id = "similar",
                Label = "Sản phẩm tương tự",
                IsActive = true,
                Products =
                [
                    Related("Xiaomi 17 Ultra 5G 16GB 512GB", $"{PhoneImageRoot}/phone-camera-cutout.png", "Xiaomi 17 Ultra màu đen", 32_690_000m, 39_990_000m, "Giảm 18%", "Giảm thêm 500.000đ cho MTB Xiaomi - Poco khi mua cùng bất ...", rating: 5m),
                    Related("iPhone 17 512GB | Chính hãng", $"{PhoneImageRoot}/phone-rose-cutout.png", "iPhone 17 màu hồng", 29_890_000m, 31_490_000m, "Giảm 5%", "Trả góp 0% lãi suất, tối đa 12 tháng, trả trước từ 10% qua CTT...", rating: 5m),
                    Related("iPhone Air 512GB | Chính hãng", $"{PhoneImageRoot}/phone-orange-cutout.png", "iPhone Air màu xanh", 28_990_000m, 38_490_000m, "Giảm 25%", "Trả góp 0% lãi suất, tối đa 12 tháng, trả trước từ 10% qua CTT...", rating: 5m),
                    Related("Samsung Galaxy Z Fold 6 12GB 256GB", $"{PhoneImageRoot}/phone-violet-cutout.png", "Samsung Galaxy Z Fold 6", 29_990_000m, 43_990_000m, "Giảm 32%", "Tặng ốp lưng Samsung", rating: 4.9m),
                    Related("OPPO Find X9 Pro 16GB 512GB", "/images/home/hero-smartphones.webp", "OPPO Find X9 Pro", 32_490_000m, 32_990_000m, "Giảm 2%", "Tặng gói Google One AI Premium miễn phí 3 tháng sử dụng", rating: 5m)
                ]
            },
            new()
            {
                Id = "used",
                Label = "Tham khảo hàng cũ",
                Products =
                [
                    Related("Samsung Galaxy S26 Ultra 5G 12GB 256GB - Đã kích hoạt", $"{PhoneImageRoot}/phone-violet-cutout.png", "Samsung Galaxy S26 Ultra đã kích hoạt", 26_490_000m, null, "Giảm 28%", "Tặng Sim/Esim 5G VNSKY, có ngay 3GB data/ngày+500 phút g...", usedPriceLabel: "Giá hàng mới: 36.990.000", savingLabel: "Tiết kiệm: 10.500.000"),
                    Related("Samsung Galaxy S26 Ultra 5G 12GB 512GB - Đã kích hoạt", $"{PhoneImageRoot}/phone-violet-cutout.png", "Samsung Galaxy S26 Ultra 512GB đã kích hoạt", 30_390_000m, null, "Giảm 29%", "Tặng Sim/Esim 5G VNSKY, có ngay 3GB data/ngày+500 phút g...", usedPriceLabel: "Giá hàng mới: 42.990.000", savingLabel: "Tiết kiệm: 12.600.000"),
                    Related("Samsung Galaxy S26 Ultra 5G 12GB 512GB - Cũ trầy xước", $"{PhoneImageRoot}/phone-gaming-cutout.png", "Samsung Galaxy S26 Ultra cũ trầy xước", 27_890_000m, null, "Giảm 35%", "Tặng Sim/Esim 5G VNSKY, có ngay 3GB data/ngày+500 phút g...", usedPriceLabel: "Giá hàng mới: 42.990.000", savingLabel: "Tiết kiệm: 15.100.000"),
                    Related("Samsung Galaxy S26 Ultra 5G 12GB 512GB - Cũ đẹp", $"{PhoneImageRoot}/phone-camera-cutout.png", "Samsung Galaxy S26 Ultra cũ đẹp", 28_890_000m, null, "Giảm 33%", "Tặng Sim/Esim 5G VNSKY, có ngay 3GB data/ngày+500 phút g...", usedPriceLabel: "Giá hàng mới: 42.990.000", savingLabel: "Tiết kiệm: 14.100.000")
                ]
            }
        ];
    }

    private static ProductReviewSummaryViewModel CreateReviewSummary(string productName)
    {
        return new ProductReviewSummaryViewModel
        {
            Title = $"Đánh giá {productName}",
            Score = 5m,
            TotalReviews = 21,
            RatingBreakdown =
            [
                new() { Stars = 5, Count = 21, Percent = 100 },
                new() { Stars = 4, Count = 0, Percent = 0 },
                new() { Stars = 3, Count = 0, Percent = 0 },
                new() { Stars = 2, Count = 0, Percent = 0 },
                new() { Stars = 1, Count = 0, Percent = 0 }
            ],
            ExperienceRatings =
            [
                new() { Label = "Hiệu năng", Score = 5m, Count = 18 },
                new() { Label = "Thời lượng pin", Score = 4.9m, Count = 18 },
                new() { Label = "Chất lượng camera", Score = 5m, Count = 18 }
            ],
            Reviews =
            [
                new()
                {
                    Author = "Nguyễn Công",
                    Initial = "N",
                    Rating = 5m,
                    RatingText = "Tuyệt vời",
                    Tags = ["Hiệu năng Siêu mạnh mẽ", "Thời lượng pin Khủng", "Chất lượng camera Chụp đẹp, chuyên nghiệp"],
                    Content = "Mua con này vì: AI - Xoá ảnh thực sự ngon, máy mấy đời cũ thì xoá được, nhưng điểm ảnh vẫn còn một chút rõ. Camera chụp công trình ngon, thiếu sáng vẫn xử lý ổn, đôi lúc bấm 5x để chụp thông số thì AI tự vẽ cho nét thêm. Máy mạnh, mượt và ít nóng hơn đời trước.",
                    TimeAgo = "Đánh giá đã đăng vào 1 tuần trước"
                },
                new()
                {
                    Author = "Anh Bảo",
                    Initial = "A",
                    Rating = 5m,
                    RatingText = "Tuyệt vời",
                    Content = "Đã mua hàng chất lượng dịch vụ tốt, máy đẹp và giao nhanh.",
                    TimeAgo = "Đánh giá đã đăng vào 3 tuần trước"
                },
                new()
                {
                    Author = "Tiến Văn",
                    Initial = "T",
                    Rating = 5m,
                    RatingText = "Tuyệt vời",
                    Tags = ["Đã mua tại CellphoneS", "Hiệu năng Siêu mạnh mẽ", "Thời lượng pin Cực khủng"],
                    Content = "Máy dùng mượt, màn hình sáng và pin rất ổn. Tư vấn tại cửa hàng nhiệt tình.",
                    TimeAgo = "Đánh giá đã đăng vào 1 tháng trước"
                }
            ]
        };
    }

    private static QuestionAnswerSectionViewModel CreateQuestionAnswerSection(string activeStorage)
    {
        return new QuestionAnswerSectionViewModel
        {
            Title = "Hỏi và đáp",
            FormTitle = "Hãy đặt câu hỏi cho chúng tôi",
            Description = "TechStore sẽ phản hồi trong vòng 1 giờ. Nếu Quý khách gửi câu hỏi sau 22h, chúng tôi sẽ trả lời vào sáng hôm sau. Thông tin có thể thay đổi theo thời gian, vui lòng đặt câu hỏi để nhận được cập nhật mới nhất.",
            Placeholder = "Viết câu hỏi của bạn tại đây",
            SubmitLabel = "Gửi câu hỏi",
            AdditionalCommentCount = 305,
            Threads =
            [
                QuestionThread(
                    "Trương Minh Hòa",
                    "T",
                    "2 ngày trước",
                    "20 tuổi trả góp điện thoại được không ạ",
                    "Chào anh Hòa,\nDạ có thể trả góp được ạ.\nSản phẩm iPhone 17 Pro Max " + activeStorage + " | Chính hãng đang có giá khuyến mãi hiện tại 36.990.000đ. Sản phẩm này đang có ưu đãi trả góp 0%, trả trước linh hoạt theo từng công ty tài chính hoặc thẻ tín dụng.\nCảm ơn anh đã quan tâm đến TechStore."),
                QuestionThread(
                    "Nguyễn Ngọc Kim Thư",
                    "N",
                    "1 tuần trước",
                    "có bản ip 17 prm esim kh ạ",
                    "Xin chào chị Nguyễn Ngọc Kim Thư ạ,\nDạ hiện tại iPhone 17 Pro Max hỗ trợ eSIM và nano SIM tùy phiên bản. Chị quan tâm sản phẩm bản bao nhiêu GB và màu nào cụ thể ạ? Dạ không biết mình ở khu vực tỉnh/thành nào để em kiểm tra giữ hàng và giá trong 24h cho mình nhé."),
                QuestionThread(
                    "Phạm Quốc Bảo",
                    "B",
                    "2 tuần trước",
                    "Màu Trắng Classic còn hàng ở Hồ Chí Minh không shop?",
                    "Chào anh Bảo,\nDạ màu Trắng Classic hiện còn hàng tại một số cửa hàng khu vực Hồ Chí Minh. Anh có thể đặt giữ hàng online hoặc để lại khu vực gần nhất, bên em sẽ kiểm tra cửa hàng thuận tiện nhất cho mình ạ."),
                QuestionThread(
                    "Lê Hoài An",
                    "A",
                    "3 tuần trước",
                    "Máy có hỗ trợ sạc nhanh và tặng kèm củ sạc không?",
                    "Chào anh/chị,\nSản phẩm hỗ trợ sạc nhanh qua USB-C. Bộ sản phẩm theo tiêu chuẩn hãng chưa bao gồm củ sạc, mình có thể mua kèm củ sạc chính hãng tại cửa hàng để được tư vấn công suất phù hợp ạ.")
            ]
        };
    }

    private static ProductTechnicalSpecSectionViewModel Section(
        string id,
        string title,
        IReadOnlyList<ProductTechnicalSpecRowViewModel> rows)
    {
        return new ProductTechnicalSpecSectionViewModel
        {
            Id = id,
            Title = title,
            Rows = rows
        };
    }

    private static ProductTechnicalSpecRowViewModel Row(string label, string value, bool isHighlighted = false)
    {
        return new ProductTechnicalSpecRowViewModel
        {
            Label = label,
            Value = value,
            IsHighlighted = isHighlighted
        };
    }

    private static ProductRelatedProductViewModel Related(
        string name,
        string imageUrl,
        string imageAlt,
        decimal currentPrice,
        decimal? oldPrice,
        string? discountLabel,
        string? giftNote,
        decimal? rating = null,
        string? usedPriceLabel = null,
        string? savingLabel = null,
        string? url = null)
    {
        return new ProductRelatedProductViewModel
        {
            Url = url ?? $"/product/{SlugifyProductName(name)}",
            Name = name,
            ImageUrl = imageUrl,
            ImageAlt = imageAlt,
            CurrentPrice = currentPrice,
            OldPrice = oldPrice,
            DiscountLabel = discountLabel,
            InstallmentLabel = "Trả góp 0%",
            GiftNote = giftNote,
            DeliveryLabel = "2 Giờ",
            Location = "Hồ Chí Minh",
            Rating = rating,
            UsedPriceLabel = usedPriceLabel,
            SavingLabel = savingLabel
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
                new()
                {
                    Author = "Quản Trị Viên",
                    Badge = "QTV",
                    TimeAgo = timeAgo,
                    Content = reply
                }
            ]
        };
    }

    private static ProductCardViewModel? FindMockProduct(string slug)
    {
        return GetAllMockProducts()
            .FirstOrDefault(product => string.Equals(product.Id, slug, StringComparison.OrdinalIgnoreCase));
    }

    private static IReadOnlyList<ProductCardViewModel> GetAllMockProducts()
    {
        return AllMockProducts.Value;
    }

    private static IReadOnlyList<ProductCardViewModel> CreateAllMockProducts()
    {
        var homeSections = new[]
        {
            PhoneCategorySectionFactory.Create(),
            ComputerCategorySectionFactory.Create(),
            AudioWearablesCategorySectionFactory.Create()
        };

        var homeProducts = homeSections
            .SelectMany(section => section.Tabs)
            .Where(tab => tab.Panel is not null)
            .SelectMany(tab => tab.Panel!.Products);
        var categoryProducts = MockCategoryPageViewModelFactory.CreatePhoneProducts()
            .Concat(MockCategoryPageViewModelFactory
                .CreateAudioSections(new CategoryPageRequest("audio"))
                .SelectMany(section => section.Products));
        return homeProducts
            .Concat(categoryProducts)
            .DistinctBy(product => product.Id, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static IReadOnlyList<ProductTechnicalSpecSectionViewModel> CreateGenericTechnicalSpecs(
        string slug,
        string categoryLabel,
        string brand)
    {
        return
        [
            Section("thong-tin-chung", "Thông tin chung",
            [
                Row("Mã sản phẩm", slug, isHighlighted: true),
                Row("Nhóm sản phẩm", categoryLabel, isHighlighted: true),
                Row("Thương hiệu", brand),
                Row("Tình trạng", "Mới, chính hãng"),
                Row("Bảo hành", "Theo chính sách của nhà sản xuất")
            ]),
            Section("mua-hang", "Thông tin mua hàng",
            [
                Row("Giao hàng", "Giao nhanh tùy khu vực", isHighlighted: true),
                Row("Thanh toán", "Tiền mặt, chuyển khoản hoặc trả góp"),
                Row("Đổi trả", "Áp dụng theo chính sách TechStore")
            ])
        ];
    }

    private static IReadOnlyList<ProductRelatedProductGroupViewModel> CreateGenericRelatedProductGroups(
        string slug,
        string categorySlug)
    {
        var products = GetAllMockProducts()
            .Where(product => !string.Equals(product.Id, slug, StringComparison.OrdinalIgnoreCase))
            .Where(product =>
            {
                var category = ResolveProductCategory(product.Id, product.ImageUrl);
                return string.Equals(category.Slug, categorySlug, StringComparison.OrdinalIgnoreCase);
            })
            .Take(5)
            .Select(product => Related(
                product.Name,
                product.ImageUrl,
                product.ImageAlt,
                ParsePrice(product.CurrentPrice) ?? 0m,
                ParsePrice(product.OldPrice),
                product.DiscountLabel,
                product.PromotionNote,
                product.Rating,
                url: product.Url))
            .ToList();

        return
        [
            new()
            {
                Id = "similar",
                Label = "Sản phẩm tương tự",
                IsActive = true,
                Products = products
            }
        ];
    }

    private static ProductReviewSummaryViewModel CreateGenericReviewSummary(string productName)
    {
        return new ProductReviewSummaryViewModel
        {
            Title = $"Đánh giá {productName}",
            Score = 4.8m,
            TotalReviews = 12,
            RatingBreakdown =
            [
                new() { Stars = 5, Count = 10, Percent = 83 },
                new() { Stars = 4, Count = 2, Percent = 17 },
                new() { Stars = 3, Count = 0, Percent = 0 },
                new() { Stars = 2, Count = 0, Percent = 0 },
                new() { Stars = 1, Count = 0, Percent = 0 }
            ],
            ExperienceRatings =
            [
                new() { Label = "Chất lượng sản phẩm", Score = 4.8m, Count = 12 },
                new() { Label = "Thiết kế", Score = 4.7m, Count = 10 },
                new() { Label = "Giá trị sử dụng", Score = 4.8m, Count = 11 }
            ],
            Reviews =
            [
                new()
                {
                    Author = "Minh Anh",
                    Initial = "M",
                    Rating = 5m,
                    RatingText = "Rất hài lòng",
                    Content = "Sản phẩm đúng mô tả, đóng gói cẩn thận và giao hàng nhanh.",
                    TimeAgo = "Đánh giá đã đăng vào 1 tuần trước"
                },
                new()
                {
                    Author = "Quốc Bảo",
                    Initial = "B",
                    Rating = 4.5m,
                    RatingText = "Tốt",
                    Content = "Trải nghiệm sử dụng ổn định, nhân viên tư vấn rõ ràng.",
                    TimeAgo = "Đánh giá đã đăng vào 3 tuần trước"
                }
            ]
        };
    }

    private static QuestionAnswerSectionViewModel CreateGenericQuestionAnswer(string productName)
    {
        return new QuestionAnswerSectionViewModel
        {
            Title = "Hỏi và đáp",
            FormTitle = "Hãy đặt câu hỏi cho chúng tôi",
            Description = "TechStore sẽ phản hồi thông tin về giá, tồn kho, bảo hành và giao hàng trong thời gian sớm nhất.",
            Placeholder = "Viết câu hỏi của bạn tại đây",
            SubmitLabel = "Gửi câu hỏi",
            VisibleThreadLimit = 2,
            Threads =
            [
                QuestionThread(
                    "Thanh Tùng",
                    "T",
                    "2 ngày trước",
                    "Sản phẩm này có hỗ trợ trả góp không?",
                    $"{productName} có thể áp dụng trả góp tùy phương thức thanh toán và chương trình tại thời điểm đặt hàng."),
                QuestionThread(
                    "Ngọc Linh",
                    "L",
                    "1 tuần trước",
                    "Mua online có được bảo hành chính hãng không?",
                    "Chào bạn, sản phẩm chính hãng được áp dụng chính sách bảo hành theo nhà sản xuất và thông tin hiển thị trên đơn hàng.")
            ]
        };
    }

    private static (string Slug, string Label) ResolveProductCategory(string slug, string imageUrl)
    {
        var source = $"{slug} {imageUrl}".ToLowerInvariant();

        if (source.Contains("laptop", StringComparison.Ordinal) || source.Contains("macbook", StringComparison.Ordinal))
        {
            return ("laptop", "Laptop");
        }

        if (source.Contains("monitor", StringComparison.Ordinal))
        {
            return ("monitor", "Màn hình máy tính");
        }

        if (source.Contains("desktop", StringComparison.Ordinal) || source.Contains("pc-", StringComparison.Ordinal))
        {
            return ("desktop", "PC và máy tính để bàn");
        }

        if (source.Contains("component", StringComparison.Ordinal)
            || source.Contains("ssd", StringComparison.Ordinal)
            || source.Contains("ram-", StringComparison.Ordinal)
            || source.Contains("psu", StringComparison.Ordinal))
        {
            return ("computer-accessories", "Linh kiện máy tính");
        }

        if (source.Contains("watch", StringComparison.Ordinal) || source.Contains("smartwatch", StringComparison.Ordinal))
        {
            return ("smartwatch", "Đồng hồ thông minh");
        }

        if (source.Contains("audio", StringComparison.Ordinal)
            || source.Contains("headphone", StringComparison.Ordinal)
            || source.Contains("earbuds", StringComparison.Ordinal)
            || source.Contains("airpods", StringComparison.Ordinal)
            || source.Contains("speaker", StringComparison.Ordinal)
            || source.Contains("mic-", StringComparison.Ordinal))
        {
            return ("audio", "Thiết bị âm thanh");
        }

        if (source.Contains("accessor", StringComparison.Ordinal)
            || source.Contains("/categories/", StringComparison.Ordinal)
            || source.Contains("charging", StringComparison.Ordinal))
        {
            return ("accessories", "Phụ kiện");
        }

        if (source.Contains("ipad", StringComparison.Ordinal) || source.Contains("tablet", StringComparison.Ordinal))
        {
            return ("tablet", "Máy tính bảng");
        }

        return ("phone", "Điện thoại");
    }

    private static string ResolveBrand(string productName)
    {
        var normalizedName = productName.Trim();

        if (normalizedName.StartsWith("iPhone", StringComparison.OrdinalIgnoreCase)
            || normalizedName.StartsWith("iPad", StringComparison.OrdinalIgnoreCase)
            || normalizedName.StartsWith("MacBook", StringComparison.OrdinalIgnoreCase)
            || normalizedName.StartsWith("AirPods", StringComparison.OrdinalIgnoreCase))
        {
            return "Apple";
        }

        var firstWord = normalizedName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return string.IsNullOrWhiteSpace(firstWord) ? "TechStore" : firstWord;
    }

    private static string ResolveFallbackImage(string slug)
    {
        var normalizedSlug = slug.ToLowerInvariant();

        if (normalizedSlug.Contains("laptop", StringComparison.Ordinal)
            || normalizedSlug.Contains("macbook", StringComparison.Ordinal))
        {
            return "/images/products/computing/laptop-08.webp";
        }

        if (normalizedSlug.Contains("monitor", StringComparison.Ordinal))
        {
            return "/images/products/computing/monitor-01.webp";
        }

        if (normalizedSlug.Contains("desktop", StringComparison.Ordinal)
            || normalizedSlug.StartsWith("pc-", StringComparison.Ordinal))
        {
            return "/images/products/computing/desktop-01.webp";
        }

        if (normalizedSlug.Contains("ssd", StringComparison.Ordinal)
            || normalizedSlug.Contains("ram", StringComparison.Ordinal)
            || normalizedSlug.Contains("usb", StringComparison.Ordinal)
            || normalizedSlug.Contains("psu", StringComparison.Ordinal))
        {
            return "/images/products/computing/component-01.webp";
        }

        if (normalizedSlug.Contains("watch", StringComparison.Ordinal))
        {
            return "/images/products/audio-wearables/watch-01.webp";
        }

        if (normalizedSlug.Contains("speaker", StringComparison.Ordinal)
            || normalizedSlug.Contains("jbl", StringComparison.Ordinal)
            || normalizedSlug.Contains("marshall", StringComparison.Ordinal))
        {
            return "/images/products/audio-wearables/audio-02.webp";
        }

        if (normalizedSlug.Contains("audio", StringComparison.Ordinal)
            || normalizedSlug.Contains("headphone", StringComparison.Ordinal)
            || normalizedSlug.Contains("earbuds", StringComparison.Ordinal)
            || normalizedSlug.Contains("airpods", StringComparison.Ordinal)
            || normalizedSlug.Contains("buds", StringComparison.Ordinal)
            || normalizedSlug.Contains("mic", StringComparison.Ordinal))
        {
            return "/images/products/audio-wearables/audio-01.webp";
        }

        if (normalizedSlug.Contains("accessory", StringComparison.Ordinal)
            || normalizedSlug.Contains("charge", StringComparison.Ordinal)
            || normalizedSlug.Contains("cable", StringComparison.Ordinal))
        {
            return "/images/categories/accessories/charging-cables.webp";
        }

        return $"{PhoneImageRoot}/phone-orange-cutout.png";
    }

    private static decimal ResolveFallbackPrice(string slug)
    {
        var category = ResolveProductCategory(slug, ResolveFallbackImage(slug));

        return category.Slug switch
        {
            "accessories" => 199_000m,
            "audio" => 1_990_000m,
            "smartwatch" => 4_990_000m,
            "computer-accessories" => 2_990_000m,
            "monitor" => 4_990_000m,
            "laptop" => 19_990_000m,
            "desktop" => 14_990_000m,
            "tablet" => 12_990_000m,
            _ => 9_990_000m
        };
    }

    private static decimal? ParsePrice(string? price)
    {
        if (string.IsNullOrWhiteSpace(price))
        {
            return null;
        }

        var digits = new string(price.Where(char.IsDigit).ToArray());
        return decimal.TryParse(digits, out var value) ? value : null;
    }

    private static string HumanizeProductSlug(string slug)
    {
        var words = slug.Split('-', StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0)
        {
            return "Sản phẩm TechStore";
        }

        return string.Join(' ', words.Select(word =>
            word.Length <= 3 && word.All(char.IsLetterOrDigit)
                ? word.ToUpperInvariant()
                : char.ToUpperInvariant(word[0]) + word[1..]));
    }

    private static string SlugifyProductName(string value)
    {
        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            var normalizedCharacter = character == 'đ' ? 'd' : character;
            if (char.IsLetterOrDigit(normalizedCharacter))
            {
                builder.Append(normalizedCharacter);
            }
            else if (builder.Length > 0 && builder[^1] != '-')
            {
                builder.Append('-');
            }
        }

        return builder.ToString().Trim('-');
    }

    private static string ResolveActiveStorage(string slug)
    {
        if (slug.Contains("2tb", StringComparison.Ordinal))
        {
            return "2TB";
        }

        if (slug.Contains("1tb", StringComparison.Ordinal))
        {
            return "1TB";
        }

        if (slug.Contains("512gb", StringComparison.Ordinal))
        {
            return "512GB";
        }

        return "256GB";
    }
}
