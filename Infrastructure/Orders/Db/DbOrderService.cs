using System.Data;
using System.Globalization;
using System.Text;
using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Orders;
using e_commerce_web_customer.Data;
using e_commerce_web_customer.Models.Entities;
using e_commerce_web_customer.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace e_commerce_web_customer.Infrastructure.Orders.Db;

public sealed class DbOrderService(EcommerceDbContext dbContext) : IOrderService
{
    public async Task<PlacedOrder> PlaceOrderAsync(
        PlaceOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        try
        {
            var user = await ResolveUserAsync(request.UserEmail, cancellationToken);
            var paymentMethod = await ResolvePaymentMethodAsync(
                request.PaymentMethod,
                cancellationToken);
            var orderLines = await ResolveOrderLinesAsync(
                request.Items,
                cancellationToken);

            var subtotal = orderLines.Sum(line => line.Variant.Price * line.Quantity);
            var shippingFee = Math.Max(0m, request.ShippingFee);
            var discount = Math.Clamp(
                request.Discount,
                0m,
                subtotal + shippingFee);
            var now = DateTime.UtcNow;
            var orderCode = await GenerateOrderCodeAsync(now, cancellationToken);

            var order = new Order
            {
                UserId = user.Id,
                PaymentMethodId = paymentMethod.Id,
                OrderCode = orderCode,
                ShippingContactName = request.CustomerName.Trim(),
                ShippingPhone = request.Phone.Trim(),
                ShippingProvince = request.ShippingProvince.Trim(),
                ShippingWard = request.ShippingWard.Trim(),
                ShippingDetail = request.ShippingDetail.Trim(),
                SubtotalAmount = subtotal,
                ShippingFee = shippingFee,
                VoucherDiscount = discount,
                TotalAmount = subtotal + shippingFee - discount,
                OrderStatus = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Unpaid,
                CreatedAt = now
            };

            foreach (var line in orderLines)
            {
                line.Variant.Quantity -= line.Quantity;
                line.Variant.SoldCount += line.Quantity;

                if (line.Variant.Product is not null)
                {
                    line.Variant.Product.TotalSoldCount += line.Quantity;
                }

                order.OrderItems.Add(new OrderItem
                {
                    ProductVariantId = line.Variant.Id,
                    Quantity = line.Quantity,
                    UnitPrice = line.Variant.Price
                });
            }

            dbContext.Orders.Add(order);

            var orderedVariantIds = orderLines
                .Select(line => line.Variant.Id)
                .ToHashSet();
            var completedCartItems = await dbContext.CartItems
                .Where(item =>
                    item.UserId == user.Id
                    && orderedVariantIds.Contains(item.ProductVariantId))
                .ToListAsync(cancellationToken);

            if (completedCartItems.Count > 0)
            {
                dbContext.CartItems.RemoveRange(completedCartItems);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var placedAt = new DateTimeOffset(now, TimeSpan.Zero).ToLocalTime();
            return new PlacedOrder(
                orderCode,
                placedAt,
                placedAt.AddDays(2));
        }
        catch (OrderPlacementException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw new OrderPlacementException(
                "Không thể lưu đơn hàng. Vui lòng kiểm tra lại sản phẩm và thử lại.");
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw new OrderPlacementException(
                "Đặt hàng chưa thành công. Vui lòng thử lại sau.");
        }
    }

    private static void ValidateRequest(PlaceOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserEmail))
        {
            throw new OrderPlacementException("Không xác định được tài khoản đặt hàng.");
        }

        if (request.Items.Count == 0)
        {
            throw new OrderPlacementException("Đơn hàng không có sản phẩm.");
        }

        if (string.IsNullOrWhiteSpace(request.CustomerName)
            || string.IsNullOrWhiteSpace(request.Phone)
            || string.IsNullOrWhiteSpace(request.ShippingProvince)
            || string.IsNullOrWhiteSpace(request.ShippingWard)
            || string.IsNullOrWhiteSpace(request.ShippingDetail))
        {
            throw new OrderPlacementException("Thông tin giao hàng chưa đầy đủ.");
        }
    }

    private async Task<User> ResolveUserAsync(
        string userEmail,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = userEmail.Trim().ToLowerInvariant();
        var user = await dbContext.Users.FirstOrDefaultAsync(
            item => item.IsActive && item.Email.ToLower() == normalizedEmail,
            cancellationToken);

        return user
            ?? throw new OrderPlacementException("Tài khoản đặt hàng không còn hoạt động.");
    }

    private async Task<PaymentMethod> ResolvePaymentMethodAsync(
        string requestedMethod,
        CancellationToken cancellationToken)
    {
        var methods = await dbContext.PaymentMethods
            .Where(method => method.IsActive)
            .OrderBy(method => method.Id)
            .ToListAsync(cancellationToken);
        var requested = NormalizeText(requestedMethod);

        var method = requested switch
        {
            "cod" => methods.FirstOrDefault(item =>
                ContainsAny(NormalizeText(item.Name), "cod", "nhan hang")),
            "banktransfer" => methods.FirstOrDefault(item =>
                ContainsAny(NormalizeText(item.Name), "chuyen khoan", "napas", "noi dia")),
            "momo" => methods.FirstOrDefault(item =>
                ContainsAny(NormalizeText(item.Name), "momo")),
            "vnpay" => methods.FirstOrDefault(item =>
                ContainsAny(NormalizeText(item.Name), "vnpay")),
            "zalopay" => methods.FirstOrDefault(item =>
                ContainsAny(NormalizeText(item.Name), "zalopay")),
            _ => null
        };

        return method
            ?? throw new OrderPlacementException(
                "Phương thức thanh toán hiện không khả dụng.");
    }

    private async Task<IReadOnlyList<ResolvedOrderLine>> ResolveOrderLinesAsync(
        IReadOnlyList<PlaceOrderLine> requestedLines,
        CancellationToken cancellationToken)
    {
        var normalizedLines = requestedLines
            .Where(line => !string.IsNullOrWhiteSpace(line.ProductId))
            .GroupBy(line => line.ProductId.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(group => new
            {
                Key = group.Key,
                Quantity = group.Sum(line => Math.Max(1, line.Quantity))
            })
            .ToList();

        if (normalizedLines.Count == 0)
        {
            throw new OrderPlacementException("Đơn hàng không có sản phẩm hợp lệ.");
        }

        var resolvedLines = new List<ResolvedOrderLine>();
        foreach (var line in normalizedLines)
        {
            ProductVariant? variant;
            if (long.TryParse(
                line.Key,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var variantId))
            {
                variant = await dbContext.ProductVariants
                    .Include(item => item.Product)
                    .FirstOrDefaultAsync(
                        item =>
                            item.Id == variantId
                            && item.IsActive
                            && item.Product != null
                            && item.Product.IsActive,
                        cancellationToken);
            }
            else
            {
                variant = await dbContext.ProductVariants
                    .Include(item => item.Product)
                    .FirstOrDefaultAsync(
                        item =>
                            item.Code == line.Key
                            && item.IsActive
                            && item.Product != null
                            && item.Product.IsActive,
                        cancellationToken);
            }

            if (variant is null)
            {
                throw new OrderPlacementException(
                    $"Sản phẩm có mã {line.Key} không còn khả dụng.");
            }

            if (variant.Quantity < line.Quantity)
            {
                throw new OrderPlacementException(
                    $"Sản phẩm {variant.Product!.Name} chỉ còn {variant.Quantity} sản phẩm.");
            }

            resolvedLines.Add(new ResolvedOrderLine(variant, line.Quantity));
        }

        return resolvedLines;
    }

    private async Task<string> GenerateOrderCodeAsync(
        DateTime now,
        CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var suffix = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var orderCode = $"ORD-{now:yyyyMMdd}-{suffix}";
            var exists = await dbContext.Orders
                .AsNoTracking()
                .AnyAsync(order => order.OrderCode == orderCode, cancellationToken);

            if (!exists)
            {
                return orderCode;
            }
        }

        throw new OrderPlacementException(
            "Không thể tạo mã đơn hàng. Vui lòng thử lại.");
    }

    private static string NormalizeText(string value)
    {
        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        return builder
            .ToString()
            .Normalize(NormalizationForm.FormC)
            .Replace('đ', 'd')
            .Replace('Đ', 'D')
            .Replace(" ", string.Empty)
            .ToLowerInvariant();
    }

    private static bool ContainsAny(string value, params string[] keywords)
    {
        return keywords.Any(keyword =>
            value.Contains(
                keyword.Replace(" ", string.Empty),
                StringComparison.OrdinalIgnoreCase));
    }

    private sealed record ResolvedOrderLine(
        ProductVariant Variant,
        int Quantity);
}
