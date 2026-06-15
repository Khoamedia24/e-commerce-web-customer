using e_commerce_web_customer.Models.Entities;
using Microsoft.EntityFrameworkCore;
using AttributeEntity = e_commerce_web_customer.Models.Entities.Attribute;

namespace e_commerce_web_customer.Data;

public class EcommerceDbContext : DbContext
{
    public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserAddress> UserAddresses => Set<UserAddress>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductVariantImage> ProductVariantImages => Set<ProductVariantImage>();
    public DbSet<Specification> Specifications => Set<Specification>();
    public DbSet<CategorySpecification> CategorySpecifications => Set<CategorySpecification>();
    public DbSet<ProductSpecification> ProductSpecifications => Set<ProductSpecification>();
    public DbSet<AttributeEntity> Attributes => Set<AttributeEntity>();
    public DbSet<AttributeOption> AttributeOptions => Set<AttributeOption>();
    public DbSet<CategoryVariantAttribute> CategoryVariantAttributes => Set<CategoryVariantAttribute>();
    public DbSet<VariantAttribute> VariantAttributes => Set<VariantAttribute>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<Voucher> Vouchers => Set<Voucher>();
    public DbSet<VoucherUser> VoucherUsers => Set<VoucherUser>();
    public DbSet<VoucherUsage> VoucherUsages => Set<VoucherUsage>();
    public DbSet<VoucherTarget> VoucherTargets => Set<VoucherTarget>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<CampaignCategory> CampaignCategories => Set<CampaignCategory>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<PromotionTarget> PromotionTargets => Set<PromotionTarget>();
    public DbSet<PromotionRule> PromotionRules => Set<PromotionRule>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<GoodsReceipt> GoodsReceipts => Set<GoodsReceipt>();
    public DbSet<GoodReceiptItem> GoodReceiptItems => Set<GoodReceiptItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUsers(modelBuilder);
        ConfigureCatalog(modelBuilder);
        ConfigureOrders(modelBuilder);
        ConfigureMarketing(modelBuilder);
        ConfigureInventory(modelBuilder);

        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(entity => entity.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }

    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasIndex(user => user.Username).IsUnique();
            entity.HasIndex(user => user.Email).IsUnique();
            entity.Property(user => user.Username).HasMaxLength(100).IsRequired();
            entity.Property(user => user.Email).HasMaxLength(255).IsRequired();
            entity.Property(user => user.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(user => user.FullName).HasMaxLength(255).IsRequired();
            entity.Property(user => user.Phone).HasMaxLength(30);
            entity.Property(user => user.Gender).HasConversion<string>().HasMaxLength(30);
            entity.Property(user => user.Role).HasConversion<string>().HasMaxLength(30);
            entity.Property(user => user.AvatarImage).HasMaxLength(500);
        });

        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity.ToTable("user_addresses");
            entity.HasIndex(address => new { address.UserId, address.IsDefault });
            entity.HasIndex(address => new { address.UserId, address.IsDeleted });
            entity.Property(address => address.ContactName).HasMaxLength(255).IsRequired();
            entity.Property(address => address.Phone).HasMaxLength(30).IsRequired();
            entity.Property(address => address.ProvinceCode).HasMaxLength(30).IsRequired();
            entity.Property(address => address.ProvinceName).HasMaxLength(120).IsRequired();
            entity.Property(address => address.WardCode).HasMaxLength(30).IsRequired();
            entity.Property(address => address.WardName).HasMaxLength(120).IsRequired();
            entity.Property(address => address.DetailAddress).HasMaxLength(500).IsRequired();
            entity.Property(address => address.Type).HasConversion<string>().HasMaxLength(30);
            entity.Property(address => address.IsDeleted).HasDefaultValue(false);
            entity.HasOne(address => address.User)
                .WithMany(user => user.Addresses)
                .HasForeignKey(address => address.UserId);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.ToTable("cart_items");
            entity.HasIndex(item => new { item.UserId, item.ProductVariantId }).IsUnique();
            entity.Property(item => item.UnitPrice).HasPrecision(18, 2);
            entity.Property(item => item.DiscountValue).HasPrecision(18, 2);
            entity.HasOne(item => item.User)
                .WithMany(user => user.CartItems)
                .HasForeignKey(item => item.UserId);
            entity.HasOne(item => item.ProductVariant)
                .WithMany(variant => variant.CartItems)
                .HasForeignKey(item => item.ProductVariantId);
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.ToTable("wishlist");
            entity.HasIndex(item => new { item.UserId, item.ProductVariantId }).IsUnique();
            entity.HasOne(item => item.User)
                .WithMany(user => user.Wishlists)
                .HasForeignKey(item => item.UserId);
            entity.HasOne(item => item.ProductVariant)
                .WithMany(variant => variant.Wishlists)
                .HasForeignKey(item => item.ProductVariantId);
        });
    }

    private static void ConfigureCatalog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.ToTable("brands");
            entity.HasIndex(brand => brand.Slug).IsUnique();
            entity.Property(brand => brand.Name).HasMaxLength(255).IsRequired();
            entity.Property(brand => brand.Description).HasMaxLength(1000);
            entity.Property(brand => brand.ImagePath).HasMaxLength(500);
            entity.Property(brand => brand.Slug).HasMaxLength(255).IsRequired();
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");
            entity.HasIndex(category => category.Slug).IsUnique();
            entity.Property(category => category.Name).HasMaxLength(255).IsRequired();
            entity.Property(category => category.Description).HasMaxLength(1000);
            entity.Property(category => category.ImagePath).HasMaxLength(500);
            entity.Property(category => category.Slug).HasMaxLength(255).IsRequired();
            entity.HasOne(category => category.Parent)
                .WithMany(category => category.Children)
                .HasForeignKey(category => category.ParentId);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");
            entity.HasIndex(product => product.Slug).IsUnique();
            entity.Property(product => product.Name).HasMaxLength(255).IsRequired();
            entity.Property(product => product.Description).HasMaxLength(4000);
            entity.Property(product => product.Slug).HasMaxLength(255).IsRequired();
            entity.Property(product => product.RatingAverage).HasPrecision(3, 2);
            entity.HasOne(product => product.Brand)
                .WithMany(brand => brand.Products)
                .HasForeignKey(product => product.BrandId);
            entity.HasOne(product => product.Category)
                .WithMany(category => category.Products)
                .HasForeignKey(product => product.CategoryId);
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.ToTable("product_variants");
            entity.HasIndex(variant => variant.Code).IsUnique();
            entity.Property(variant => variant.Code).HasMaxLength(80).IsRequired();
            entity.Property(variant => variant.Price).HasPrecision(18, 2);
            entity.Property(variant => variant.ColorName).HasMaxLength(120);
            entity.Property(variant => variant.ColorHex).HasMaxLength(7).IsUnicode(false);
            entity.HasOne(variant => variant.Product)
                .WithMany(product => product.ProductVariants)
                .HasForeignKey(variant => variant.ProductId);
        });

        modelBuilder.Entity<ProductVariantImage>(entity =>
        {
            entity.ToTable("product_variant_images");
            entity.Property(image => image.ImagePath).HasMaxLength(500).IsRequired();
            entity.Property(image => image.AltText).HasMaxLength(255);
            entity.HasOne(image => image.ProductVariant)
                .WithMany(variant => variant.ProductVariantImages)
                .HasForeignKey(image => image.ProductVariantId);
        });

        modelBuilder.Entity<Specification>(entity =>
        {
            entity.ToTable("specifications");
            entity.HasIndex(specification => specification.Key).IsUnique();
            entity.Property(specification => specification.Key).HasMaxLength(100).IsRequired();
            entity.Property(specification => specification.Name).HasMaxLength(255).IsRequired();
            entity.Property(specification => specification.Unit).HasMaxLength(50);
            entity.Property(specification => specification.Icon).HasMaxLength(100);
        });

        modelBuilder.Entity<CategorySpecification>(entity =>
        {
            entity.ToTable("category_specifications");
            entity.HasKey(item => new { item.CategoryId, item.SpecificationId });
            entity.Property(item => item.GroupName).HasMaxLength(120);
            entity.HasOne(item => item.Category)
                .WithMany(category => category.CategorySpecifications)
                .HasForeignKey(item => item.CategoryId);
            entity.HasOne(item => item.Specification)
                .WithMany(specification => specification.CategorySpecifications)
                .HasForeignKey(item => item.SpecificationId);
        });

        modelBuilder.Entity<ProductSpecification>(entity =>
        {
            entity.ToTable("product_specifications");
            entity.HasKey(item => new { item.ProductId, item.SpecificationId });
            entity.Property(item => item.Value).HasMaxLength(1000).IsRequired();
            entity.HasOne(item => item.Product)
                .WithMany(product => product.ProductSpecifications)
                .HasForeignKey(item => item.ProductId);
            entity.HasOne(item => item.Specification)
                .WithMany(specification => specification.ProductSpecifications)
                .HasForeignKey(item => item.SpecificationId);
        });

        modelBuilder.Entity<AttributeEntity>(entity =>
        {
            entity.ToTable("attributes");
            entity.HasIndex(attribute => attribute.Code).IsUnique();
            entity.Property(attribute => attribute.Code).HasMaxLength(80).IsRequired();
            entity.Property(attribute => attribute.Name).HasMaxLength(255).IsRequired();
        });

        modelBuilder.Entity<AttributeOption>(entity =>
        {
            entity.ToTable("attribute_options");
            entity.HasIndex(option => new { option.AttributeId, option.Value }).IsUnique();
            entity.Property(option => option.Value).HasMaxLength(120).IsRequired();
            entity.Property(option => option.Label).HasMaxLength(255).IsRequired();
            entity.HasOne(option => option.Attribute)
                .WithMany(attribute => attribute.AttributeOptions)
                .HasForeignKey(option => option.AttributeId);
        });

        modelBuilder.Entity<CategoryVariantAttribute>(entity =>
        {
            entity.ToTable("category_variant_attributes");
            entity.HasKey(item => new { item.CategoryId, item.AttributeId });
            entity.HasOne(item => item.Category)
                .WithMany(category => category.CategoryVariantAttributes)
                .HasForeignKey(item => item.CategoryId);
            entity.HasOne(item => item.Attribute)
                .WithMany(attribute => attribute.CategoryVariantAttributes)
                .HasForeignKey(item => item.AttributeId);
        });

        modelBuilder.Entity<VariantAttribute>(entity =>
        {
            entity.ToTable("variant_attributes");
            entity.HasKey(item => new { item.ProductVariantId, item.AttributeOptionId });
            entity.HasOne(item => item.ProductVariant)
                .WithMany(variant => variant.VariantAttributes)
                .HasForeignKey(item => item.ProductVariantId);
            entity.HasOne(item => item.AttributeOption)
                .WithMany(option => option.VariantAttributes)
                .HasForeignKey(item => item.AttributeOptionId);
        });
    }

    private static void ConfigureOrders(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.ToTable("payment_methods");
            entity.Property(method => method.Name).HasMaxLength(120).IsRequired();
            entity.Property(method => method.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasIndex(order => order.OrderCode).IsUnique();
            entity.Property(order => order.OrderCode).HasMaxLength(50).IsRequired();
            entity.Property(order => order.ShippingContactName).HasMaxLength(255).IsRequired();
            entity.Property(order => order.ShippingPhone).HasMaxLength(30).IsRequired();
            entity.Property(order => order.ShippingProvince).HasMaxLength(120).IsRequired();
            entity.Property(order => order.ShippingWard).HasMaxLength(120).IsRequired();
            entity.Property(order => order.ShippingDetail).HasMaxLength(500).IsRequired();
            entity.Property(order => order.SubtotalAmount).HasPrecision(18, 2);
            entity.Property(order => order.ShippingFee).HasPrecision(18, 2);
            entity.Property(order => order.VoucherDiscount).HasPrecision(18, 2);
            entity.Property(order => order.TotalAmount).HasPrecision(18, 2);
            entity.Property(order => order.OrderStatus).HasConversion<string>().HasMaxLength(30);
            entity.Property(order => order.PaymentStatus).HasConversion<string>().HasMaxLength(30);
            entity.HasOne(order => order.User)
                .WithMany(user => user.Orders)
                .HasForeignKey(order => order.UserId);
            entity.HasOne(order => order.PaymentMethod)
                .WithMany(method => method.Orders)
                .HasForeignKey(order => order.PaymentMethodId);
            entity.HasOne(order => order.Voucher)
                .WithMany(voucher => voucher.Orders)
                .HasForeignKey(order => order.VoucherId);
            entity.HasOne(order => order.ShippingAddress)
                .WithMany(address => address.Orders)
                .HasForeignKey(order => order.ShippingAddressId);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("order_items");
            entity.Property(item => item.UnitPrice).HasPrecision(18, 2);
            entity.HasOne(item => item.Order)
                .WithMany(order => order.OrderItems)
                .HasForeignKey(item => item.OrderId);
            entity.HasOne(item => item.ProductVariant)
                .WithMany(variant => variant.OrderItems)
                .HasForeignKey(item => item.ProductVariantId);
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.ToTable("ratings");
            entity.HasIndex(rating => rating.OrderItemId).IsUnique();
            entity.Property(rating => rating.Comment).HasMaxLength(1000);
            entity.HasOne(rating => rating.OrderItem)
                .WithOne(orderItem => orderItem.Rating)
                .HasForeignKey<Rating>(rating => rating.OrderItemId);
            entity.HasOne(rating => rating.User)
                .WithMany(user => user.Ratings)
                .HasForeignKey(rating => rating.UserId);
        });
    }

    private static void ConfigureMarketing(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.ToTable("vouchers");
            entity.HasIndex(voucher => voucher.Code).IsUnique();
            entity.Property(voucher => voucher.Code).HasMaxLength(80).IsRequired();
            entity.Property(voucher => voucher.Description).HasMaxLength(1000);
            entity.Property(voucher => voucher.DiscountType).HasConversion<string>().HasMaxLength(30);
            entity.Property(voucher => voucher.DiscountValue).HasPrecision(18, 2);
            entity.Property(voucher => voucher.MinOrderValue).HasPrecision(18, 2);
            entity.Property(voucher => voucher.MaxDiscountValue).HasPrecision(18, 2);
        });

        modelBuilder.Entity<VoucherUser>(entity =>
        {
            entity.ToTable("voucher_users");
            entity.HasIndex(item => new { item.VoucherId, item.UserId }).IsUnique();
            entity.HasOne(item => item.Voucher)
                .WithMany(voucher => voucher.VoucherUsers)
                .HasForeignKey(item => item.VoucherId);
            entity.HasOne(item => item.User)
                .WithMany(user => user.VoucherUsers)
                .HasForeignKey(item => item.UserId);
        });

        modelBuilder.Entity<VoucherUsage>(entity =>
        {
            entity.ToTable("voucher_usages");
            entity.HasIndex(item => item.OrderId).IsUnique();
            entity.HasOne(item => item.Voucher)
                .WithMany(voucher => voucher.VoucherUsages)
                .HasForeignKey(item => item.VoucherId);
            entity.HasOne(item => item.User)
                .WithMany(user => user.VoucherUsages)
                .HasForeignKey(item => item.UserId);
            entity.HasOne(item => item.Order)
                .WithMany(order => order.VoucherUsages)
                .HasForeignKey(item => item.OrderId);
        });

        modelBuilder.Entity<VoucherTarget>(entity =>
        {
            entity.ToTable("voucher_targets");
            entity.HasIndex(item => new { item.VoucherId, item.TargetType, item.TargetId }).IsUnique();
            entity.Property(item => item.TargetType).HasConversion<string>().HasMaxLength(50);
            entity.HasOne(item => item.Voucher)
                .WithMany(voucher => voucher.VoucherTargets)
                .HasForeignKey(item => item.VoucherId);
        });

        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.ToTable("campaigns");
            entity.HasIndex(campaign => campaign.Slug).IsUnique();
            entity.Property(campaign => campaign.Name).HasMaxLength(255).IsRequired();
            entity.Property(campaign => campaign.Slug).HasMaxLength(255).IsRequired();
            entity.Property(campaign => campaign.Type).HasConversion<string>().HasMaxLength(30);
            entity.Property(campaign => campaign.Description).HasMaxLength(1000);
        });

        modelBuilder.Entity<CampaignCategory>(entity =>
        {
            entity.ToTable("campaign_categories");
            entity.Property(item => item.ImagePath).HasMaxLength(500);
            entity.Property(item => item.Title).HasMaxLength(255);
            entity.Property(item => item.Description).HasMaxLength(1000);
            entity.HasOne(item => item.Campaign)
                .WithMany(campaign => campaign.CampaignCategories)
                .HasForeignKey(item => item.CampaignId);
            entity.HasOne(item => item.Category)
                .WithMany(category => category.CampaignCategories)
                .HasForeignKey(item => item.CategoryId);
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.ToTable("promotions");
            entity.Property(promotion => promotion.Name).HasMaxLength(255).IsRequired();
            entity.Property(promotion => promotion.Description).HasMaxLength(1000);
            entity.Property(promotion => promotion.MinOrderValue).HasPrecision(18, 2);
            entity.Property(promotion => promotion.MaxDiscountValue).HasPrecision(18, 2);
        });

        modelBuilder.Entity<PromotionTarget>(entity =>
        {
            entity.ToTable("promotion_targets");
            entity.HasIndex(item => new { item.PromotionId, item.TargetType, item.TargetId }).IsUnique();
            entity.Property(item => item.TargetType).HasConversion<string>().HasMaxLength(50);
            entity.HasOne(item => item.Promotion)
                .WithMany(promotion => promotion.PromotionTargets)
                .HasForeignKey(item => item.PromotionId);
        });

        modelBuilder.Entity<PromotionRule>(entity =>
        {
            entity.ToTable("promotion_rules");
            entity.Property(rule => rule.ActionType).HasConversion<string>().HasMaxLength(50);
            entity.Property(rule => rule.DiscountValue).HasPrecision(18, 2);
            entity.HasOne(rule => rule.Promotion)
                .WithMany(promotion => promotion.PromotionRules)
                .HasForeignKey(rule => rule.PromotionId);
            entity.HasOne(rule => rule.GiftProductVariant)
                .WithMany(variant => variant.GiftPromotionRules)
                .HasForeignKey(rule => rule.GiftProductVariantId);
        });
    }

    private static void ConfigureInventory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("suppliers");
            entity.Property(supplier => supplier.Name).HasMaxLength(255).IsRequired();
            entity.Property(supplier => supplier.Phone).HasMaxLength(30);
            entity.Property(supplier => supplier.Email).HasMaxLength(100);
            entity.Property(supplier => supplier.Address).HasMaxLength(500);
        });

        modelBuilder.Entity<GoodsReceipt>(entity =>
        {
            entity.ToTable("goods_receipts");
            entity.HasIndex(receipt => receipt.ReceiptCode).IsUnique();
            entity.Property(receipt => receipt.ReceiptCode).HasMaxLength(50).IsRequired();
            entity.Property(receipt => receipt.TotalAmount).HasPrecision(18, 2);
            entity.Property(receipt => receipt.Status).HasConversion<string>().HasMaxLength(30);
            entity.HasOne(receipt => receipt.Supplier)
                .WithMany(supplier => supplier.GoodsReceipts)
                .HasForeignKey(receipt => receipt.SupplierId);
            entity.HasOne(receipt => receipt.CreatedByUser)
                .WithMany(user => user.CreatedGoodsReceipts)
                .HasForeignKey(receipt => receipt.CreatedBy);
            entity.HasOne(receipt => receipt.ApprovedByUser)
                .WithMany(user => user.ApprovedGoodsReceipts)
                .HasForeignKey(receipt => receipt.ApprovedBy);
        });

        modelBuilder.Entity<GoodReceiptItem>(entity =>
        {
            entity.ToTable("good_receipt_items");
            entity.Property(item => item.ImportPrice).HasPrecision(18, 2);
            entity.HasOne(item => item.GoodsReceipt)
                .WithMany(receipt => receipt.GoodReceiptItems)
                .HasForeignKey(item => item.GoodsReceiptId);
            entity.HasOne(item => item.ProductVariant)
                .WithMany(variant => variant.GoodReceiptItems)
                .HasForeignKey(item => item.ProductVariantId);
        });
    }
}
