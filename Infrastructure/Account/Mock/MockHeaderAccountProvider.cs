using e_commerce_web_customer.Application.Account;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Account.Mock;

public sealed class MockHeaderAccountProvider : IHeaderAccountProvider
{
    private static readonly IReadOnlyList<HeaderAccountNotificationViewModel> SampleNotifications =
    [
        new HeaderAccountNotificationViewModel
        {
            Id = "order-delivered-wn0303995253",
            Category = "orders",
            LeadText = "Đơn hàng",
            HighlightText = "WN0303995253",
            TailText = "vừa được giao thành công. Cảm ơn bạn đã đặt hàng tại TechStore.",
            TimeAgoText = "5 tháng trước",
            DetailUrl = "/orders/track?code=WN0303995253"
        },
        new HeaderAccountNotificationViewModel
        {
            Id = "order-ready-wn0303995253",
            Category = "orders",
            LeadText = "Đơn hàng",
            HighlightText = "WN0303995253",
            TailText = "đã sẵn hàng tại 4/39 Quang Trung, Thới Tam Thôn, H. Hóc Môn, TP. HCM. Mời bạn đến nhận sản phẩm trong vòng 24h nhé!",
            TimeAgoText = "5 tháng trước",
            DetailUrl = "/orders/track?code=WN0303995253"
        },
        new HeaderAccountNotificationViewModel
        {
            Id = "order-processing-wn0303995253",
            Category = "orders",
            LeadText = "Cảm ơn bạn đã đặt hàng. Đơn hàng",
            HighlightText = "WN0303995253",
            TailText = "của bạn đang được xử lý.",
            TimeAgoText = "5 tháng trước",
            DetailUrl = "/orders/track?code=WN0303995253"
        },
        new HeaderAccountNotificationViewModel
        {
            Id = "order-processing-wn0303995250",
            Category = "orders",
            LeadText = "Cảm ơn bạn đã đặt hàng. Đơn hàng",
            HighlightText = "WN0303995250",
            TailText = "của bạn đang được xử lý.",
            TimeAgoText = "5 tháng trước",
            DetailUrl = "/orders/track?code=WN0303995250"
        }
    ];

    public Task<HeaderAccountViewModel> GetAccountAsync(
        bool isLoggedIn,
        string? email,
        string? displayName,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!isLoggedIn)
        {
            return Task.FromResult(new HeaderAccountViewModel());
        }

        var fullName = ResolveName(displayName, email);

        return Task.FromResult(new HeaderAccountViewModel
        {
            IsLoggedIn = true,
            DisplayName = ToButtonName(fullName),
            FullName = fullName,
            Email = email,
            Notifications = SampleNotifications
        });
    }

    private static string ResolveName(string? displayName, string? email)
    {
        if (!string.IsNullOrWhiteSpace(displayName))
        {
            return displayName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            return email.Split('@', StringSplitOptions.RemoveEmptyEntries)[0];
        }

        return "Thành viên";
    }

    private static string ToButtonName(string fullName)
    {
        var firstPart = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return string.IsNullOrWhiteSpace(firstPart) ? fullName : firstPart;
    }
}
