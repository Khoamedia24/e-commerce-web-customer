namespace e_commerce_web_customer.Models.Entities;

public class Brand
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Category
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long? ParentId { get; set; }
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
    public string Slug { get; set; } = string.Empty;
    public int Position { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<CategorySpecification> CategorySpecifications { get; set; } = new List<CategorySpecification>();
    public ICollection<CategoryVariantAttribute> CategoryVariantAttributes { get; set; } = new List<CategoryVariantAttribute>();
    public ICollection<CampaignCategory> CampaignCategories { get; set; } = new List<CampaignCategory>();
}

public class Product
{
    public long Id { get; set; }
    public long BrandId { get; set; }
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Slug { get; set; } = string.Empty;
    public int ViewsCount { get; set; }
    public int TotalSoldCount { get; set; }
    public decimal RatingAverage { get; set; }
    public int RatingCount { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Brand? Brand { get; set; }
    public Category? Category { get; set; }
    public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    public ICollection<ProductSpecification> ProductSpecifications { get; set; } = new List<ProductSpecification>();
}

public class ProductVariant
{
    public long Id { get; set; }
    public long ProductId { get; set; }

    // Ma SKU/ma bien the thuc te, vi du: IP15PM-256-BLK, NIKE-AF1-42-WHT.
    public string Code { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int SoldCount { get; set; }
    public int Quantity { get; set; }
    public string? ColorName { get; set; }
    public string? ColorHex { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Product? Product { get; set; }
    public ICollection<VariantAttribute> VariantAttributes { get; set; } = new List<VariantAttribute>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<GoodReceiptItem> GoodReceiptItems { get; set; } = new List<GoodReceiptItem>();
    public ICollection<PromotionRule> GiftPromotionRules { get; set; } = new List<PromotionRule>();
    public ICollection<ProductVariantImage> ProductVariantImages { get; set; } = new List<ProductVariantImage>();
}

public class ProductVariantImage
{
    public long Id { get; set; }
    public long ProductVariantId { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public int Position { get; set; }
    public ProductVariant? ProductVariant { get; set; }
}

public class Specification
{
    public long Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public string? Icon { get; set; }

    public ICollection<CategorySpecification> CategorySpecifications { get; set; } = new List<CategorySpecification>();
    public ICollection<ProductSpecification> ProductSpecifications { get; set; } = new List<ProductSpecification>();
}

public class CategorySpecification
{
    public long SpecificationId { get; set; }
    public long CategoryId { get; set; }
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }
    public string? GroupName { get; set; }

    public Specification? Specification { get; set; }
    public Category? Category { get; set; }
}

public class ProductSpecification
{
    public long ProductId { get; set; }
    public long SpecificationId { get; set; }
    public string Value { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsHighlight { get; set; }

    public Product? Product { get; set; }
    public Specification? Specification { get; set; }
}

public class Attribute
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public ICollection<AttributeOption> AttributeOptions { get; set; } = new List<AttributeOption>();
    public ICollection<CategoryVariantAttribute> CategoryVariantAttributes { get; set; } = new List<CategoryVariantAttribute>();
}

public class AttributeOption
{
    public long Id { get; set; }
    public long AttributeId { get; set; }
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;

    public Attribute? Attribute { get; set; }
    public ICollection<VariantAttribute> VariantAttributes { get; set; } = new List<VariantAttribute>();
}

public class CategoryVariantAttribute
{
    public long AttributeId { get; set; }
    public long CategoryId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Attribute? Attribute { get; set; }
    public Category? Category { get; set; }
}

public class VariantAttribute
{
    public long ProductVariantId { get; set; }
    public long AttributeOptionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ProductVariant? ProductVariant { get; set; }
    public AttributeOption? AttributeOption { get; set; }
}
