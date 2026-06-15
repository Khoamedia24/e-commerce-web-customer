using e_commerce_web_customer.Infrastructure.Home.Content;
using e_commerce_web_customer.ViewModels.Home;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Home.Mock;

internal static class ComputerCategorySectionFactory
{
    private const string ImageRoot = "/images/products/computing";

    public static CategoryProductsViewModel Create()
    {
        return new CategoryProductsViewModel
        {
            Id = "computer-products",
            Rows = 2,
            EnableTabSwitching = true,
            ShowPagination = false,
            Tabs =
            [
                Tab("laptops", "Laptop", "/catalog?cat=laptop", CreateLaptopPanel(), true),
                Tab("monitors", "Màn hình máy tính", "/catalog?cat=monitor", CreateMonitorPanel()),
                Tab("desktop-pcs", "PC", "/catalog?cat=desktop", CreateDesktopPanel()),
                Tab("computer-accessories", "Phụ kiện máy tính", "/catalog?cat=computer-accessories", CreateComponentPanel())
            ]
        };
    }

    private static CategoryProductPanelViewModel CreateLaptopPanel()
    {
        return Panel(
            "/catalog?cat=laptop",
            [
                QuickLink("Văn phòng", "laptop", "usage=office", "laptop-03.webp"),
                QuickLink("Gaming", "laptop", "usage=gaming", "laptop-01.webp"),
                QuickLink("Mỏng nhẹ", "laptop", "usage=thin-light", "laptop-05.webp"),
                QuickLink("Đồ họa - kỹ thuật", "laptop", "usage=creator", "laptop-09.webp"),
                QuickLink("Sinh viên", "laptop", "usage=student", "laptop-10.webp"),
                QuickLink("Cảm ứng", "laptop", "feature=touch", "laptop-02.webp")
            ],
            Brands("laptop", "MacBook", "ASUS", "Lenovo", "MSI", "Acer", "HP", "Dell", "LG", "Gigabyte", "Masstel"),
            HomeProductCardFactory.AddVariants(
            [
                Product("laptop-msi-prestige-13", "Laptop MSI Prestige 13 AI+ Ukiyoe Edition A2VMG", "laptop-01.webp",
                    "51.990.000đ", "54.990.000đ", "Giảm 5%", "Smember giảm đến 520.000đ",
                    "Tặng phiếu mua hàng và phần mềm bản quyền.", "S-Student giảm thêm 1.000.000đ"),
                Product("laptop-hp-omnibook-x-flip", "Laptop HP OmniBook X Flip 14-FK0092AU", "laptop-02.webp",
                    "30.690.000đ", "33.990.000đ", "Giảm 10%", "Smember giảm đến 307.000đ",
                    "Nâng cấp laptop, PC lên Windows 11 Pro.", "S-Student giảm thêm 1.000.000đ"),
                Product("laptop-hp-omnibook-5", "Laptop HP OmniBook 5 AI 16-AF1048TU", "laptop-03.webp",
                    "26.790.000đ", "27.190.000đ", "Giảm 1%", "Smember giảm đến 268.000đ",
                    "Giảm thêm 10% cho phụ kiện văn phòng.", "S-Student giảm thêm 1.000.000đ"),
                Product("laptop-acer-gaming-aspire-7", "Laptop Acer Gaming Aspire 7 A715-59G", "laptop-04.webp",
                    "21.490.000đ", "23.990.000đ", "Giảm 10%", "Smember giảm đến 215.000đ",
                    "Tặng balo và chuột gaming chính hãng.", "S-Student giảm thêm 1.000.000đ"),
                Product("laptop-asus-vivobook-14", "Laptop ASUS Vivobook 14 M1405NAQ", "laptop-05.webp",
                    "17.990.000đ", "20.990.000đ", "Giảm 13%", "Smember giảm đến 180.000đ",
                    "Tặng phiếu mua hàng và bảo hành mở rộng.", "S-Student giảm thêm 899.500đ"),
                Product("laptop-hp-15-fd", "Laptop HP 15-FD1289TU Core Ultra 5", "laptop-06.webp",
                    "24.290.000đ", "24.990.000đ", "Giảm 3%", "Smember giảm đến 243.000đ",
                    "Nâng cấp Windows 11 Pro chỉ với 990.000đ.", "S-Student giảm thêm 1.000.000đ"),
                Product("macbook-neo-13", "MacBook Neo 13 inch chip A18 Pro 512GB", "laptop-07.webp",
                    "18.490.000đ", "18.990.000đ", "Giảm 3%", "Smember giảm đến 185.000đ",
                    "Trả góp 0%, không phụ phí, trả trước từ 0đ.", "S-Student giảm thêm 500.000đ"),
                Product("macbook-air-m5-13", "MacBook Air M5 13 inch 2026 16GB 512GB", "laptop-08.webp",
                    "28.990.000đ", "29.990.000đ", "Giảm 3%", "Smember giảm đến 290.000đ",
                    "Trả góp 0% qua thẻ tín dụng.", "S-Student giảm thêm 500.000đ"),
                Product("laptop-lenovo-yoga-creator", "Laptop Lenovo Yoga Creator 14 OLED", "laptop-09.webp",
                    "32.490.000đ", "35.990.000đ", "Giảm 10%", "Smember giảm đến 325.000đ",
                    "Tặng bộ phần mềm sáng tạo bản quyền.", "S-Student giảm thêm 1.000.000đ"),
                Product("laptop-dell-inspiron-14", "Laptop Dell Inspiron 14 Plus 7440", "laptop-10.webp",
                    "22.990.000đ", "24.490.000đ", "Giảm 6%", "Smember giảm đến 230.000đ",
                    "Tặng balo chống sốc và chuột không dây.", "S-Student giảm thêm 700.000đ")
            ],
            [
                new(0, "laptop-msi-prestige-13-32gb", "Laptop MSI Prestige 13 AI+ 32GB 1TB"),
                new(1, "laptop-hp-omnibook-x-flip-1tb", "Laptop HP OmniBook X Flip 14 1TB"),
                new(2, "laptop-hp-omnibook-5-ultra-7", "Laptop HP OmniBook 5 AI Core Ultra 7"),
                new(3, "laptop-acer-gaming-aspire-7-32gb", "Laptop Acer Gaming Aspire 7 32GB"),
                new(4, "laptop-asus-vivobook-14-oled", "Laptop ASUS Vivobook 14 OLED")
            ]));
    }

    private static CategoryProductPanelViewModel CreateMonitorPanel()
    {
        return Panel(
            "/catalog?cat=monitor",
            [
                QuickLink("Gaming", "monitor", "usage=gaming", "monitor-01.webp"),
                QuickLink("Văn phòng", "monitor", "usage=office", "monitor-02.webp"),
                QuickLink("Đồ họa", "monitor", "usage=graphics", "monitor-07.webp"),
                QuickLink("Màn hình cong", "monitor", "design=curved", "monitor-03.webp"),
                QuickLink("Màn hình lập trình", "monitor", "usage=coding", "monitor-09.webp"),
                QuickLink("Màn hình di động", "monitor", "design=portable", "monitor-04.webp")
            ],
            Brands("monitor", "ASUS", "LG", "Samsung", "MSI", "Xiaomi", "Dell", "AOC", "Acer", "Philips", "ViewSonic", "Lenovo"),
            HomeProductCardFactory.AddVariants(
            [
                Product("monitor-asus-tuf-vg27aq5a", "Màn hình Gaming ASUS TUF VG27AQ5A 27 inch", "monitor-01.webp",
                    "4.990.000đ", "5.390.000đ", "Giảm 6%", "Smember giảm đến 100.000đ",
                    "Tần số quét cao, hỗ trợ đồng bộ khung hình.", "S-Student giảm thêm 249.500đ"),
                Product("monitor-xiaomi-a27qi", "Màn hình Xiaomi A27QI 2026 27 inch", "monitor-02.webp",
                    "3.690.000đ", "4.500.000đ", "Giảm 18%", "Smember giảm đến 100.000đ",
                    "Độ phân giải 2K, màu sắc chuẩn cho văn phòng.", "S-Student giảm thêm 184.500đ"),
                Product("monitor-lg-24u411a", "Màn hình LG 24U411A-B 24 inch 120Hz", "monitor-03.webp",
                    "2.290.000đ", "2.990.000đ", "Giảm 24%", "Smember giảm đến 100.000đ",
                    "Thiết kế gọn, phù hợp học tập và làm việc.", "S-Student giảm thêm 114.500đ"),
                Product("monitor-msi-pro-mp165", "Màn hình di động MSI Pro MP165 E6 16 inch", "monitor-04.webp",
                    "2.590.000đ", "3.090.000đ", "Giảm 16%", "Smember giảm đến 52.000đ",
                    "Màn hình phụ linh hoạt cho laptop và máy chơi game.", "S-Student giảm thêm 129.500đ"),
                Product("monitor-msi-mag-245f", "Màn hình Gaming MSI MAG 245F X24 24 inch", "monitor-05.webp",
                    "2.990.000đ", "3.490.000đ", "Giảm 14%", "Smember giảm đến 60.000đ",
                    "Tần số quét 240Hz cho chuyển động mượt mà.", "S-Student giảm thêm 149.500đ"),
                Product("monitor-asus-tuf-vg259q5a", "Màn hình Gaming ASUS TUF VG259Q5A 25 inch", "monitor-06.webp",
                    "2.790.000đ", "3.290.000đ", "Giảm 15%", "Smember giảm đến 56.000đ",
                    "Tối ưu trải nghiệm game tốc độ cao.", "S-Student giảm thêm 139.500đ"),
                Product("monitor-vsp-pg1614ws1", "Màn hình di động VSP PG1614WS1 16 inch", "monitor-07.webp",
                    "3.090.000đ", "3.490.000đ", "Giảm 11%", "Smember giảm đến 62.000đ",
                    "Thiết kế mỏng nhẹ, kết nối USB-C tiện lợi.", "S-Student giảm thêm 154.500đ"),
                Product("monitor-msi-pro-mp242l", "Màn hình MSI Pro MP242L 24 inch", "monitor-08.webp",
                    "1.890.000đ", "2.490.000đ", "Giảm 24%", "Smember giảm đến 38.000đ",
                    "Tấm nền chống chói, bảo vệ mắt khi làm việc.", "S-Student giảm thêm 94.500đ"),
                Product("monitor-dell-ultrasharp-u2725", "Màn hình Dell UltraSharp U2725 27 inch", "monitor-09.webp",
                    "12.490.000đ", "13.990.000đ", "Giảm 11%", "Smember giảm đến 250.000đ",
                    "Màu sắc chuyên nghiệp dành cho nhà sáng tạo.", "S-Student giảm thêm 500.000đ"),
                Product("monitor-lg-myview-27", "Màn hình thông minh LG MyView 27 inch", "monitor-10.webp",
                    "6.490.000đ", "7.290.000đ", "Giảm 11%", "Smember giảm đến 130.000đ",
                    "Giải trí trực tuyến không cần kết nối máy tính.", "S-Student giảm thêm 300.000đ")
            ],
            [
                new(0, "monitor-asus-tuf-vg27aq5a-2k", "Màn hình ASUS TUF VG27AQ5A 2K 180Hz"),
                new(1, "monitor-xiaomi-a27qi-usbc", "Màn hình Xiaomi A27QI USB-C 27 inch"),
                new(2, "monitor-lg-24u411a-white", "Màn hình LG 24U411A-B phiên bản trắng"),
                new(3, "monitor-msi-pro-mp165-touch", "Màn hình di động MSI Pro MP165 cảm ứng"),
                new(4, "monitor-msi-mag-245f-240hz", "Màn hình MSI MAG 245F 240Hz")
            ]));
    }

    private static CategoryProductPanelViewModel CreateDesktopPanel()
    {
        return Panel(
            "/catalog?cat=desktop",
            [
                QuickLink("Build PC", "desktop", "type=build", "desktop-01.webp"),
                QuickLink("PC ráp sẵn", "desktop", "type=prebuilt", "desktop-02.webp"),
                QuickLink("Máy tính All In One", "desktop", "type=all-in-one", "desktop-10.webp"),
                QuickLink("Máy tính đồng bộ", "desktop", "type=office", "desktop-04.webp"),
                QuickLink("Linh kiện máy tính", "computer-accessories", "type=components", "desktop-06.webp")
            ],
            Brands("desktop", "Build PC", "PC Gaming", "PC đồ họa", "PC văn phòng", "PC đồng bộ"),
            HomeProductCardFactory.AddVariants(
            [
                Product("pc-cps-gaming-g02", "PC CPS Gaming G02 i3 12100F RTX 3050", "desktop-01.webp",
                    "13.690.000đ", "16.990.000đ", "Giảm 37%", "Smember giảm đến 274.000đ",
                    "Ưu đãi mua kèm Windows 11 Pro bản quyền.", "S-Student giảm thêm 500.000đ"),
                Product("pc-cps-gaming-g01", "PC CPS Gaming G01 i3 12100F RX 6500 XT", "desktop-02.webp",
                    "11.990.000đ", "20.490.000đ", "Giảm 41%", "Smember giảm đến 240.000đ",
                    "Ưu đãi mua kèm màn hình và bàn phím.", "S-Student giảm thêm 500.000đ"),
                Product("pc-cps-gaming-g13", "PC CPS Gaming G13 i7 12700F RTX 5060", "desktop-03.webp",
                    "25.990.000đ", "38.990.000đ", "Giảm 33%", "Smember giảm đến 520.000đ",
                    "Tặng bộ phụ kiện gaming cao cấp.", "S-Student giảm thêm 500.000đ"),
                Product("pc-cps-office-s01", "PC CPS văn phòng S01 R3 3200G 8GB 256GB", "desktop-04.webp",
                    "6.790.000đ", "10.990.000đ", "Giảm 38%", "Smember giảm đến 136.000đ",
                    "Cài đặt phần mềm văn phòng miễn phí.", "S-Student giảm thêm 339.500đ"),
                Product("pc-cps-white-creator", "PC CPS Creator White i5 RTX 4060", "desktop-05.webp",
                    "29.990.000đ", "35.990.000đ", "Giảm 17%", "Smember giảm đến 600.000đ",
                    "Tặng gói vệ sinh và bảo trì định kỳ.", "S-Student giảm thêm 800.000đ"),
                Product("pc-cps-quantum-blaze", "PC CPS Quantum Blaze i9 RTX 5070 Ti", "desktop-06.webp",
                    "67.990.000đ", "88.790.000đ", "Giảm 23%", "Smember giảm đến 1.360.000đ",
                    "Bảo hành tận nơi và hỗ trợ kỹ thuật ưu tiên.", "S-Student giảm thêm 500.000đ"),
                Product("pc-cps-workstation", "PC CPS Workstation Xeon 32GB RTX", "desktop-07.webp",
                    "45.990.000đ", "52.990.000đ", "Giảm 13%", "Smember giảm đến 920.000đ",
                    "Tối ưu cho dựng phim và thiết kế kỹ thuật.", "S-Student giảm thêm 700.000đ"),
                Product("pc-cps-mini-office", "PC CPS Mini Office i5 16GB 512GB", "desktop-08.webp",
                    "12.490.000đ", "14.990.000đ", "Giảm 17%", "Smember giảm đến 250.000đ",
                    "Thiết kế nhỏ gọn, tiết kiệm không gian.", "S-Student giảm thêm 400.000đ"),
                Product("pc-cps-creator-ai", "PC CPS Creator AI i7 RTX 5070", "desktop-09.webp",
                    "53.990.000đ", "75.390.000đ", "Giảm 28%", "Smember giảm đến 1.080.000đ",
                    "Tặng phần mềm sáng tạo và bản quyền Windows.", "S-Student giảm thêm 500.000đ"),
                Product("all-in-one-cps-27", "Máy tính All In One CPS 27 inch Core i5", "desktop-10.webp",
                    "16.990.000đ", "19.990.000đ", "Giảm 15%", "Smember giảm đến 340.000đ",
                    "Tặng bàn phím và chuột không dây.", "S-Student giảm thêm 500.000đ")
            ],
            [
                new(0, "pc-cps-gaming-g02-32gb", "PC CPS Gaming G02 32GB RTX 3050"),
                new(1, "pc-cps-gaming-g01-1tb", "PC CPS Gaming G01 SSD 1TB"),
                new(2, "pc-cps-gaming-g13-rtx5070", "PC CPS Gaming G13 RTX 5070"),
                new(3, "pc-cps-office-s01-16gb", "PC CPS văn phòng S01 16GB 512GB"),
                new(4, "pc-cps-white-creator-32gb", "PC CPS Creator White 32GB RTX 4060")
            ]));
    }

    private static CategoryProductPanelViewModel CreateComponentPanel()
    {
        return Panel(
            "/catalog?cat=computer-accessories",
            [
                QuickLink("CPU", "computer-accessories", "type=cpu", "component-10.webp"),
                QuickLink("Mainboard", "computer-accessories", "type=mainboard", "component-03.webp"),
                QuickLink("RAM", "computer-accessories", "type=ram", "component-09.webp"),
                QuickLink("Ổ cứng", "computer-accessories", "type=storage", "component-01.webp"),
                QuickLink("Card màn hình", "computer-accessories", "type=gpu", "component-05.webp"),
                QuickLink("Nguồn máy tính", "computer-accessories", "type=psu", "component-10.webp")
            ],
            Brands("computer-accessories", "ASUS", "Intel", "MSI", "Samsung", "Gigabyte", "ASRock"),
            HomeProductCardFactory.AddVariants(
            [
                Product("ssd-adata-sc750-500gb", "Ổ cứng di động SSD ADATA SC750 USB 3.2 500GB", "component-01.webp",
                    "2.690.000đ", "2.990.000đ", "Giảm 10%", "Smember giảm đến 54.000đ",
                    "Giá khuyến mãi áp dụng giới hạn số lượng.", "S-Student giảm thêm 200.000đ"),
                Product("usb-transcend-esd310c-256gb", "Ổ cứng di động SSD Transcend ESD310C 256GB", "component-02.webp",
                    "2.190.000đ", "2.990.000đ", "Giảm 27%", "Smember giảm đến 44.000đ",
                    "Giá khuyến mãi áp dụng giới hạn số lượng.", "S-Student giảm thêm 200.000đ"),
                Product("ssd-kingston-nv3-1tb", "Ổ cứng SSD Kingston NV3 PCIe 4.0 NVMe 1TB", "component-03.webp",
                    "4.490.000đ", "6.090.000đ", "Giảm 26%", "Smember giảm đến 90.000đ",
                    "Sản phẩm không áp dụng bán sỉ.", "S-Student giảm thêm 224.500đ"),
                Product("usb-transcend-esd310c-128gb", "Ổ cứng di động SSD Transcend ESD310C 128GB", "component-04.webp",
                    "1.390.000đ", "1.990.000đ", "Giảm 30%", "Smember giảm đến 28.000đ",
                    "Giá khuyến mãi áp dụng giới hạn số lượng.", "S-Student giảm thêm 139.000đ"),
                Product("ssd-sandisk-e61-1tb", "Ổ cứng di động SanDisk Extreme V2 SSD 1TB", "component-05.webp",
                    "6.990.000đ", "8.990.000đ", "Giảm 22%", "Smember giảm đến 140.000đ",
                    "Chống sốc và kháng nước cho công việc di động.", "S-Student giảm thêm 200.000đ"),
                Product("ram-kingston-sodimm-16gb", "RAM Laptop Kingston SODIMM 16GB 3200MHz", "component-06.webp",
                    "3.990.000đ", "5.990.000đ", "Giảm 33%", "Smember giảm đến 80.000đ",
                    "Sản phẩm không áp dụng bán sỉ.", "S-Student giảm thêm 199.500đ"),
                Product("ssd-kingston-nvme-512gb", "Ổ cứng SSD Kingston NVMe 512GB", "component-07.webp",
                    "2.490.000đ", "3.490.000đ", "Giảm 29%", "Smember giảm đến 50.000đ",
                    "Miễn phí lắp đặt khi mua cùng máy tính.", "S-Student giảm thêm 200.000đ"),
                Product("usb-transcend-pink-512gb", "Ổ cứng di động SSD Transcend USB-C 512GB", "component-08.webp",
                    "2.690.000đ", "4.990.000đ", "Giảm 46%", "Smember giảm đến 54.000đ",
                    "Thiết kế nhỏ gọn, tốc độ truyền dữ liệu cao.", "S-Student giảm thêm 200.000đ"),
                Product("ram-desktop-ddr5-32gb", "RAM Desktop DDR5 32GB Kit 6000MHz", "component-09.webp",
                    "5.490.000đ", "6.990.000đ", "Giảm 21%", "Smember giảm đến 110.000đ",
                    "Tương thích nền tảng Intel và AMD mới.", "S-Student giảm thêm 250.000đ"),
                Product("psu-750w-gold", "Nguồn máy tính 750W chuẩn 80 Plus Gold", "component-10.webp",
                    "2.990.000đ", "3.590.000đ", "Giảm 17%", "Smember giảm đến 60.000đ",
                    "Bảo hành chính hãng, vận hành ổn định.", "S-Student giảm thêm 150.000đ")
            ],
            [
                new(0, "ssd-adata-sc750-1tb", "Ổ cứng di động SSD ADATA SC750 1TB"),
                new(1, "usb-transcend-esd310c-512gb", "Ổ cứng di động Transcend ESD310C 512GB"),
                new(2, "ssd-kingston-nv3-2tb", "Ổ cứng SSD Kingston NV3 NVMe 2TB"),
                new(3, "usb-transcend-esd310c-512gb-black", "SSD Transcend ESD310C 512GB màu đen"),
                new(4, "ssd-sandisk-e61-2tb", "Ổ cứng di động SanDisk Extreme V2 2TB")
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
            "/catalog?cat=laptop" =>
            [
                HomeCategoryBannerFactory.Create(
                    "banner_laptop.png",
                    "Ưu đãi laptop nổi bật",
                    "/catalog?cat=laptop"),
                HomeCategoryBannerFactory.Create(
                    "banner_mac.png",
                    "Ưu đãi MacBook dành cho sinh viên",
                    "/catalog?cat=laptop&brand=apple")
            ],
            "/catalog?cat=monitor" =>
            [
                HomeCategoryBannerFactory.Create(
                    "banner_asus_monitor.png",
                    "Ưu đãi màn hình ASUS",
                    "/catalog?cat=monitor&brand=asus"),
                HomeCategoryBannerFactory.Create(
                    "banner_acer_monitor.png",
                    "Ưu đãi màn hình Acer",
                    "/catalog?cat=monitor&brand=acer")
            ],
            "/catalog?cat=desktop" =>
            [
                HomeCategoryBannerFactory.Create(
                    "banner_pc_acer.png",
                    "Ưu đãi máy tính Acer",
                    "/catalog?cat=desktop&brand=acer"),
                HomeCategoryBannerFactory.Create(
                    "banner_pc_asus.png",
                    "Ưu đãi máy tính ASUS",
                    "/catalog?cat=desktop&brand=asus")
            ],
            "/catalog?cat=computer-accessories" =>
            [
                HomeCategoryBannerFactory.Create(
                    "banner_amd.png",
                    "Linh kiện máy tính AMD nổi bật",
                    "/catalog?cat=computer-accessories&brand=amd"),
                HomeCategoryBannerFactory.Create(
                    "banner_nvidia.png",
                    "Linh kiện máy tính NVIDIA nổi bật",
                    "/catalog?cat=computer-accessories&brand=nvidia")
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
        string note,
        string? studentOffer = null)
    {
        return HomeProductCardFactory.Create(
            id,
            name,
            $"{ImageRoot}/{imageName}",
            price,
            oldPrice,
            discount,
            memberOffer,
            note,
            studentOffer: studentOffer);
    }
}
