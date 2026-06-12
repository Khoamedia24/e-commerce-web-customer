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
}

public sealed class HeaderViewModel
{
    public required SiteCategoryMenuViewModel CategoryMenu { get; init; }
    public int CartItemCount { get; init; }
    public bool IsLoggedIn { get; init; }
}
