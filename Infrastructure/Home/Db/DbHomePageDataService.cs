using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Data;
using e_commerce_web_customer.Infrastructure.Home.Content;
using e_commerce_web_customer.Models.Entities;
using Microsoft.EntityFrameworkCore;
using e_commerce_web_customer.ViewModels.Home;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Home.Db;

public sealed class DbHomePageDataService(EcommerceDbContext dbContext) : IHomePageDataService
{
    private const int MaxVariantsPerPanel = 20;
    private const string CategoryFallbackImage = "/images/logo-techstore-icon.svg";

    private static readonly string[] PhoneCategorySlugs =
    [
        "phone",
        "dien-thoai",
        "smartphone",
        "dien-thoai-di-dong"
    ];

    private static readonly string[] TabletCategorySlugs =
    [
        "tablet",
        "may-tinh-bang"
    ];

    private static readonly CategorySectionDefinition[] ComputerSectionDefinitions =
    [
        new("laptops", "Laptop", "laptop", true, true),
        new("desktop-pcs", "PC", "pc"),
        new("monitors", "Màn hình", "man-hinh"),
        new("computer-accessories", "Phụ kiện máy tính", "may-in")
    ];

    private static readonly CategorySectionDefinition[] AudioWearableSectionDefinitions =
    [
        new("watches", "Đồng hồ", "dong-ho", true),
        new("audio", "Âm thanh", "am-thanh")
    ];

    public async Task<HomeIndexViewModel> CreateHomePageAsync(
        SiteCategoryMenuViewModel categoryMenu,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var categories = await GetActiveCategoriesAsync(cancellationToken);
        var phoneTabletSection = await CreatePhoneTabletSectionAsync(
            categories,
            cancellationToken);
        var computerSection = await CreateCategorySectionAsync(
            "computer-products",
            rows: 2,
            ComputerSectionDefinitions,
            categories,
            cancellationToken);
        var audioWearableSection = await CreateCategorySectionAsync(
            "audio-wearable-products",
            rows: 1,
            AudioWearableSectionDefinitions,
            categories,
            cancellationToken);

        return new HomeIndexViewModel
        {
            Hero = HomeHeroViewModelFactory.Create(categoryMenu.Items),
            FeaturedCategorySections = [phoneTabletSection],
            AccessoryDirectory = new CategoryDirectoryViewModel
            {
                Id = "db-accessory-directory",
                Title = "Sắm thêm phụ kiện chất lượng",
                ViewAllUrl = "/catalog?cat=accessories",
                Items = []
            },
            AdditionalCategorySections =
            [
                computerSection,
                audioWearableSection
            ]
        };
    }

    private async Task<CategoryProductsViewModel> CreatePhoneTabletSectionAsync(
        IReadOnlyList<CategoryRecord> categories,
        CancellationToken cancellationToken)
    {
        var phoneProducts = await GetVariantCardsByCategoryAsync(
            PhoneCategorySlugs,
            categories,
            cancellationToken);
        var tabletProducts = await GetVariantCardsByCategoryAsync(
            TabletCategorySlugs,
            categories,
            cancellationToken);
        var phoneBrands = await GetBrandsByCategoryAsync(
            PhoneCategorySlugs,
            "phone",
            categories,
            cancellationToken);
        var tabletBrands = await GetBrandsByCategoryAsync(
            TabletCategorySlugs,
            "tablet",
            categories,
            cancellationToken);

        return new CategoryProductsViewModel
        {
            Id = "phone-products",
            Rows = 2,
            EnableTabSwitching = true,
            ShowPagination = false,
            Tabs =
            [
                new()
                {
                    Id = "phones",
                    Label = "Điện thoại",
                    Url = "/catalog?cat=phone",
                    IsActive = true,
                    Panel = new CategoryProductPanelViewModel
                    {
                        ViewAllUrl = "/catalog?cat=phone",
                        Banners = HomePhoneTabletSectionContent.CreatePhoneBanners(),
                        QuickLinks = HomePhoneTabletSectionContent.CreatePhoneQuickLinks(),
                        Brands = phoneBrands,
                        Products = phoneProducts
                    }
                },
                new()
                {
                    Id = "tablets",
                    Label = "Máy tính bảng",
                    Url = "/catalog?cat=tablet",
                    Panel = new CategoryProductPanelViewModel
                    {
                        ViewAllUrl = "/catalog?cat=tablet",
                        Banners = HomePhoneTabletSectionContent.CreateTabletBanners(),
                        QuickLinks = HomePhoneTabletSectionContent.CreateTabletQuickLinks(),
                        Brands = tabletBrands,
                        Products = tabletProducts
                    }
                }
            ]
        };
    }

    private async Task<CategoryProductsViewModel> CreateCategorySectionAsync(
        string sectionId,
        int rows,
        IReadOnlyList<CategorySectionDefinition> definitions,
        IReadOnlyList<CategoryRecord> categories,
        CancellationToken cancellationToken)
    {
        var tabs = new List<CategoryTabViewModel>(definitions.Count);

        foreach (var definition in definitions)
        {
            var categorySlugs = new[] { definition.CategorySlug };
            var products = await GetVariantCardsByCategoryAsync(
                categorySlugs,
                categories,
                cancellationToken);
            var brands = definition.ShowBrandFilter
                ? await GetBrandsByCategoryAsync(
                    categorySlugs,
                    definition.CategorySlug,
                    categories,
                    cancellationToken)
                : [];

            tabs.Add(new CategoryTabViewModel
            {
                Id = definition.Id,
                Label = definition.Label,
                Url = BuildCatalogUrl(definition.CategorySlug),
                IsActive = definition.IsActive,
                Panel = new CategoryProductPanelViewModel
                {
                    ViewAllUrl = BuildCatalogUrl(definition.CategorySlug),
                    Banners = HomeAdditionalCategorySectionContent.CreateBanners(
                        definition.CategorySlug),
                    QuickLinks = BuildLevelTwoCategoryLinks(
                        definition.CategorySlug,
                        categories),
                    Brands = brands,
                    Products = products
                }
            });
        }

        return new CategoryProductsViewModel
        {
            Id = sectionId,
            Rows = rows,
            EnableTabSwitching = true,
            ShowPagination = false,
            Tabs = tabs
        };
    }

    private async Task<IReadOnlyList<ProductCardViewModel>> GetVariantCardsByCategoryAsync(
        IReadOnlyCollection<string> categorySlugs,
        IReadOnlyList<CategoryRecord> categories,
        CancellationToken cancellationToken)
    {
        var categoryIds = GetCategoryTreeIds(categorySlugs, categories);
        if (categoryIds.Count == 0)
        {
            return [];
        }

        var variants = await dbContext.ProductVariants
            .AsNoTracking()
            .AsSplitQuery()
            .Include(variant => variant.Product)
                .ThenInclude(product => product!.Brand)
            .Include(variant => variant.ProductVariantImages)
            .Include(variant => variant.VariantAttributes)
                .ThenInclude(attribute => attribute.AttributeOption)
                    .ThenInclude(option => option!.Attribute)
            .Where(variant => variant.IsActive)
            .Where(variant => variant.Product != null && variant.Product.IsActive)
            .Where(variant => categoryIds.Contains(variant.Product!.CategoryId))
            .OrderByDescending(variant => variant.Product!.IsFeatured)
            .ThenByDescending(variant => variant.SoldCount)
            .ThenByDescending(variant => variant.Product!.TotalSoldCount)
            .ThenByDescending(variant => variant.IsDefault)
            .ThenByDescending(variant => variant.CreatedAt)
            .Take(MaxVariantsPerPanel)
            .ToListAsync(cancellationToken);

        return variants
            .Select(DbHomeProductCardMapper.ToProductCard)
            .OfType<ProductCardViewModel>()
            .ToList();
    }

    private async Task<IReadOnlyList<CategoryBrandViewModel>> GetBrandsByCategoryAsync(
        IReadOnlyCollection<string> categorySlugs,
        string categoryQueryValue,
        IReadOnlyList<CategoryRecord> categories,
        CancellationToken cancellationToken)
    {
        var categoryIds = GetCategoryTreeIds(categorySlugs, categories);
        if (categoryIds.Count == 0)
        {
            return [];
        }

        var brands = await dbContext.Products
            .AsNoTracking()
            .Where(product => product.IsActive)
            .Where(product => categoryIds.Contains(product.CategoryId))
            .Where(product => product.Brand != null && product.Brand.IsActive)
            .Where(product => product.ProductVariants.Any(variant => variant.IsActive))
            .Select(product => new
            {
                product.Brand!.Name,
                product.Brand.Slug
            })
            .Distinct()
            .OrderBy(brand => brand.Name)
            .ToListAsync(cancellationToken);

        return brands
            .Select(brand => new CategoryBrandViewModel
            {
                Label = brand.Name,
                Url = $"/catalog?cat={categoryQueryValue}&brand={Uri.EscapeDataString(GetBrandUrlValue(brand.Slug, brand.Name))}"
            })
            .ToList();
    }

    private async Task<IReadOnlyList<CategoryRecord>> GetActiveCategoriesAsync(
        CancellationToken cancellationToken)
    {
        return await dbContext.Categories
            .AsNoTracking()
            .Where(category => category.IsActive)
            .OrderBy(category => category.Position)
            .ThenBy(category => category.Id)
            .Select(category => new CategoryRecord(
                category.Id,
                category.ParentId,
                category.Name,
                category.Slug,
                category.ImagePath,
                category.Position))
            .ToListAsync(cancellationToken);
    }

    private static IReadOnlyList<CategoryQuickLinkViewModel> BuildLevelTwoCategoryLinks(
        string rootCategorySlug,
        IReadOnlyList<CategoryRecord> categories)
    {
        var rootCategory = categories.FirstOrDefault(category =>
            category.ParentId is null
            && string.Equals(
                category.Slug,
                rootCategorySlug,
                StringComparison.OrdinalIgnoreCase));
        if (rootCategory is null)
        {
            return [];
        }

        return categories
            .Where(category => category.ParentId == rootCategory.Id)
            .OrderBy(category => category.Position)
            .ThenBy(category => category.Id)
            .Select(category => new CategoryQuickLinkViewModel
            {
                Label = category.Name,
                Url = BuildCatalogUrl(category.Slug),
                ImageUrl = NormalizeCategoryImage(category.ImagePath)
            })
            .ToList();
    }

    private static HashSet<long> GetCategoryTreeIds(
        IReadOnlyCollection<string> rootSlugs,
        IReadOnlyList<CategoryRecord> categories)
    {
        var normalizedSlugs = rootSlugs.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var categoryIds = categories
            .Where(category => normalizedSlugs.Contains(category.Slug))
            .Select(category => category.Id)
            .ToHashSet();

        var addedChild = true;
        while (addedChild)
        {
            addedChild = false;
            foreach (var category in categories)
            {
                if (category.ParentId.HasValue
                    && categoryIds.Contains(category.ParentId.Value)
                    && categoryIds.Add(category.Id))
                {
                    addedChild = true;
                }
            }
        }

        return categoryIds;
    }

    private static string BuildCatalogUrl(string categorySlug)
    {
        return $"/catalog?cat={Uri.EscapeDataString(categorySlug)}";
    }

    private static string NormalizeCategoryImage(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return CategoryFallbackImage;
        }

        if (imagePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || imagePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            || imagePath.StartsWith('/'))
        {
            return imagePath;
        }

        return "/" + imagePath.TrimStart('/');
    }

    private static string GetBrandUrlValue(string? slug, string name)
    {
        return string.IsNullOrWhiteSpace(slug)
            ? name.ToLowerInvariant()
            : slug;
    }

    private sealed record CategorySectionDefinition(
        string Id,
        string Label,
        string CategorySlug,
        bool IsActive = false,
        bool ShowBrandFilter = false);

    private sealed record CategoryRecord(
        long Id,
        long? ParentId,
        string Name,
        string Slug,
        string? ImagePath,
        int Position);
}
