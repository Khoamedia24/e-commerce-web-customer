using e_commerce_web_customer.Application.Navigation;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Navigation.Mock;

public sealed class MockSiteCategoryMenuProvider : ISiteCategoryMenuProvider
{
    private static readonly SiteCategoryMenuViewModel Menu = new()
    {
        Items =
        [
            Item("site-cat-phone", "/catalog?cat=phone", "Điện thoại, Tablet", "phone",
                MockSiteCategoryMegaMenuData.Phone),
            Item("site-cat-laptop", "/catalog?cat=laptop", "Laptop", "laptop",
                MockSiteCategoryMegaMenuData.Laptop),
            Item("site-cat-audio", "/catalog?cat=audio", "Âm thanh, Mic thu âm", "audio",
                MockSiteCategoryMegaMenuData.Audio),
            Item("site-cat-watch", "/catalog?cat=watch", "Đồng hồ, Camera", "watch",
                MockSiteCategoryMegaMenuData.Watch),
            Item("site-cat-appliances", "/catalog?cat=appliances", "Đồ gia dụng, Làm đẹp", "home",
                MockSiteCategoryMegaMenuData.Appliances),
            Item("site-cat-accessories", "/catalog?cat=accessories", "Phụ kiện", "cable",
                MockSiteCategoryMegaMenuData.Accessories),
            Item("site-cat-pc", "/catalog?cat=pc", "PC, Màn hình, Máy in", "desktop",
                MockSiteCategoryMegaMenuData.Pc),
            Item("site-cat-tv", "/catalog?cat=tv", "Tivi, Điện máy", "tv",
                MockSiteCategoryMegaMenuData.Tv),
            Item("site-cat-tradein", "/catalog?cat=trade-in", "Thu cũ đổi mới", "swap",
                MockSiteCategoryMegaMenuData.TradeIn),
            Item("site-cat-used", "/catalog?cat=used", "Hàng cũ", "history",
                MockSiteCategoryMegaMenuData.Used),
            Item("site-cat-deals", "/catalog?cat=deals", "Khuyến mãi", "discount",
                MockSiteCategoryMegaMenuData.Deals, true),
            Item("site-cat-tech", "/catalog?cat=tech", "Tin công nghệ", "news",
                MockSiteCategoryMegaMenuData.Tech)
        ]
    };

    public Task<SiteCategoryMenuViewModel> GetMenuAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Menu);
    }

    private static SiteCategoryMenuItemViewModel Item(
        string id,
        string url,
        string label,
        string icon,
        IReadOnlyList<SiteCategoryMenuGroupViewModel> groups,
        bool isHighlighted = false)
    {
        return new SiteCategoryMenuItemViewModel
        {
            Id = id,
            Url = url,
            Label = label,
            Icon = icon,
            Groups = groups,
            IsHighlighted = isHighlighted
        };
    }
}
