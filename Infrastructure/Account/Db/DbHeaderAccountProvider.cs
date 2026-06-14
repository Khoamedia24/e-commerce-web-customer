using e_commerce_web_customer.Application.Account;
using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Account.Db;

public sealed class DbHeaderAccountProvider(
    IHeaderAccountDataService headerAccountDataService) : IHeaderAccountProvider
{
    public Task<HeaderAccountViewModel> GetAccountAsync(
        bool isLoggedIn,
        string? email,
        string? displayName,
        CancellationToken cancellationToken = default)
    {
        if (!isLoggedIn)
        {
            return Task.FromResult(new HeaderAccountViewModel());
        }

        var resolvedEmail = email?.Trim();
        var resolvedDisplayName = string.IsNullOrWhiteSpace(displayName)
            ? resolvedEmail ?? "Thành viên"
            : displayName.Trim();

        if (string.IsNullOrWhiteSpace(resolvedEmail))
        {
            return Task.FromResult(new HeaderAccountViewModel
            {
                IsLoggedIn = true,
                DisplayName = ToButtonName(resolvedDisplayName),
                FullName = resolvedDisplayName
            });
        }

        return headerAccountDataService.GetAccountAsync(
            resolvedEmail,
            resolvedDisplayName,
            cancellationToken);
    }

    private static string ToButtonName(string fullName)
    {
        var firstPart = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return string.IsNullOrWhiteSpace(firstPart) ? fullName : firstPart;
    }
}
