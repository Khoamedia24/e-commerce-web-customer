namespace e_commerce_web_customer.Application.Products;

public sealed record ProductReadModel(
    string Id,
    string Name,
    string ProductUrl,
    string ImageUrl,
    string ImageAlt,
    decimal CurrentPrice,
    decimal? OldPrice,
    int DiscountPercent,
    decimal? StudentPrice,
    string? PromotionNote,
    string SearchText,
    IReadOnlyList<string>? Aliases = null,
    string? CategorySlug = null,
    string? CategoryName = null,
    string? AvailabilityLabel = null,
    decimal? Rating = null,
    IReadOnlyList<string>? Specifications = null,
    int PopularityScore = 0);
