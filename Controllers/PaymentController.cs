using e_commerce_web_customer.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace e_commerce_web_customer.Controllers;

/// <summary>
/// Handles MoMo payment callbacks — mirrors payment_result_screen.dart in sportset_customer.
/// Flutter: deeplink yourapp://payment-result?resultCode=...
/// Web:     GET /payment/momo-return?resultCode=...&orderId=...&transId=...
/// </summary>
public sealed class PaymentController(
    IMoMoIntegration momoIntegration,
    IOrderService orderService,
    ILogger<PaymentController> logger) : Controller
{
    /// <summary>
    /// MoMo redirects here after payment (equivalent to Flutter deeplink handler).
    /// MoMo passes: resultCode, orderId, requestId, amount, orderInfo,
    ///              orderType, transId, message, payType, responseTime,
    ///              extraData, signature
    /// </summary>
    [HttpGet("payment/momo-return")]
    public async Task<IActionResult> MoMoReturn(CancellationToken cancellationToken)
    {
        // Log full callback URL for debugging signature issues
        var qs = string.Join(" | ", Request.Query.Select(kv => $"{kv.Key}={kv.Value}"));
        logger.LogInformation("[MoMo Callback] {QueryString}", qs);

        var result = momoIntegration.ProcessCallback(Request.Query);

        // Update order payment status in DB (mirrors _confirmBooking() in Flutter)
        if (!string.IsNullOrEmpty(result.OrderId))
        {
            await orderService.UpdatePaymentStatusAsync(
                result.OrderId,
                isPaid: result.Success,
                transactionId: result.TransactionId,
                cancellationToken);
        }

        return View("MoMoResult", new MoMoResultViewModel(
            Success: result.Success,
            OrderId: result.OrderId,
            TransactionId: result.TransactionId,
            ResultCode: result.ResultCode,
            Message: result.Message));
    }

    /// <summary>
    /// IPN (server-to-server notification) from MoMo.
    /// MoMo calls this in the background independently of the redirect.
    /// </summary>
    [HttpPost("payment/momo-ipn")]
    public async Task<IActionResult> MoMoIpn(CancellationToken cancellationToken)
    {
        var result = momoIntegration.ProcessCallback(Request.Query);

        if (result.Success && !string.IsNullOrEmpty(result.OrderId))
        {
            await orderService.UpdatePaymentStatusAsync(
                result.OrderId,
                isPaid: true,
                transactionId: result.TransactionId,
                cancellationToken);
        }

        // MoMo expects HTTP 204 or a JSON ack for IPN
        return Ok(new { statusCode = 0, message = "Received" });
    }
}

public sealed record MoMoResultViewModel(
    bool Success,
    string OrderId,
    string TransactionId,
    string ResultCode,
    string Message);
