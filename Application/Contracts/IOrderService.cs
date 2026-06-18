using e_commerce_web_customer.Application.Orders;

namespace e_commerce_web_customer.Application.Contracts;

public interface IOrderService
{
    Task<PlacedOrder> PlaceOrderAsync(
        PlaceOrderRequest request,
        CancellationToken cancellationToken = default);

    Task UpdatePaymentStatusAsync(
        string orderCode,
        bool isPaid,
        string transactionId,
        CancellationToken cancellationToken = default);
}
