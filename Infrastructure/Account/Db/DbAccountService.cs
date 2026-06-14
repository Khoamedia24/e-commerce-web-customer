using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Data;
using e_commerce_web_customer.ViewModels.Account;

namespace e_commerce_web_customer.Infrastructure.Account.Db;

public sealed class DbAccountService(EcommerceDbContext dbContext) : IAccountService
{
    public Task<bool> LoginAsync(
        string email,
        string password,
        bool rememberMe,
        CancellationToken cancellationToken = default)
    {
        _ = dbContext;
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(false);
    }

    public Task<AccountProfile?> GetProfileAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        _ = dbContext;
        _ = email;
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult<AccountProfile?>(null);
    }

    public Task<bool> RegisterAsync(
        RegisterViewModel model,
        CancellationToken cancellationToken = default)
    {
        _ = dbContext;
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(false);
    }

    public Task<bool> UserExistsAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        _ = dbContext;
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(false);
    }
}
