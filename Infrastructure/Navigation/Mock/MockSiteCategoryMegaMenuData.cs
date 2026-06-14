using e_commerce_web_customer.Application.Navigation;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Navigation.Mock;

internal static class MockSiteCategoryMegaMenuData
{
    public static IReadOnlyList<SiteCategoryMenuGroupViewModel> Phone { get; } =
    [
        Group("Thương hiệu", "/catalog?cat=phone&brand=",
            "Apple", "Samsung", "Xiaomi", "OPPO", "HONOR", "Sony", "Nokia", "realme"),
        Group("Điện thoại nổi bật", "/catalog?cat=phone&q=",
            "iPhone 17 Pro Max", "Galaxy S26 Ultra", "OPPO Find X9 Ultra", "Xiaomi 17T", "HONOR 600 5G"),
        Group("Máy tính bảng", "/catalog?cat=tablet&q=",
            "iPad Pro", "iPad Air", "Galaxy Tab S11", "Xiaomi Pad 7", "Lenovo Legion Tab"),
        Group("Khoảng giá", "/catalog?cat=phone&price=",
            "Dưới 2 triệu", "Từ 2 - 5 triệu", "Từ 5 - 10 triệu", "Từ 10 - 20 triệu", "Trên 20 triệu")
    ];

    public static IReadOnlyList<SiteCategoryMenuGroupViewModel> Laptop { get; } =
    [
        Group("Thương hiệu", "/catalog?cat=laptop&brand=",
            "MacBook", "ASUS", "Lenovo", "Dell", "HP", "Acer", "MSI", "Gigabyte", "Samsung"),
        Group("Nhu cầu sử dụng", "/catalog?cat=laptop&usage=",
            "Văn phòng", "Gaming", "Mỏng nhẹ", "Đồ họa - kỹ thuật", "Sinh viên", "Cảm ứng", "Laptop AI"),
        Group("Dòng chip", "/catalog?cat=laptop&chip=",
            "Core i3", "Core i5", "Core i7", "Core Ultra", "Apple M4", "Apple M5", "AMD Ryzen"),
        Group("Phân khúc giá", "/catalog?cat=laptop&price=",
            "Dưới 10 triệu", "Từ 10 - 15 triệu", "Từ 15 - 20 triệu", "Từ 20 - 30 triệu", "Trên 30 triệu")
    ];

    public static IReadOnlyList<SiteCategoryMenuGroupViewModel> Audio { get; } =
    [
        Group("Chọn loại tai nghe", "/catalog?cat=audio&type=",
            "Bluetooth", "Chụp tai", "Nhét tai", "Có dây", "Thể thao", "Gaming"),
        Group("Hãng tai nghe", "/catalog?cat=audio&brand=",
            "AirPods", "Sony", "JBL", "Samsung", "Marshall", "Bose", "Edifier", "Xiaomi", "Anker"),
        Group("Loa nổi bật", "/catalog?cat=speaker&brand=",
            "JBL", "Marshall", "Harman Kardon", "Samsung", "Sony", "LG", "Tronsmart"),
        Group("Mic thu âm", "/catalog?cat=microphone&type=",
            "Mic cài áo", "Mic podcast", "Mic livestream", "Micro không dây", "Mic karaoke")
    ];

    public static IReadOnlyList<SiteCategoryMenuGroupViewModel> Watch { get; } =
    [
        Group("Đồng hồ thông minh", "/catalog?cat=watch&brand=",
            "Apple Watch", "Samsung", "Garmin", "Huawei", "Xiaomi", "Amazfit"),
        Group("Nhu cầu sử dụng", "/catalog?cat=watch&usage=",
            "Thể thao", "Sức khỏe", "Trẻ em", "Thời trang", "Nghe gọi độc lập"),
        Group("Camera", "/catalog?cat=camera&type=",
            "Camera an ninh", "Camera hành trình", "Webcam", "Camera 360", "Camera ngoài trời"),
        Group("Thiết bị quay", "/catalog?cat=camera-accessories&type=",
            "Gimbal", "Flycam", "Action camera", "Tripod", "Phụ kiện máy ảnh")
    ];

    public static IReadOnlyList<SiteCategoryMenuGroupViewModel> Appliances { get; } =
    [
        Group("Gia dụng nhà bếp", "/catalog?cat=appliances&type=",
            "Nồi chiên không dầu", "Nồi cơm điện", "Máy xay", "Bếp điện", "Máy lọc nước"),
        Group("Chăm sóc nhà cửa", "/catalog?cat=appliances&type=",
            "Robot hút bụi", "Máy hút bụi", "Máy lọc không khí", "Quạt", "Máy hút ẩm"),
        Group("Làm đẹp", "/catalog?cat=beauty&type=",
            "Máy sấy tóc", "Máy tạo kiểu tóc", "Máy cạo râu", "Bàn chải điện", "Máy rửa mặt"),
        Group("Chăm sóc sức khỏe", "/catalog?cat=health&type=",
            "Máy massage", "Cân sức khỏe", "Máy đo huyết áp", "Máy tăm nước")
    ];

    public static IReadOnlyList<SiteCategoryMenuGroupViewModel> Accessories { get; } =
    [
        Group("Phụ kiện điện thoại", "/catalog?cat=accessories&type=",
            "Ốp lưng", "Dán màn hình", "Cáp sạc", "Củ sạc", "Pin dự phòng"),
        Group("Phụ kiện Apple", "/catalog?cat=accessories&brand=",
            "AirTag", "Apple Pencil", "Magic Keyboard", "MagSafe", "AirPods"),
        Group("Lưu trữ và kết nối", "/catalog?cat=accessories&type=",
            "Thẻ nhớ", "USB", "Hub chuyển đổi", "Thiết bị mạng", "Ổ cứng di động"),
        Group("Phụ kiện khác", "/catalog?cat=accessories&type=",
            "Balo - túi xách", "Gaming Gear", "Giá đỡ", "Gimbal", "Phụ kiện laptop")
    ];

    public static IReadOnlyList<SiteCategoryMenuGroupViewModel> Pc { get; } =
    [
        Group("Máy tính để bàn", "/catalog?cat=desktop&type=",
            "Build PC", "PC Gaming", "PC đồ họa", "PC văn phòng", "Máy tính đồng bộ"),
        Group("Màn hình máy tính", "/catalog?cat=monitor&usage=",
            "Gaming", "Văn phòng", "Đồ họa", "Màn hình cong", "Màn hình di động"),
        Group("Linh kiện máy tính", "/catalog?cat=computer-accessories&type=",
            "CPU", "Mainboard", "RAM", "Ổ cứng", "Card màn hình", "Nguồn máy tính"),
        Group("Máy in và thiết bị", "/catalog?cat=printer&type=",
            "Máy in laser", "Máy in màu", "Máy in đa năng", "Mực in", "Máy scan")
    ];

    public static IReadOnlyList<SiteCategoryMenuGroupViewModel> Tv { get; } =
    [
        Group("Tivi", "/catalog?cat=tv&type=",
            "Smart Tivi", "Google Tivi", "Tivi OLED", "Tivi Mini LED", "Tivi 4K"),
        Group("Thương hiệu Tivi", "/catalog?cat=tv&brand=",
            "Samsung", "LG", "Sony", "TCL", "Xiaomi", "Hisense"),
        Group("Điện lạnh", "/catalog?cat=home-electronics&type=",
            "Máy lạnh", "Tủ lạnh", "Máy giặt", "Máy sấy", "Tủ đông"),
        Group("Thiết bị giải trí", "/catalog?cat=entertainment&type=",
            "Android TV Box", "Máy chiếu", "Loa thanh", "Điều khiển Tivi")
    ];

    public static IReadOnlyList<SiteCategoryMenuGroupViewModel> TradeIn { get; } =
    [
        Group("Thu cũ theo sản phẩm", "/catalog?cat=trade-in&type=",
            "Điện thoại", "Máy tính bảng", "Laptop", "Đồng hồ", "Tai nghe"),
        Group("Nâng cấp thiết bị", "/catalog?cat=trade-in&upgrade=",
            "Lên đời iPhone", "Lên đời Samsung", "Đổi laptop mới", "Đổi máy tính bảng"),
        Group("Quy trình", "/catalog?cat=trade-in&step=",
            "Định giá sản phẩm", "Kiểm tra thiết bị", "Nhận trợ giá", "Hoàn tất đổi máy"),
        Group("Thông tin hỗ trợ", "/catalog?cat=trade-in&help=",
            "Bảng giá thu cũ", "Điều kiện thu cũ", "Cửa hàng áp dụng", "Câu hỏi thường gặp")
    ];

    public static IReadOnlyList<SiteCategoryMenuGroupViewModel> Used { get; } =
    [
        Group("Sản phẩm đã qua sử dụng", "/catalog?cat=used&type=",
            "Điện thoại cũ", "Laptop cũ", "Tablet cũ", "Đồng hồ cũ", "Phụ kiện cũ"),
        Group("Tình trạng sản phẩm", "/catalog?cat=used&condition=",
            "Đẹp như mới", "Trầy xước nhẹ", "Đã kích hoạt", "Hàng trưng bày"),
        Group("Khoảng giá", "/catalog?cat=used&price=",
            "Dưới 5 triệu", "Từ 5 - 10 triệu", "Từ 10 - 20 triệu", "Trên 20 triệu"),
        Group("Chính sách", "/catalog?cat=used&policy=",
            "Bảo hành hàng cũ", "Đổi trả 30 ngày", "Kiểm định chất lượng", "Trả góp")
    ];

    public static IReadOnlyList<SiteCategoryMenuGroupViewModel> Deals { get; } =
    [
        Group("Khuyến mãi nổi bật", "/catalog?cat=deals&campaign=",
            "Giảm giá cuối tuần", "Ưu đãi thành viên", "Flash Sale", "Freeship", "Trả góp 0%"),
        Group("Ưu đãi theo ngành hàng", "/catalog?cat=deals&category=",
            "Điện thoại", "Laptop", "Phụ kiện", "Âm thanh", "Đồng hồ", "Gia dụng"),
        Group("Chương trình đặc biệt", "/catalog?cat=deals&program=",
            "S-Student", "Thu cũ đổi mới", "Ưu đãi doanh nghiệp", "Combo tiết kiệm"),
        Group("Mã giảm giá", "/catalog?cat=deals&voucher=",
            "Voucher thanh toán", "Voucher ứng dụng", "Ưu đãi ngân hàng", "Mã freeship")
    ];

    public static IReadOnlyList<SiteCategoryMenuGroupViewModel> Tech { get; } =
    [
        Group("Chủ đề công nghệ", "/catalog?cat=tech&topic=",
            "Tin mới", "Đánh giá sản phẩm", "So sánh", "Tư vấn mua sắm", "Thủ thuật"),
        Group("Sản phẩm", "/catalog?cat=tech&product=",
            "Điện thoại", "Laptop", "Máy tính bảng", "Đồng hồ", "Âm thanh"),
        Group("Hướng dẫn", "/catalog?cat=tech&type=",
            "Cài đặt thiết bị", "Sao lưu dữ liệu", "Bảo mật", "Tối ưu hiệu năng"),
        Group("Xu hướng", "/catalog?cat=tech&trend=",
            "AI", "Thiết bị gập", "Gaming", "Smart Home", "Năng lượng xanh")
    ];

    private static SiteCategoryMenuGroupViewModel Group(
        string title,
        string urlPrefix,
        params string[] labels)
    {
        return new SiteCategoryMenuGroupViewModel
        {
            Title = title,
            Links = labels
                .Select(label => new SiteCategoryMenuLinkViewModel
                {
                    Label = label,
                    Url = $"{urlPrefix}{Uri.EscapeDataString(label.ToLowerInvariant())}"
                })
                .ToArray()
        };
    }
}
