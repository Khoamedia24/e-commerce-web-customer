using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.ViewModels.Catalog;

public enum CategoryPageLayoutMode
{
    FilterListing,
    Sectioned
}

public sealed class CategoryPageViewModel
{
    public required string Slug { get; init; }
    public required string Title { get; init; }
    public required string MetaDescription { get; init; }
    public CategoryPageLayoutMode LayoutMode { get; init; }
    public required IReadOnlyList<CategoryBreadcrumbViewModel> Breadcrumbs { get; init; }
    public required IReadOnlyList<CategoryPromotionBannerViewModel> PromotionBanners { get; init; }
    public required IReadOnlyList<CategoryBrandViewModel> Brands { get; init; }
    public required IReadOnlyList<CategoryQuickLinkViewModel> QuickLinks { get; init; }
    public required CategoryHotSaleViewModel HotSale { get; init; }
    public required CategoryFilterViewModel Filter { get; init; }
    public required IReadOnlyList<ProductCardViewModel> Products { get; init; }
    public int InitialProductCount { get; init; } = 20;
    public IReadOnlyList<CategorySectionNavigationItemViewModel> SectionTabs { get; init; } = [];
    public IReadOnlyList<CategoryProductSectionViewModel> ProductSections { get; init; } = [];
    public required CategorySeoContentViewModel SeoContent { get; init; }
    public required QuestionAnswerSectionViewModel QuestionAnswer { get; init; }
}

public sealed class CategoryBreadcrumbViewModel
{
    public required string Label { get; init; }
    public required string Url { get; init; }
    public bool IsCurrent { get; init; }
}

public sealed class CategoryPromotionBannerViewModel
{
    public required string Id { get; init; }
    public required string Kicker { get; init; }
    public required string Title { get; init; }
    public required string PriceText { get; init; }
    public required string ImageUrl { get; init; }
    public required string ImageAlt { get; init; }
    public required string Url { get; init; }
    public string Theme { get; init; } = "light";
}

public sealed class CategoryBrandViewModel
{
    public required string Id { get; init; }
    public required string Label { get; init; }
    public required string Url { get; init; }
    public string? ImageUrl { get; init; }
    public string? ImageAlt { get; init; }
}

public sealed class CategoryQuickLinkViewModel
{
    public required string Label { get; init; }
    public required string Url { get; init; }
    public required string ImageUrl { get; init; }
    public required string ImageAlt { get; init; }
}

public sealed class CategoryHotSaleViewModel
{
    public required string Title { get; init; }
    public DateTimeOffset? EndsAt { get; init; }
    public required IReadOnlyList<ProductCardViewModel> Products { get; init; }
}

public sealed class CategoryFilterViewModel
{
    public required string Title { get; init; }
    public required IReadOnlyList<CategoryFilterItemViewModel> PrimaryItems { get; init; }
    public required IReadOnlyList<CategoryFilterItemViewModel> SecondaryItems { get; init; }
    public IReadOnlyList<CategoryFilterGroupViewModel> Groups { get; init; } = [];
    public required IReadOnlyList<CategorySortOptionViewModel> SortOptions { get; init; }
    public string CategorySlug { get; init; } = string.Empty;
    public string? Brand { get; init; }
    public string? Sort { get; init; }
    public int ActiveSelectionCount { get; init; }
    public int ResultCount { get; init; }
}

public sealed class CategoryFilterItemViewModel
{
    public required string Label { get; init; }
    public required string Url { get; init; }
    public string? Icon { get; init; }
    public bool HasDropdown { get; init; }
    public bool IsEmphasized { get; init; }
    public bool IsActive { get; init; }
}

public sealed class CategoryFilterGroupViewModel
{
    public required string Key { get; init; }
    public required string Label { get; init; }
    public string? Icon { get; init; }
    public required IReadOnlyList<CategoryFilterOptionViewModel> Options { get; init; }
    public int SelectedCount { get; init; }
}

public sealed class CategoryFilterOptionViewModel
{
    public required string Value { get; init; }
    public required string Label { get; init; }
    public bool IsSelected { get; init; }
    public bool IsAvailable { get; init; }
}

public sealed class CategorySortOptionViewModel
{
    public required string Label { get; init; }
    public required string Url { get; init; }
    public string? Icon { get; init; }
    public bool IsActive { get; init; }
}

public sealed class CategorySeoContentViewModel
{
    public required string Title { get; init; }
    public required IReadOnlyList<string> Paragraphs { get; init; }
}

public sealed class CategorySectionNavigationItemViewModel
{
    public required string Id { get; init; }
    public required string Label { get; init; }
    public required string Url { get; init; }
    public bool IsActive { get; init; }
}

public sealed class CategoryProductSectionViewModel
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required string ViewAllUrl { get; init; }
    public int VisibleProductLimit { get; init; } = 10;
    public CategorySectionBannerViewModel? Banner { get; init; }
    public IReadOnlyList<CategorySectionPillViewModel> Subcategories { get; init; } = [];
    public IReadOnlyList<CategorySortOptionViewModel> SortOptions { get; init; } = [];
    public required IReadOnlyList<ProductCardViewModel> Products { get; init; }
}

public sealed class CategorySectionBannerViewModel
{
    public required string Title { get; init; }
    public required string Subtitle { get; init; }
    public required string ImageUrl { get; init; }
    public required string ImageAlt { get; init; }
    public string Theme { get; init; } = "rose";
}

public sealed class CategorySectionPillViewModel
{
    public required string Label { get; init; }
    public required string Url { get; init; }
    public required string ImageUrl { get; init; }
    public required string ImageAlt { get; init; }
    public bool IsActive { get; init; }
}
