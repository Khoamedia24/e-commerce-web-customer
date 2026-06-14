using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Products;
using e_commerce_web_customer.Application.Search;

namespace e_commerce_web_customer.Infrastructure.Products.Mock;

public sealed class MockProductCatalog : IProductCatalog
{
    private const string PhoneImageRoot = "/images/products/phone";
    private const string DefaultPromotion =
        "Không phí chuyển đổi khi trả góp 0% qua thẻ tín dụng kỳ hạn 3-6 tháng";

    private static readonly IReadOnlyList<ProductReadModel> Products =
    [
        Product("samsung-galaxy-s26-ultra-256gb", "Samsung Galaxy S26 Ultra 5G 12GB 256GB", "phone-violet-cutout.png", 29_990_000m, 36_990_000m, 19, 29_490_000m, ["samsung-galaxy-s26-ultra", "galaxy-s26-ultra-5g"]),
        Product("samsung-galaxy-s26-256gb", "Samsung Galaxy S26 5G 12GB 256GB", "phone-camera-cutout.png", 20_990_000m, 25_990_000m, 19, 20_490_000m, ["samsung-galaxy-s26", "galaxy-s26-5g"]),
        Product("samsung-galaxy-a17-5g", "Samsung Galaxy A17 5G 8GB 128GB", "phone-camera.webp", 5_990_000m, 6_390_000m, 6, 5_690_500m, ["samsung-galaxy-a17"]),
        Product("samsung-galaxy-a57-5g", "Samsung Galaxy A57 5G 8GB 128GB", "phone-rose-cutout.png", 10_590_000m, 12_490_000m, 15, 10_090_000m, ["samsung-galaxy-a57", "galaxy-a57-5g"]),
        Product("samsung-galaxy-s24-plus", "Samsung Galaxy S24 Plus 12GB 256GB", "phone-violet.webp", 16_490_000m, 18_650_000m, 12, 15_990_000m),
        Product("samsung-galaxy-a07-5g", "Samsung Galaxy A07 5G 4GB 128GB", "phone-gaming-cutout.png", 4_290_000m, 4_590_000m, 7, 4_075_500m),
        Product("samsung-galaxy-s25-plus", "Samsung Galaxy S25 Plus 256GB", "phone-camera-cutout.png", 19_990_000m, 26_500_000m, 25, 19_490_000m),
        Product("samsung-galaxy-z-flip7", "Samsung Galaxy Z Flip7 12GB 256GB", "phone-violet-cutout.png", 23_590_000m, 28_990_000m, 19, 23_090_000m),
        Product("samsung-galaxy-s25", "Samsung Galaxy S25 256GB", "phone-violet.webp", 17_490_000m, 22_580_000m, 23, 16_990_000m),
        Product("samsung-galaxy-s26-ultra-512gb", "Samsung Galaxy S26 Ultra 5G 12GB 512GB", "phone-violet-cutout.png", 36_090_000m, 42_990_000m, 16, 35_590_000m),
        Product("samsung-galaxy-tab-a11", "Samsung Galaxy Tab A11 Wifi 4GB 64GB", "phone-camera.webp", 3_490_000m, 4_390_000m, 21, 3_210_800m),
        Product("samsung-galaxy-tab-s11", "Samsung Galaxy Tab S11 Wifi 12GB 128GB", "phone-rose-cutout.png", 19_990_000m, 22_490_000m, 11, 19_190_000m),
        Product("samsung-galaxy-s25-ultra-512gb", "Samsung Galaxy S25 Ultra 12GB 512GB", "phone-gaming-cutout.png", 30_990_000m, 37_990_000m, 18, 30_390_000m, ["samsung-galaxy-s25-ultra"]),
        Product("samsung-galaxy-s24-ultra-cu", "Samsung Galaxy S24 Ultra cũ 12GB 256GB", "phone-violet-cutout.png", 19_990_000m, 24_990_000m, 20, 19_590_000m),
        Product("samsung-galaxy-z-fold7", "Samsung Galaxy Z Fold7 12GB 512GB", "phone-camera-cutout.png", 39_990_000m, 46_990_000m, 15, 39_090_000m),
        Product("samsung-galaxy-m56-5g", "Samsung Galaxy M56 5G 8GB 256GB", "phone-rose.webp", 8_990_000m, 10_590_000m, 15, 8_690_000m),
        Product("samsung-galaxy-a36-5g", "Samsung Galaxy A36 5G 8GB 128GB", "phone-camera.webp", 7_790_000m, 8_990_000m, 13, 7_490_000m),
        Product("samsung-galaxy-a56-5g", "Samsung Galaxy A56 5G 12GB 256GB", "phone-violet.webp", 9_990_000m, 11_990_000m, 17, 9_490_000m),
        Product("samsung-galaxy-s24-fe", "Samsung Galaxy S24 FE 5G 8GB 256GB", "phone-rose-cutout.png", 12_490_000m, 16_990_000m, 26, 11_990_000m),
        Product("samsung-galaxy-z-flip6-cu", "Samsung Galaxy Z Flip6 cũ 12GB 256GB", "phone-violet-cutout.png", 14_990_000m, 20_990_000m, 29, 14_490_000m),
        Product("samsung-galaxy-buds-4-pro", "Tai nghe Samsung Galaxy Buds 4 Pro", "/images/products/audio-wearables/audio-06.webp", 5_990_000m, 7_490_000m, 20, 5_690_000m),
        Product("samsung-galaxy-watch-8", "Samsung Galaxy Watch 8 44mm LTE", "/images/products/audio-wearables/watch-02.webp", 7_490_000m, 8_990_000m, 17, 7_190_000m),
        Product("samsung-galaxy-tab-s10-fe", "Samsung Galaxy Tab S10 FE 5G 128GB", "phone-rose.webp", 13_990_000m, 15_990_000m, 12, 13_490_000m, ["galaxy-tab-s10-fe"]),
        Product("samsung-galaxy-tab-s11-ultra", "Samsung Galaxy Tab S11 Ultra 5G 256GB", "phone-violet-cutout.png", 27_990_000m, 31_990_000m, 12, 27_190_000m, ["galaxy-tab-s11-ultra"]),
        Product("samsung-galaxy-a26-5g", "Samsung Galaxy A26 5G 8GB 128GB", "phone-camera-cutout.png", 6_490_000m, 7_990_000m, 19, 6_190_000m),
        Product("samsung-galaxy-s23-ultra-cu", "Samsung Galaxy S23 Ultra cũ 12GB 256GB", "phone-gaming-cutout.png", 16_490_000m, 21_990_000m, 25, 15_990_000m),
        Product("samsung-galaxy-z-fold6-cu", "Samsung Galaxy Z Fold6 cũ 12GB 256GB", "phone-violet.webp", 24_990_000m, 32_990_000m, 24, 24_290_000m),
        Product("samsung-galaxy-s25-edge", "Samsung Galaxy S25 Edge 12GB 256GB", "phone-camera.webp", 22_990_000m, 28_990_000m, 21, 22_490_000m),
        Product("samsung-galaxy-a15-cu", "Samsung Galaxy A15 cũ 8GB 128GB", "phone-rose-cutout.png", 3_490_000m, 4_990_000m, 30, 3_290_000m),
        Product("samsung-galaxy-smarttag-2", "Samsung Galaxy SmartTag 2", "/images/categories/accessories/network-devices.webp", 590_000m, 790_000m, 25, null),
        Product("samsung-galaxy-fit-4", "Vòng đeo tay Samsung Galaxy Fit 4", "/images/products/audio-wearables/watch-05.webp", 1_290_000m, 1_790_000m, 28, 1_190_000m),
        Product("samsung-45w-usb-c", "Củ sạc nhanh Samsung 45W USB-C", "/images/categories/accessories/charging-cables.webp", 690_000m, 990_000m, 30, null),
        Product("samsung-galaxy-a06", "Samsung Galaxy A06 4GB 64GB", "phone-camera.webp", 2_690_000m, 3_190_000m, 16, 2_490_000m),
        Product("samsung-galaxy-s22-ultra-cu", "Samsung Galaxy S22 Ultra cũ 12GB 256GB", "phone-violet-cutout.png", 12_990_000m, 17_990_000m, 28, 12_490_000m),
        Product("samsung-galaxy-tab-a9-plus", "Samsung Galaxy Tab A9 Plus Wifi 8GB 128GB", "phone-rose.webp", 5_990_000m, 7_990_000m, 25, 5_690_000m),
        Product("iphone-17-pro-max-256gb", "iPhone 17 Pro Max 256GB | Chính hãng", "phone-orange-cutout.png", 36_990_000m, 37_990_000m, 3, null, ["iphone-17-pro-max"]),
        Product("iphone-17-256gb", "iPhone 17 256GB | Chính hãng", "phone-rose-cutout.png", 23_990_000m, 24_990_000m, 4, null, ["iphone-17"]),
        Product("oppo-find-x9-ultra", "OPPO Find X9 Ultra 12GB 512GB", "phone-camera.webp", 48_990_000m, 49_990_000m, 2, null),
        Product("macbook-air-m5", "MacBook Air M5 13 inch 2026", "/images/products/computing/laptop-10.webp", 28_990_000m, 39_990_000m, 28, null),
        Product("sony-wh-1000xm6", "Tai nghe Sony không dây WH-1000XM6", "/images/products/audio-wearables/audio-01.webp", 7_490_000m, 7_990_000m, 6, null),
        Product("headphone-wireless-pro", "Tai nghe Bluetooth chụp tai Wireless Pro", "/images/products/audio-wearables/audio-01.webp", 2_790_000m, 3_290_000m, 15, null)
    ];

    private static readonly IReadOnlyDictionary<string, ProductReadModel> ProductsById =
        BuildProductIndex();

    public Task<IReadOnlyList<ProductReadModel>> SearchAsync(
        string? query,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var normalizedQuery = SearchTextNormalizer.Normalize(query ?? string.Empty);
        IReadOnlyList<ProductReadModel> result = string.IsNullOrWhiteSpace(normalizedQuery)
            ? Products
            : Products
                .Where(product => SearchTextNormalizer.Normalize(product.SearchText)
                    .Contains(normalizedQuery, StringComparison.Ordinal))
                .ToList();

        return Task.FromResult(result);
    }

    public Task<ProductReadModel?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ProductsById.TryGetValue(id.Trim(), out var product);
        return Task.FromResult(product);
    }

    private static ProductReadModel Product(
        string id,
        string name,
        string imageName,
        decimal currentPrice,
        decimal? oldPrice,
        int discountPercent,
        decimal? studentPrice,
        IReadOnlyList<string>? aliases = null)
    {
        var imageUrl = imageName.StartsWith("/", StringComparison.Ordinal)
            ? imageName
            : $"{PhoneImageRoot}/{imageName}";

        return new ProductReadModel(
            id,
            name,
            $"/product/{id}",
            imageUrl,
            name,
            currentPrice,
            oldPrice,
            discountPercent,
            studentPrice,
            DefaultPromotion,
            $"{id} {name} điện thoại máy tính bảng phụ kiện cũ giá rẻ",
            aliases);
    }

    private static IReadOnlyDictionary<string, ProductReadModel> BuildProductIndex()
    {
        var index = new Dictionary<string, ProductReadModel>(StringComparer.OrdinalIgnoreCase);

        foreach (var product in Products)
        {
            index[product.Id] = product;
            foreach (var alias in product.Aliases ?? [])
            {
                index[alias] = product;
            }
        }

        return index;
    }

}
