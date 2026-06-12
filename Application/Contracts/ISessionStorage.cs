namespace e_commerce_web_customer.Application.Contracts;

/// <summary>
/// Abstraction over the underlying session mechanism to decouple Application layer from web frameworks.
/// </summary>
public interface ISessionStorage
{
    string? GetString(string key);
    void SetString(string key, string value);
    void Remove(string key);
}
