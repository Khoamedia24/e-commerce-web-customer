using e_commerce_web_customer.Application.Services;

namespace e_commerce_web_customer.Application.Contracts;

/// <summary>
/// Validates and hydrates a cart item before it is saved to session.
/// Protects against malicious client-side price manipulation.
/// </summary>
public interface ICartItemValidator
{
    Task<CartSessionItem> ValidateAsync(CartSessionItem requestItem);
}
