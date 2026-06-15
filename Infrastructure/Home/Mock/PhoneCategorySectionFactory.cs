using e_commerce_web_customer.Infrastructure.Home.Content;
using e_commerce_web_customer.ViewModels.Home;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Home.Mock;

internal static class PhoneCategorySectionFactory
{
    public static CategoryProductsViewModel Create()
    {
        const string imageRoot = "/images/products/phone";

        return new CategoryProductsViewModel
        {
            Id = "phone-products",
            Rows = 2,
            EnableTabSwitching = true,
            ShowPagination = false,
            Tabs =
            [
                new()
                {
                    Id = "phones",
                    Label = "Điện thoại",
                    Url = "/catalog?cat=phone",
                    IsActive = true,
                    Panel = new CategoryProductPanelViewModel
                    {
                        ViewAllUrl = "/catalog?cat=phone",
                        Banners =
                        [
                            HomeCategoryBannerFactory.Create(
                                "banner_iphone.png",
                                "Ưu đãi iPhone nổi bật",
                                "/catalog?cat=phone&brand=apple"),
                            HomeCategoryBannerFactory.Create(
                                "banner_samsung_phone.png",
                                "Ưu đãi điện thoại Samsung",
                                "/catalog?cat=phone&brand=samsung")
                        ],
                        QuickLinks =
                        [
                            QuickLink("Điện thoại chơi game", "usage=gaming", "phone-gaming.webp"),
                            QuickLink("Điện thoại pin trâu", "usage=battery", "phone-orange.webp"),
                            QuickLink("Điện thoại 5G", "network=5g", "phone-violet.webp"),
                            QuickLink("Điện thoại chụp ảnh đẹp", "usage=camera", "phone-camera.webp"),
                            QuickLink("Điện thoại gập", "design=fold", "phone-rose.webp"),
                            QuickLink("Điện thoại cao cấp", "tier=flagship", "phone-orange.webp")
                        ],
                        Brands =
                        [
                            Brand("Apple"), Brand("Samsung"), Brand("Xiaomi"), Brand("OPPO"),
                            Brand("TECNO"), Brand("HONOR"), Brand("Nubia"), Brand("Sony"),
                            Brand("Nokia"), Brand("Infinix"), Brand("Nothing")
                        ],
                        Products = HomeProductCardFactory.AddVariants(
                        [
                            Product("iphone-17-pro-max-256gb", "iPhone 17 Pro Max 256GB | Chính hãng", "phone-orange.webp",
                                "36.990.000đ", "37.990.000đ", "Giảm 3%", "Smember giảm đến 370.000đ",
                                "Trả góp 0%, không phụ phí, trả trước từ 0đ."),
                            Product("galaxy-s26-ultra-5g", "Samsung Galaxy S26 Ultra 5G 12GB 256GB", "phone-violet.webp",
                                "31.990.000đ", "36.990.000đ", "Giảm 14%", "Smember giảm đến 320.000đ",
                                "Không phí chuyển đổi khi trả góp 0% qua thẻ tín dụng.",
                                "S-Student giảm thêm 500.000đ"),
                            Product("oppo-find-x9-ultra", "OPPO Find X9 Ultra 12GB 512GB", "phone-camera.webp",
                                "48.990.000đ", "49.990.000đ", "Giảm 2%", "Smember giảm đến 490.000đ",
                                "Tặng eSIM 5G và gói bảo hành mở rộng.",
                                "S-Student giảm thêm 300.000đ"),
                            Product("oppo-find-x9s", "OPPO Find X9s 12GB 256GB", "phone-rose.webp",
                                "23.990.000đ", "24.990.000đ", "Giảm 4%", "Smember giảm đến 240.000đ",
                                "Thu cũ trợ giá thêm đến 1 triệu đồng.",
                                "S-Student giảm thêm 300.000đ"),
                            Product("nubia-neo-5-pro", "Nubia Neo 5 Pro 5G 8GB 128GB", "phone-gaming.webp",
                                "6.990.000đ", "7.490.000đ", "Giảm 7%", "Smember giảm đến 70.000đ",
                                "Tặng ốp lưng và hỗ trợ trả góp nhanh."),
                            Product("iphone-17-pro-256gb", "iPhone 17 Pro 256GB | Chính hãng", "phone-orange.webp",
                                "33.790.000đ", "34.990.000đ", "Giảm 3%", "Smember giảm đến 338.000đ",
                                "Trả góp 0%, không phụ phí, trả trước từ 0đ."),
                            Product("galaxy-s26-5g", "Samsung Galaxy S26 5G 12GB 256GB", "phone-rose.webp",
                                "21.490.000đ", "25.990.000đ", "Giảm 17%", "Smember giảm đến 215.000đ",
                                "Không phí chuyển đổi khi trả góp qua thẻ tín dụng.",
                                "S-Student giảm thêm 300.000đ"),
                            Product("iphone-17-256gb", "iPhone 17 256GB | Chính hãng", "phone-rose.webp",
                                "23.990.000đ", "24.990.000đ", "Giảm 4%", "Smember giảm đến 240.000đ",
                                "Bảo hành chính hãng 12 tháng."),
                            Product("galaxy-a57-5g", "Samsung Galaxy A57 5G 8GB 256GB", "phone-violet.webp",
                                "9.490.000đ", "10.790.000đ", "Giảm 12%", "Smember giảm đến 95.000đ",
                                "Tặng gói bảo hành rơi vỡ trong 6 tháng.",
                                "S-Student giảm thêm 200.000đ"),
                            Product("nubia-neo-5-5g", "Nubia Neo 5 5G 8GB 128GB", "phone-gaming.webp",
                                "7.490.000đ", "8.190.000đ", "Giảm 8%", "Smember giảm đến 75.000đ",
                                "Tặng phụ kiện gaming và giao hàng miễn phí.")
                        ],
                        [
                            new(0, "iphone-17-pro-max-512gb", "iPhone 17 Pro Max 512GB | Chính hãng"),
                            new(1, "galaxy-s26-ultra-1tb", "Samsung Galaxy S26 Ultra 5G 1TB"),
                            new(2, "oppo-find-x9-ultra-1tb", "OPPO Find X9 Ultra 16GB 1TB"),
                            new(3, "oppo-find-x9s-512gb", "OPPO Find X9s 16GB 512GB"),
                            new(4, "nubia-neo-5-pro-256gb", "Nubia Neo 5 Pro 5G 12GB 256GB")
                        ])
                    }
                },
                new()
                {
                    Id = "tablets",
                    Label = "Máy tính bảng",
                    Url = "/catalog?cat=tablet",
                    Panel = new CategoryProductPanelViewModel
                    {
                        ViewAllUrl = "/catalog?cat=tablet",
                        Banners =
                        [
                            HomeCategoryBannerFactory.Create(
                                "banner_ipad.png",
                                "Ưu đãi iPad nổi bật",
                                "/catalog?cat=tablet&brand=apple"),
                            HomeCategoryBannerFactory.Create(
                                "banner_galaxy_tab.png",
                                "Ưu đãi Samsung Galaxy Tab",
                                "/catalog?cat=tablet&brand=samsung")
                        ],
                        QuickLinks =
                        [
                            QuickLink("iPad cho học tập", "usage=study", "phone-orange.webp", "tablet"),
                            QuickLink("Máy tính bảng Android", "os=android", "phone-violet.webp", "tablet"),
                            QuickLink("Máy tính bảng 5G", "network=5g", "phone-camera.webp", "tablet"),
                            QuickLink("Máy tính bảng có bút", "feature=stylus", "phone-rose.webp", "tablet"),
                            QuickLink("Màn hình lớn", "feature=large-display", "phone-gaming.webp", "tablet"),
                            QuickLink("Máy tính bảng cao cấp", "tier=flagship", "phone-orange.webp", "tablet")
                        ],
                        Brands =
                        [
                            Brand("Apple", "tablet"), Brand("Samsung", "tablet"),
                            Brand("Xiaomi", "tablet"), Brand("Lenovo", "tablet"),
                            Brand("Huawei", "tablet"), Brand("OPPO", "tablet")
                        ],
                        Products = HomeProductCardFactory.AddVariants(
                        [
                            Product("ipad-pro-m5-11", "iPad Pro M5 11 inch Wi-Fi 256GB", "phone-orange.webp",
                                "28.990.000đ", "30.990.000đ", "Giảm 6%", "Smember giảm đến 290.000đ",
                                "Tặng ưu đãi phụ kiện khi mua kèm Apple Pencil."),
                            Product("galaxy-tab-s11-ultra", "Samsung Galaxy Tab S11 Ultra 5G 256GB", "phone-violet.webp",
                                "27.990.000đ", "31.990.000đ", "Giảm 12%", "Smember giảm đến 280.000đ",
                                "Tặng bàn phím và gói bảo hành mở rộng.",
                                "S-Student giảm thêm 500.000đ"),
                            Product("ipad-air-m4-11", "iPad Air M4 11 inch Wi-Fi 128GB", "phone-camera.webp",
                                "17.990.000đ", "19.990.000đ", "Giảm 10%", "Smember giảm đến 180.000đ",
                                "Hỗ trợ trả góp 0% qua thẻ tín dụng."),
                            Product("galaxy-tab-s10-fe", "Samsung Galaxy Tab S10 FE 5G 128GB", "phone-rose.webp",
                                "13.990.000đ", "15.990.000đ", "Giảm 12%", "Smember giảm đến 140.000đ",
                                "Tặng bao da và bút S Pen chính hãng.",
                                "S-Student giảm thêm 300.000đ"),
                            Product("xiaomi-pad-8-pro", "Xiaomi Pad 8 Pro Wi-Fi 256GB", "phone-gaming.webp",
                                "11.490.000đ", "12.990.000đ", "Giảm 11%", "Smember giảm đến 115.000đ",
                                "Tặng bộ bàn phím và giao hàng miễn phí."),
                            Product("ipad-mini-8", "iPad mini 8 Wi-Fi 128GB", "phone-orange.webp",
                                "14.990.000đ", "16.490.000đ", "Giảm 9%", "Smember giảm đến 150.000đ",
                                "Thiết kế nhỏ gọn, bảo hành chính hãng 12 tháng."),
                            Product("lenovo-tab-pro-12", "Lenovo Tab Pro 12.7 inch 256GB", "phone-violet.webp",
                                "9.990.000đ", "11.490.000đ", "Giảm 13%", "Smember giảm đến 100.000đ",
                                "Tặng bao da và bút cảm ứng."),
                            Product("oppo-pad-neo-5g", "OPPO Pad Neo 5G 256GB", "phone-rose.webp",
                                "10.990.000đ", "12.490.000đ", "Giảm 12%", "Smember giảm đến 110.000đ",
                                "Ưu đãi mua kèm tai nghe không dây."),
                            Product("huawei-matepad-air", "Huawei MatePad Air 12 inch 256GB", "phone-camera.webp",
                                "12.490.000đ", "13.990.000đ", "Giảm 10%", "Smember giảm đến 125.000đ",
                                "Tặng bàn phím thông minh chính hãng."),
                            Product("redmi-pad-pro-5g", "Redmi Pad Pro 5G 128GB", "phone-gaming.webp",
                                "8.490.000đ", "9.490.000đ", "Giảm 10%", "Smember giảm đến 85.000đ",
                                "Pin lớn, màn hình 120Hz và hỗ trợ SIM 5G.")
                        ],
                        [
                            new(0, "ipad-pro-m5-13", "iPad Pro M5 13 inch Wi-Fi 512GB"),
                            new(1, "galaxy-tab-s11-ultra-512gb", "Samsung Galaxy Tab S11 Ultra 5G 512GB"),
                            new(2, "ipad-air-m4-13", "iPad Air M4 13 inch Wi-Fi 256GB"),
                            new(3, "galaxy-tab-s10-fe-plus", "Samsung Galaxy Tab S10 FE Plus 5G"),
                            new(4, "xiaomi-pad-8-pro-512gb", "Xiaomi Pad 8 Pro Wi-Fi 512GB")
                        ])
                    }
                }
            ]
        };

        CategoryQuickLinkViewModel QuickLink(
            string label,
            string query,
            string imageName,
            string category = "phone")
        {
            return new CategoryQuickLinkViewModel
            {
                Label = label,
                Url = $"/catalog?cat={category}&{query}",
                ImageUrl = $"{imageRoot}/{imageName}"
            };
        }

        static CategoryBrandViewModel Brand(string label, string category = "phone")
        {
            return new CategoryBrandViewModel
            {
                Label = label,
                Url = $"/catalog?cat={category}&brand={Uri.EscapeDataString(label.ToLowerInvariant())}"
            };
        }

        ProductCardViewModel Product(
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
                $"{imageRoot}/{imageName}",
                price,
                oldPrice,
                discount,
                memberOffer,
                note,
                studentOffer: studentOffer);
        }
    }
}
