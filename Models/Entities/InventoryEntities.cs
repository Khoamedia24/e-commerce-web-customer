using e_commerce_web_customer.Models.Enums;

namespace e_commerce_web_customer.Models.Entities;

public class Supplier
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();
}

public class GoodsReceipt
{
    public long Id { get; set; }
    public long SupplierId { get; set; }
    public string ReceiptCode { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public GoodsReceiptStatus Status { get; set; } = GoodsReceiptStatus.Draft;
    public long CreatedBy { get; set; }
    public long? ApprovedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Supplier? Supplier { get; set; }
    public User? CreatedByUser { get; set; }
    public User? ApprovedByUser { get; set; }
    public ICollection<GoodReceiptItem> GoodReceiptItems { get; set; } = new List<GoodReceiptItem>();
}

public class GoodReceiptItem
{
    public long Id { get; set; }
    public long GoodsReceiptId { get; set; }
    public long ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public decimal ImportPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public GoodsReceipt? GoodsReceipt { get; set; }
    public ProductVariant? ProductVariant { get; set; }
}
