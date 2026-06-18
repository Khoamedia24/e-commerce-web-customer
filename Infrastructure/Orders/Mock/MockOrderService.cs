using System.Collections.Concurrent;
using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Orders;

namespace e_commerce_web_customer.Infrastructure.Orders.Mock;

public sealed class MockOrderService : IOrderService
{
    private readonly ConcurrentDictionary<string, PlaceOrderRequest> _orders = new();

    public Task<PlacedOrder> PlaceOrderAsync(
        PlaceOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (request.Items.Count == 0)
        {
            throw new OrderPlacementException("Đơn hàng không có sản phẩm.");
        }

        var placedAt = DateTimeOffset.Now;
        var orderCode = $"TS{placedAt:yyyyMMddHHmmss}{Random.Shared.Next(100, 1000)}";
        _orders[orderCode] = request;

        return Task.FromResult(new PlacedOrder(
            orderCode,
            placedAt,
            placedAt.AddDays(2)));
    }

    public Task UpdatePaymentStatusAsync(
        string orderCode,
        bool isPaid,
        string transactionId,
        CancellationToken cancellationToken = default)
    {
        // Mock: no persistent storage — status update is a no-op
        return Task.CompletedTask;
    }
}
