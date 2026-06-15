using e_commerce_web_customer.Models.Enums;

namespace e_commerce_web_customer.Models.Entities;

public class PaymentMethod
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

public class Order
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long PaymentMethodId { get; set; }
    public long? VoucherId { get; set; }

    // Ma don hang thuc te, vi du: ORD-20260527-000001.
    public string OrderCode { get; set; } = string.Empty;
    public long? ShippingAddressId { get; set; }
    public string ShippingContactName { get; set; } = string.Empty;
    public string ShippingPhone { get; set; } = string.Empty;
    public string ShippingProvince { get; set; } = string.Empty;
    public string ShippingWard { get; set; } = string.Empty;
    public string ShippingDetail { get; set; } = string.Empty;
    public decimal SubtotalAmount { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal VoucherDiscount { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public User? User { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public Voucher? Voucher { get; set; }
    public UserAddress? ShippingAddress { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<VoucherUsage> VoucherUsages { get; set; } = new List<VoucherUsage>();
}

public class OrderItem
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public long ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public Order? Order { get; set; }
    public ProductVariant? ProductVariant { get; set; }
    public Rating? Rating { get; set; }
}

public class Rating
{
    public long Id { get; set; }
    public long OrderItemId { get; set; }
    public long UserId { get; set; }
    public int Stars { get; set; }
    public string? Comment { get; set; }
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public OrderItem? OrderItem { get; set; }
    public User? User { get; set; }
}
