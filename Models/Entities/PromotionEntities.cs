using e_commerce_web_customer.Models.Enums;

namespace e_commerce_web_customer.Models.Entities;

public class Voucher
{
    public long Id { get; set; }

    // Ma voucher thuc te, vi du: SUMMER2026, FREESHIP-05.
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DiscountType DiscountType { get; set; } = DiscountType.FixedAmount;
    public decimal DiscountValue { get; set; }
    public decimal MinOrderValue { get; set; }
    public decimal? MaxDiscountValue { get; set; }
    public int? MaxUses { get; set; }
    public int? MaxUsesPerUser { get; set; }
    public int UsedCount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Priority { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<VoucherUser> VoucherUsers { get; set; } = new List<VoucherUser>();
    public ICollection<VoucherUsage> VoucherUsages { get; set; } = new List<VoucherUsage>();
    public ICollection<VoucherTarget> VoucherTargets { get; set; } = new List<VoucherTarget>();
}

public class VoucherUser
{
    public long Id { get; set; }
    public long VoucherId { get; set; }
    public long UserId { get; set; }
    public int MaxUses { get; set; } = 1;
    public int UsedCount { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public Voucher? Voucher { get; set; }
    public User? User { get; set; }
}

public class VoucherUsage
{
    public long Id { get; set; }
    public long VoucherId { get; set; }
    public long UserId { get; set; }
    public long OrderId { get; set; }
    public DateTime UsedAt { get; set; } = DateTime.UtcNow;

    public Voucher? Voucher { get; set; }
    public User? User { get; set; }
    public Order? Order { get; set; }
}

public class VoucherTarget
{
    public long Id { get; set; }
    public long VoucherId { get; set; }
    public TargetType TargetType { get; set; }

    // Polymorphic id: co the tro den product, product_variant, category, brand...
    public long TargetId { get; set; }

    public Voucher? Voucher { get; set; }
}

public class Campaign
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public CampaignType Type { get; set; } = CampaignType.Banner;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<CampaignCategory> CampaignCategories { get; set; } = new List<CampaignCategory>();
}

public class CampaignCategory
{
    public long Id { get; set; }
    public long CampaignId { get; set; }
    public long CategoryId { get; set; }
    public int Position { get; set; }
    public string? ImagePath { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }

    public Campaign? Campaign { get; set; }
    public Category? Category { get; set; }
}

public class Promotion
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Priority { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal MinOrderValue { get; set; }
    public decimal? MaxDiscountValue { get; set; }
    public int? UsageLimit { get; set; }
    public int UsedCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<PromotionTarget> PromotionTargets { get; set; } = new List<PromotionTarget>();
    public ICollection<PromotionRule> PromotionRules { get; set; } = new List<PromotionRule>();
}

public class PromotionTarget
{
    public long Id { get; set; }
    public long PromotionId { get; set; }
    public TargetType TargetType { get; set; }

    // Polymorphic id: co the tro den product, product_variant, category, brand...
    public long TargetId { get; set; }

    public Promotion? Promotion { get; set; }
}

public class PromotionRule
{
    public long Id { get; set; }
    public long PromotionId { get; set; }
    public long? GiftProductVariantId { get; set; }
    public PromotionActionType ActionType { get; set; } = PromotionActionType.DiscountOrder;
    public decimal DiscountValue { get; set; }
    public int BuyQuantity { get; set; }
    public int GetQuantity { get; set; }

    public Promotion? Promotion { get; set; }
    public ProductVariant? GiftProductVariant { get; set; }
}
