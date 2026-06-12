using e_commerce_web_customer.ViewModels.Account;

namespace e_commerce_web_customer.Application.Contracts;

/// <summary>
/// Defines the authentication and account operations.
/// This interface decouples the controller from the data access/identity layer,
/// allowing easy switching between mock implementation and a real database (e.g. EF Core, Identity).
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Authenticates a user with email and password.
    /// </summary>
    Task<bool> LoginAsync(string email, string password, bool rememberMe);

    /// <summary>
    /// Registers a new user.
    /// </summary>
    Task<bool> RegisterAsync(RegisterViewModel model);

    /// <summary>
    /// Checks if a user email already exists in the system.
    /// </summary>
    Task<bool> UserExistsAsync(string email);
}
