using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.ViewModels.Account;

namespace e_commerce_web_customer.Infrastructure.MockData;

/// <summary>
/// Simulation of a user repository/database in-memory.
/// Allows registration and login to work dynamically before database is attached.
/// </summary>
public sealed class MockAccountService : IAccountService
{
    private sealed class MockUser
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string FullName { get; set; }
        public string? PhoneNumber { get; set; }
    }

    // Thread-safe in-memory database of users
    private static readonly List<MockUser> Users =
    [
        new MockUser
        {
            Email = "demo@techstore.vn",
            Password = "Password123",
            FullName = "Nguyễn Văn Demo",
            PhoneNumber = "0987654321"
        }
    ];

    private static readonly object Lock = new();

    public Task<bool> LoginAsync(string email, string password, bool rememberMe)
    {
        lock (Lock)
        {
            var user = Users.FirstOrDefault(u =>
                string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password); // Plain text compare for mock simplicity

            return Task.FromResult(user is not null);
        }
    }

    public Task<bool> RegisterAsync(RegisterViewModel model)
    {
        lock (Lock)
        {
            if (Users.Any(u => string.Equals(u.Email, model.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.FromResult(false);
            }

            Users.Add(new MockUser
            {
                Email = model.Email,
                Password = model.Password, // Mock simplicity
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber
            });

            return Task.FromResult(true);
        }
    }

    public Task<bool> UserExistsAsync(string email)
    {
        lock (Lock)
        {
            var exists = Users.Any(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(exists);
        }
    }
}
