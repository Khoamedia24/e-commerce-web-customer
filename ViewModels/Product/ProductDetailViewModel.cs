using System.Globalization;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.ViewModels.Product;

public sealed class ProductDetailViewModel
{
    public required string Slug { get; init; }
    public string CartItemId { get; init; } = string.Empty;
    public string SelectedVariantLabel { get; init; } = string.Empty;
    public required string Name { get; init; }
    public required string Brand { get; init; }
    public required string MainImageUrl { get; init; }
    public required string MainImageAlt { get; init; }
    public decimal CurrentPrice { get; init; }
    public decimal? OldPrice { get; init; }
    public decimal Rating { get; init; }
    public int ReviewCount { get; init; }
    public required IReadOnlyList<ProductDetailBreadcrumbViewModel> Breadcrumbs { get; init; }
    public required IReadOnlyList<ProductDetailActionLinkViewModel> QuickLinks { get; init; }
    public required IReadOnlyList<ProductDetailGalleryItemViewModel> GalleryItems { get; init; }
    public required IReadOnlyList<ProductDetailStorageOptionViewModel> StorageOptions { get; init; }
    public required IReadOnlyList<ProductDetailColorOptionViewModel> ColorOptions { get; init; }
    public required IReadOnlyList<ProductTechnicalSpecSectionViewModel> TechnicalSpecSections { get; init; }
    public required IReadOnlyList<ProductRelatedProductGroupViewModel> RelatedProductGroups { get; init; }
    public required ProductReviewSummaryViewModel ReviewSummary { get; init; }
    public required QuestionAnswerSectionViewModel QuestionAnswerSection { get; init; }

    public string RatingLabel => Rating.ToString("0.#", CultureInfo.InvariantCulture);

    public static string FormatPrice(decimal price)
    {
        return string.Create(CultureInfo.GetCultureInfo("vi-VN"), $"{price:N0}đ");
    }
}

public sealed class ProductDetailBreadcrumbViewModel
{
    public required string Label { get; init; }
    public string? Url { get; init; }
}

public sealed class ProductDetailActionLinkViewModel
{
    public required string Label { get; init; }
    public required string IconId { get; init; }
    public string Url { get; init; } = "#";
}

public sealed class ProductDetailGalleryItemViewModel
{
    public required string Label { get; init; }
    public required string ImageUrl { get; init; }
    public required string ImageAlt { get; init; }
    public bool IsFeatureHighlight { get; init; }
}

public sealed class ProductDetailStorageOptionViewModel
{
    public required string Label { get; init; }
    public required string Url { get; init; }
    public bool IsActive { get; init; }
    public bool IsInitiallyHidden { get; init; }
}

public sealed class ProductDetailColorOptionViewModel
{
    public string VariantKey { get; init; } = string.Empty;
    public string DetailUrl { get; init; } = string.Empty;
    public string VariantLabel { get; init; } = string.Empty;
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
    public required string ImageAlt { get; init; }
    public decimal Price { get; init; }
    public bool IsActive { get; init; }
}

public sealed class ProductTechnicalSpecSectionViewModel
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required IReadOnlyList<ProductTechnicalSpecRowViewModel> Rows { get; init; }
}

public sealed class ProductTechnicalSpecRowViewModel
{
    public required string Label { get; init; }
    public required string Value { get; init; }
    public bool IsHighlighted { get; init; }
}

public sealed class ProductRelatedProductGroupViewModel
{
    public required string Id { get; init; }
    public required string Label { get; init; }
    public bool IsActive { get; init; }
    public required IReadOnlyList<ProductRelatedProductViewModel> Products { get; init; }
}

public sealed class ProductRelatedProductViewModel
{
    public required string Url { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
    public required string ImageAlt { get; init; }
    public decimal CurrentPrice { get; init; }
    public decimal? OldPrice { get; init; }
    public string? DiscountLabel { get; init; }
    public string? InstallmentLabel { get; init; }
    public string? GiftNote { get; init; }
    public string? DeliveryLabel { get; init; }
    public string? Location { get; init; }
    public decimal? Rating { get; init; }
    public string? UsedPriceLabel { get; init; }
    public string? SavingLabel { get; init; }
}

public sealed class ProductReviewSummaryViewModel
{
    public required string Title { get; init; }
    public decimal Score { get; init; }
    public int TotalReviews { get; init; }
    public required IReadOnlyList<ProductRatingBreakdownViewModel> RatingBreakdown { get; init; }
    public required IReadOnlyList<ProductExperienceRatingViewModel> ExperienceRatings { get; init; }
    public required IReadOnlyList<ProductReviewViewModel> Reviews { get; init; }

    public string ScoreLabel => Score.ToString("0.0", CultureInfo.InvariantCulture);
}

public sealed class ProductRatingBreakdownViewModel
{
    public int Stars { get; init; }
    public int Count { get; init; }
    public int Percent { get; init; }
}

public sealed class ProductExperienceRatingViewModel
{
    public required string Label { get; init; }
    public decimal Score { get; init; }
    public int Count { get; init; }

    public string ScoreLabel => Score.ToString("0.#", CultureInfo.InvariantCulture);
}

public sealed class ProductReviewViewModel
{
    public required string Author { get; init; }
    public required string Initial { get; init; }
    public decimal Rating { get; init; }
    public required string RatingText { get; init; }
    public required string Content { get; init; }
    public required string TimeAgo { get; init; }
    public IReadOnlyList<string> Tags { get; init; } = [];

    public string RatingLabel => Rating.ToString("0.#", CultureInfo.InvariantCulture);
}
