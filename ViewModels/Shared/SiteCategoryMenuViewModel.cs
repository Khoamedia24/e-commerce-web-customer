namespace e_commerce_web_customer.ViewModels.Shared;

public sealed class SiteCategoryMenuViewModel
{
    public required IReadOnlyList<SiteCategoryMenuItemViewModel> Items { get; init; }
}

public sealed class SiteCategoryMenuItemViewModel
{
    public required string Id { get; init; }
    public required string Url { get; init; }
    public required string Label { get; init; }
    public required string Icon { get; init; }
    public bool IsHighlighted { get; init; }
    public IReadOnlyList<SiteCategoryMenuLinkViewModel> CategoryLinks { get; init; } = [];
    public IReadOnlyList<SiteCategoryMenuGroupViewModel> Groups { get; init; } = [];
}

public sealed class SiteCategoryMenuGroupViewModel
{
    public required string Title { get; init; }
    public required IReadOnlyList<SiteCategoryMenuLinkViewModel> Links { get; init; }
}

public sealed class SiteCategoryMenuLinkViewModel
{
    public required string Label { get; init; }
    public required string Url { get; init; }
    public string? Badge { get; init; }
    public string? ImageUrl { get; init; }
    public string? ImageAlt { get; init; }
}

public sealed class SiteCategoryMegaPanelsViewModel
{
    public required string IdPrefix { get; init; }
    public required string StageClass { get; init; }
    public required IReadOnlyList<SiteCategoryMenuItemViewModel> Items { get; init; }
}

public sealed class HeaderViewModel
{
    public required SiteCategoryMenuViewModel CategoryMenu { get; init; }
    public required HeaderSearchViewModel Search { get; init; }
    public required HeaderAccountViewModel Account { get; init; }
    public int CartItemCount { get; init; }
    public bool IsLoggedIn { get; init; }
}
