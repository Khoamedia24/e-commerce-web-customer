using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Home.Mock;

internal static class AccessoryDirectoryFactory
{
    public static CategoryDirectoryViewModel Create()
    {
        const string imageRoot = "/images/categories/accessories";

        return new CategoryDirectoryViewModel
        {
            Id = "quality-accessories",
            Title = "Sắm thêm phụ kiện chất lượng",
            ViewAllUrl = "/catalog?cat=accessories",
            Items =
            [
                Item("Phụ kiện Apple", "apple", "apple-accessories"),
                Item("Cáp, sạc", "charging", "charging-cables"),
                Item("Pin sạc dự phòng", "power-bank", "power-banks"),
                Item("Ốp lưng - Bao da", "cases", "phone-cases"),
                Item("Dán màn hình", "screen-protector", "screen-protectors"),
                Item("Thẻ nhớ, USB", "memory-usb", "memory-usb"),
                Item("Gaming Gear, Playstation", "gaming", "gaming-playstation"),
                Item("Sim 4G - 5G", "sim", "sim-4g-5g"),
                Item("Thiết bị mạng", "network", "network-devices"),
                Item("Camera", "security-camera", "security-camera"),
                Item("Gimbal", "gimbal", "gimbal"),
                Item("Flycam", "drone", "drone"),
                Item("Máy ảnh", "camera", "cameras"),
                Item("Chuột, bàn phím", "keyboard-mouse", "keyboard-mouse"),
                Item("Balo, túi xách", "backpack", "backpacks"),
                Item("Hub chuyển đổi", "usb-c-hub", "usb-c-hubs"),
                Item("Phụ kiện điện thoại", "phone-accessories", "phone-pouches"),
                Item("Phụ kiện Laptop", "laptop-accessories", "laptop-accessories")
            ]
        };

        CategoryDirectoryItemViewModel Item(string label, string slug, string imageName)
        {
            return new CategoryDirectoryItemViewModel
            {
                Label = label,
                Url = $"/catalog?cat=accessories&type={slug}",
                ImageUrl = $"{imageRoot}/{imageName}.webp",
                ImageAlt = label
            };
        }
    }
}
