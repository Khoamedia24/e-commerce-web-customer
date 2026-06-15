namespace e_commerce_web_customer.Models.Enums;

public enum Gender
{
    Unknown = 0,
    Male = 1,
    Female = 2,
    Other = 3
}

public enum UserRole
{
    Customer = 0,
    Staff = 1,
    Manager = 2,
    Admin = 3
}

public enum AddressType
{
    Shipping = 0,
    Billing = 1,
    Other = 2
}

public enum DiscountType
{
    FixedAmount = 0,
    Percentage = 1
}

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Processing = 2,
    Shipping = 3,
    Completed = 4,
    Cancelled = 5,
    Returned = 6
}

public enum PaymentStatus
{
    Unpaid = 0,
    Paid = 1,
    Failed = 2,
    Refunded = 3
}

public enum CampaignType
{
    Banner = 0,
    Category = 1,
    FlashSale = 2,
    Seasonal = 3
}

public enum TargetType
{
    Product = 0,
    ProductVariant = 1,
    Category = 2,
    Brand = 3,
    User = 4
}

public enum PromotionActionType
{
    DiscountOrder = 0,
    DiscountProduct = 1,
    BuyXGetY = 2,
    GiftProduct = 3
}

public enum GoodsReceiptStatus
{
    Draft = 0,
    Pending = 1,
    Approved = 2,
    Cancelled = 3
}
