using Microsoft.AspNetCore.Http;

namespace e_commerce_web_customer.Application.Contracts;

/// <summary>
/// MoMo payment gateway integration — mirrors MoMoService in sportset_customer Flutter app.
/// </summary>
public interface IMoMoIntegration
{
    /// <summary>
    /// Calls MoMo sandbox API to create a payment and returns the payUrl.
    /// Equivalent to MoMoService.createMoMoPayment() in Flutter.
    /// </summary>
    Task<MoMoCreateResult> CreatePaymentAsync(
        MoMoCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the HMAC-SHA256 signature from MoMo redirect callback.
    /// Equivalent to verifying the deeplink params in Flutter.
    /// </summary>
    MoMoCallbackResult ProcessCallback(IQueryCollection query);
}

// ── Request / Result models ──────────────────────────────────────────────────

public sealed record MoMoCreateRequest(
    string OrderId,
    long Amount,
    string OrderInfo,
    string RedirectUrl,
    string IpnUrl);

public sealed record MoMoCreateResult(
    bool Success,
    string? PayUrl,
    string? ErrorMessage = null);

public sealed record MoMoCallbackResult(
    bool Success,
    string OrderId,
    string TransactionId,
    string ResultCode,
    string Message);
