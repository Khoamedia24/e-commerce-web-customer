using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Orders;
using e_commerce_web_customer.Data;

namespace e_commerce_web_customer.Infrastructure.Orders.Db;

public sealed class DbOrderService(EcommerceDbContext dbContext) : IOrderService
{
    public Task<PlacedOrder> PlaceOrderAsync(
        PlaceOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        _ = dbContext;
        _ = request;
        cancellationToken.ThrowIfCancellationRequested();

        throw new OrderPlacementException(
            "Database order persistence is not implemented yet.");
    }
}
