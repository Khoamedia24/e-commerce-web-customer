using e_commerce_web_customer.Application.Contracts;

namespace e_commerce_web_customer.Infrastructure.Web;

public sealed class WebSessionStorage(IHttpContextAccessor httpContextAccessor) : ISessionStorage
{
    private ISession Session =>
        httpContextAccessor.HttpContext?.Session
        ?? throw new InvalidOperationException("No active HTTP session.");

    public string? GetString(string key)
    {
        return Session.GetString(key);
    }

    public void SetString(string key, string value)
    {
        Session.SetString(key, value);
    }

    public void Remove(string key)
    {
        Session.Remove(key);
    }
}
