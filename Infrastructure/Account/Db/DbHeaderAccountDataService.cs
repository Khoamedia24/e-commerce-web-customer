using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Data;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Account.Db;

public sealed class DbHeaderAccountDataService(EcommerceDbContext dbContext) : IHeaderAccountDataService
{
    public Task<HeaderAccountViewModel> GetAccountAsync(
        string email,
        string displayName,
        CancellationToken cancellationToken = default)
    {
        _ = dbContext;
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new HeaderAccountViewModel
        {
            IsLoggedIn = true,
            DisplayName = ToButtonName(displayName),
            FullName = displayName,
            Email = email,
            Notifications = []
        });
    }

    private static string ToButtonName(string fullName)
    {
        var firstPart = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return string.IsNullOrWhiteSpace(firstPart) ? fullName : firstPart;
    }
}
