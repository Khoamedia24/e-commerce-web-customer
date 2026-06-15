using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Data;
using e_commerce_web_customer.Models.Constants;
using e_commerce_web_customer.Models.Entities;
using e_commerce_web_customer.ViewModels.Product;
using e_commerce_web_customer.ViewModels.Shared;
using Microsoft.EntityFrameworkCore;

namespace e_commerce_web_customer.Infrastructure.Products.Db;

public sealed class DbProductDetailDataService(EcommerceDbContext dbContext) : IProductDetailDataService
{
    private const string FallbackImageUrl = "/images/logo-techstore-icon.svg";
    private const string DefaultSpecGroupName = "Thông tin sản phẩm";

    public async Task<ProductDetailViewModel?> CreateProductDetailAsync(
        string slug,
        string? variantKey = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedSlug = slug.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalizedSlug))
        {
            return null;
        }

        var seedProduct = await dbContext.Products
            .AsNoTracking()
            .Include(product => product.Brand)
            .Include(product => product.Category)
            .FirstOrDefaultAsync(
                product => product.IsActive && product.Slug == normalizedSlug,
                cancellationToken);

        if (seedProduct is null)
        {
            return null;
        }

        var familyProducts = await dbContext.Products
            .AsNoTracking()
            .Where(product =>
                product.IsActive
                && product.BrandId == seedProduct.BrandId
                && product.CategoryId == seedProduct.CategoryId)
            .Include(product => product.Brand)
            .Include(product => product.Category)
                .ThenInclude(category => category!.CategorySpecifications)
                    .ThenInclude(categorySpecification => categorySpecification.Specification)
            .Include(product => product.ProductSpecifications)
                .ThenInclude(productSpecification => productSpecification.Specification)
            .Include(product => product.ProductVariants)
                .ThenInclude(variant => variant.ProductVariantImages)
            .Include(product => product.ProductVariants)
                .ThenInclude(variant => variant.VariantAttributes)
                    .ThenInclude(variantAttribute => variantAttribute.AttributeOption)
                        .ThenInclude(attributeOption => attributeOption!.Attribute)
            .AsSplitQuery()
            .OrderBy(product => product.Id)
            .ToListAsync(cancellationToken);

        var selectedProduct = familyProducts.FirstOrDefault(product => product.Id == seedProduct.Id);
        if (selectedProduct is null)
        {
            return null;
        }

        var activeVariants = familyProducts
            .SelectMany(product => product.ProductVariants.Where(variant => variant.IsActive))
            .ToList();

        if (activeVariants.Count == 0)
        {
            return null;
        }

        var selectedVariant = ResolveSelectedVariant(
            activeVariants,
            selectedProduct.Id,
            variantKey);

        if (selectedVariant is null)
        {
            return null;
        }

        selectedProduct = familyProducts.FirstOrDefault(product => product.Id == selectedVariant.ProductId)
            ?? selectedProduct;

        var versionGroups = BuildVersionGroups(familyProducts, activeVariants);
        var selectedVersionKey = BuildVersionKey(selectedVariant);
        var selectedVersionGroup = versionGroups.FirstOrDefault(group => group.Key.Equals(selectedVersionKey));

        var colorVariants = selectedVersionGroup?.Variants
            ?? [selectedVariant];
        var detailName = BuildDetailName(selectedProduct, selectedVariant);
        var selectedImage = GetPrimaryImage(selectedVariant);
        var mainImageUrl = NormalizeImageUrl(selectedImage?.ImagePath);

        return new ProductDetailViewModel
        {
            Slug = selectedProduct.Slug,
            CartItemId = GetVariantKey(selectedVariant),
            SelectedVariantLabel = BuildCartVariantLabel(selectedVariant),
            Name = detailName,
            Brand = selectedProduct.Brand?.Name ?? string.Empty,
            MainImageUrl = mainImageUrl,
            MainImageAlt = BuildImageAlt(selectedImage, detailName, selectedVariant),
            CurrentPrice = selectedVariant.Price,
            OldPrice = null,
            Rating = selectedProduct.RatingAverage,
            ReviewCount = selectedProduct.RatingCount,
            Breadcrumbs = BuildBreadcrumbs(selectedProduct, detailName),
            QuickLinks = BuildQuickLinks(),
            GalleryItems = BuildGalleryItems(selectedVariant, detailName),
            StorageOptions = BuildStorageOptions(versionGroups, selectedVersionKey, selectedVariant),
            ColorOptions = BuildColorOptions(selectedProduct, colorVariants, selectedVariant, detailName),
            TechnicalSpecSections = BuildTechnicalSpecSections(selectedProduct, detailName),
            RelatedProductGroups = BuildRelatedProductGroups(versionGroups, selectedVersionKey, selectedVariant),
            ReviewSummary = BuildReviewSummary(detailName, selectedProduct.RatingAverage, selectedProduct.RatingCount),
            QuestionAnswerSection = BuildQuestionAnswerSection(detailName)
        };
    }

    private static ProductVariant? ResolveSelectedVariant(
        IReadOnlyList<ProductVariant> activeVariants,
        long selectedProductId,
        string? variantKey)
    {
        if (!string.IsNullOrWhiteSpace(variantKey))
        {
            var normalizedVariantKey = variantKey.Trim();
            var variantByKey = activeVariants.FirstOrDefault(variant =>
                string.Equals(variant.Code, normalizedVariantKey, StringComparison.OrdinalIgnoreCase)
                || string.Equals(variant.Id.ToString(CultureInfo.InvariantCulture), normalizedVariantKey, StringComparison.OrdinalIgnoreCase));

            if (variantByKey is not null)
            {
                return variantByKey;
            }
        }

        return activeVariants
                .Where(variant => variant.ProductId == selectedProductId)
                .OrderByDescending(variant => variant.IsDefault)
                .ThenBy(variant => variant.Id)
                .FirstOrDefault()
            ?? activeVariants
                .OrderByDescending(variant => variant.IsDefault)
                .ThenBy(variant => variant.Id)
                .FirstOrDefault();
    }

    private static IReadOnlyList<VersionGroup> BuildVersionGroups(
        IReadOnlyList<Product> familyProducts,
        IReadOnlyList<ProductVariant> activeVariants)
    {
        var productById = familyProducts.ToDictionary(product => product.Id);
        var productOrderById = familyProducts
            .Select((product, index) => new { product.Id, Index = index })
            .ToDictionary(product => product.Id, product => product.Index);

        return activeVariants
            .GroupBy(BuildVersionKey)
            .Select(group => new VersionGroup(
                group.Key,
                productById[group.Key.ProductId],
                group.OrderBy(GetColorOrder).ThenBy(variant => variant.Id).ToList()))
            .OrderBy(group => productOrderById[group.Key.ProductId])
            .ThenBy(group => group.Key.StorageSize)
            .ThenBy(group => group.Key.RamSize)
            .ThenBy(group => group.Key.StorageLabel)
            .ToList();
    }

    private static VersionKey BuildVersionKey(ProductVariant variant)
    {
        var ram = FindVariantAttribute(
            variant,
            CatalogAttributeCodes.Ram,
            CatalogAttributeCodes.RamCapacity);
        var storage = FindVariantAttribute(
            variant,
            CatalogAttributeCodes.Rom,
            CatalogAttributeCodes.Storage,
            CatalogAttributeCodes.InternalStorage);

        var ramLabel = ram?.Label ?? string.Empty;
        var storageLabel = storage?.Label ?? string.Empty;

        return new VersionKey(
            variant.ProductId,
            ram?.OptionId ?? 0,
            storage?.OptionId ?? 0,
            ramLabel,
            storageLabel,
            ParseCapacityToMb(ramLabel),
            ParseCapacityToMb(storageLabel));
    }

    private static VariantAttributeValue? FindVariantAttribute(
        ProductVariant variant,
        params string[] attributeCodes)
    {
        foreach (var variantAttribute in variant.VariantAttributes)
        {
            var option = variantAttribute.AttributeOption;
            var attribute = option?.Attribute;
            if (option is null || attribute is null)
            {
                continue;
            }

            if (attributeCodes.Any(code => string.Equals(code, attribute.Code, StringComparison.OrdinalIgnoreCase)))
            {
                return new VariantAttributeValue(
                    variantAttribute.AttributeOptionId,
                    attribute.Code,
                    attribute.Name,
                    option.Value,
                    option.Label);
            }
        }

        return null;
    }

    private static IReadOnlyList<ProductDetailStorageOptionViewModel> BuildStorageOptions(
        IReadOnlyList<VersionGroup> versionGroups,
        VersionKey selectedVersionKey,
        ProductVariant selectedVariant)
    {
        return versionGroups
            .Select(group =>
            {
                var targetVariant = ResolveVersionLinkVariant(group, selectedVariant);

                return new ProductDetailStorageOptionViewModel
                {
                    Label = BuildVersionLabel(group.Product, group.Key),
                    Url = BuildVariantDetailUrl(group.Product, targetVariant),
                    IsActive = group.Key.Equals(selectedVersionKey),
                    IsInitiallyHidden = false
                };
            })
            .ToList();
    }

    private static ProductVariant ResolveVersionLinkVariant(
        VersionGroup group,
        ProductVariant selectedVariant)
    {
        if (!string.IsNullOrWhiteSpace(selectedVariant.ColorName))
        {
            var sameColorVariant = group.Variants.FirstOrDefault(variant =>
                string.Equals(variant.ColorName, selectedVariant.ColorName, StringComparison.OrdinalIgnoreCase));

            if (sameColorVariant is not null)
            {
                return sameColorVariant;
            }
        }

        return group.Variants.FirstOrDefault(variant => variant.IsDefault)
            ?? group.Variants[0];
    }

    private static IReadOnlyList<ProductDetailColorOptionViewModel> BuildColorOptions(
        Product product,
        IReadOnlyList<ProductVariant> colorVariants,
        ProductVariant selectedVariant,
        string detailName)
    {
        return colorVariants
            .OrderBy(GetColorOrder)
            .ThenBy(variant => variant.Id)
            .Select(variant =>
            {
                var image = GetPrimaryImage(variant);

                return new ProductDetailColorOptionViewModel
                {
                    VariantKey = GetVariantKey(variant),
                    DetailUrl = BuildVariantDetailUrl(product, variant),
                    VariantLabel = BuildCartVariantLabel(variant),
                    Name = string.IsNullOrWhiteSpace(variant.ColorName)
                        ? "Mặc định"
                        : variant.ColorName,
                    ImageUrl = NormalizeImageUrl(image?.ImagePath),
                    ImageAlt = BuildImageAlt(image, detailName, variant),
                    Price = variant.Price,
                    IsActive = variant.Id == selectedVariant.Id
                };
            })
            .ToList();
    }

    private static IReadOnlyList<ProductDetailGalleryItemViewModel> BuildGalleryItems(
        ProductVariant selectedVariant,
        string detailName)
    {
        var galleryItems = selectedVariant.ProductVariantImages
            .OrderBy(image => image.Position)
            .ThenBy(image => image.Id)
            .Select((image, index) => new ProductDetailGalleryItemViewModel
            {
                Label = $"Ảnh {index + 1}",
                ImageUrl = NormalizeImageUrl(image.ImagePath),
                ImageAlt = BuildImageAlt(image, detailName, selectedVariant)
            })
            .ToList();

        if (galleryItems.Count > 0)
        {
            return galleryItems;
        }

        return
        [
            new()
            {
                Label = "Sản phẩm",
                ImageUrl = FallbackImageUrl,
                ImageAlt = detailName
            }
        ];
    }

    private static IReadOnlyList<ProductTechnicalSpecSectionViewModel> BuildTechnicalSpecSections(
        Product product,
        string detailName)
    {
        var categorySpecs = product.Category?.CategorySpecifications
                .Where(categorySpecification => categorySpecification.Specification is not null)
                .ToDictionary(categorySpecification => categorySpecification.SpecificationId)
            ?? [];

        var rows = product.ProductSpecifications
            .Where(productSpecification => productSpecification.Specification is not null)
            .Select(productSpecification =>
            {
                categorySpecs.TryGetValue(productSpecification.SpecificationId, out var categorySpecification);
                var specification = productSpecification.Specification!;
                var groupName = string.IsNullOrWhiteSpace(categorySpecification?.GroupName)
                    ? DefaultSpecGroupName
                    : categorySpecification.GroupName;
                var rowSortOrder = categorySpecification?.SortOrder ?? productSpecification.SortOrder;

                return new SpecRowDraft(
                    groupName,
                    rowSortOrder,
                    new ProductTechnicalSpecRowViewModel
                    {
                        Label = specification.Name,
                        Value = FormatSpecificationValue(productSpecification.Value, specification.Unit),
                        IsHighlighted = productSpecification.IsHighlight
                    },
                    rowSortOrder);
            })
            .ToList();

        if (rows.Count == 0)
        {
            return
            [
                new()
                {
                    Id = "thong-tin-san-pham",
                    Title = DefaultSpecGroupName,
                    Rows =
                    [
                        new()
                        {
                            Label = "Tên sản phẩm",
                            Value = detailName,
                            IsHighlighted = true
                        }
                    ]
                }
            ];
        }

        return rows
            .GroupBy(row => row.GroupName)
            .OrderBy(group => group.Min(row => row.GroupSortOrder))
            .ThenBy(group => group.Key)
            .Select(group => new ProductTechnicalSpecSectionViewModel
            {
                Id = Slugify(group.Key),
                Title = group.Key,
                Rows = group
                    .OrderBy(row => row.RowSortOrder)
                    .ThenBy(row => row.Row.Label)
                    .Select(row => row.Row)
                    .ToList()
            })
            .ToList();
    }

    private static IReadOnlyList<ProductRelatedProductGroupViewModel> BuildRelatedProductGroups(
        IReadOnlyList<VersionGroup> versionGroups,
        VersionKey selectedVersionKey,
        ProductVariant selectedVariant)
    {
        var products = versionGroups
            .Where(group => !group.Key.Equals(selectedVersionKey))
            .Select(group =>
            {
                var variant = ResolveVersionLinkVariant(group, selectedVariant);
                var image = GetPrimaryImage(variant);
                var name = BuildDetailName(group.Product, variant);

                return new ProductRelatedProductViewModel
                {
                    Url = BuildVariantDetailUrl(group.Product, variant),
                    Name = name,
                    ImageUrl = NormalizeImageUrl(image?.ImagePath),
                    ImageAlt = BuildImageAlt(image, name, variant),
                    CurrentPrice = variant.Price,
                    OldPrice = null,
                    DiscountLabel = null,
                    InstallmentLabel = "Trả góp 0%",
                    GiftNote = null,
                    DeliveryLabel = "Giao 2 giờ",
                    Location = "Hồ Chí Minh",
                    Rating = group.Product.RatingAverage > 0 ? group.Product.RatingAverage : null
                };
            })
            .Take(12)
            .ToList();

        if (products.Count == 0)
        {
            return [];
        }

        return
        [
            new()
            {
                Id = "same-series",
                Label = "Sản phẩm cùng dòng",
                IsActive = true,
                Products = products
            }
        ];
    }

    private static IReadOnlyList<ProductDetailBreadcrumbViewModel> BuildBreadcrumbs(
        Product product,
        string detailName)
    {
        List<ProductDetailBreadcrumbViewModel> breadcrumbs =
        [
            new() { Label = "Trang chủ", Url = "/" }
        ];

        if (product.Category is not null)
        {
            breadcrumbs.Add(new()
            {
                Label = product.Category.Name,
                Url = $"/catalog?cat={Uri.EscapeDataString(product.Category.Slug)}"
            });
        }

        if (product.Brand is not null)
        {
            breadcrumbs.Add(new()
            {
                Label = product.Brand.Name,
                Url = product.Category is null
                    ? $"/catalog?brand={Uri.EscapeDataString(product.Brand.Slug)}"
                    : $"/catalog?cat={Uri.EscapeDataString(product.Category.Slug)}&brand={Uri.EscapeDataString(product.Brand.Slug)}"
            });
        }

        breadcrumbs.Add(new() { Label = detailName });

        return breadcrumbs;
    }

    private static IReadOnlyList<ProductDetailActionLinkViewModel> BuildQuickLinks()
    {
        return
        [
            new() { Label = "Yêu thích", IconId = "product-card-icon-heart" },
            new() { Label = "Hỏi đáp", IconId = "hero-icon-news", Url = "#block-comment-cps" },
            new() { Label = "Thông số", IconId = "hero-icon-phone" },
            new() { Label = "So sánh", IconId = "hero-icon-swap" }
        ];
    }

    private static ProductReviewSummaryViewModel BuildReviewSummary(
        string detailName,
        decimal score,
        int totalReviews)
    {
        var clampedScore = Math.Clamp(score, 0m, 5m);
        var fiveStarCount = totalReviews > 0 ? totalReviews : 0;

        return new ProductReviewSummaryViewModel
        {
            Title = $"Đánh giá {detailName}",
            Score = clampedScore,
            TotalReviews = totalReviews,
            RatingBreakdown =
            [
                new() { Stars = 5, Count = fiveStarCount, Percent = fiveStarCount > 0 ? 100 : 0 },
                new() { Stars = 4, Count = 0, Percent = 0 },
                new() { Stars = 3, Count = 0, Percent = 0 },
                new() { Stars = 2, Count = 0, Percent = 0 },
                new() { Stars = 1, Count = 0, Percent = 0 }
            ],
            ExperienceRatings = [],
            Reviews = []
        };
    }

    private static QuestionAnswerSectionViewModel BuildQuestionAnswerSection(string detailName)
    {
        return new QuestionAnswerSectionViewModel
        {
            Title = "Hỏi và đáp",
            FormTitle = "Hãy đặt câu hỏi cho chúng tôi",
            Description = $"Gửi câu hỏi về {detailName}, TechStore sẽ hỗ trợ bạn sớm nhất.",
            Placeholder = "Viết câu hỏi của bạn tại đây",
            SubmitLabel = "Gửi câu hỏi",
            AdditionalCommentCount = 0,
            Threads = []
        };
    }

    private static string BuildDetailName(Product product, ProductVariant variant)
    {
        var versionKey = BuildVersionKey(variant);
        var parts = new List<string> { product.Name };

        AddCapacityLabel(parts, versionKey.RamLabel);
        AddCapacityLabel(parts, versionKey.StorageLabel);

        return string.Join(' ', parts);
    }

    private static string BuildCartVariantLabel(ProductVariant variant)
    {
        var versionKey = BuildVersionKey(variant);
        var parts = new List<string>();

        AddCapacityLabel(parts, versionKey.RamLabel);
        AddCapacityLabel(parts, versionKey.StorageLabel);

        if (!string.IsNullOrWhiteSpace(variant.ColorName))
        {
            parts.Add(variant.ColorName.Trim());
        }

        return string.Join(" - ", parts);
    }

    private static string BuildVersionLabel(Product product, VersionKey versionKey)
    {
        var parts = new List<string> { BuildShortProductName(product) };

        AddCapacityLabel(parts, versionKey.RamLabel);
        AddCapacityLabel(parts, versionKey.StorageLabel);

        return string.Join(' ', parts);
    }

    private static string BuildShortProductName(Product product)
    {
        var name = product.Name;

        if (!string.IsNullOrWhiteSpace(product.Brand?.Name)
            && name.StartsWith(product.Brand.Name, StringComparison.OrdinalIgnoreCase))
        {
            name = name[product.Brand.Name.Length..];
        }

        name = Regex.Replace(name, "\\bGalaxy\\b", string.Empty, RegexOptions.IgnoreCase);
        name = Regex.Replace(name, "\\b5G\\b", string.Empty, RegexOptions.IgnoreCase);
        name = Regex.Replace(name, "\\s+", " ").Trim();

        return string.IsNullOrWhiteSpace(name)
            ? product.Name
            : name;
    }

    private static void AddCapacityLabel(List<string> parts, string label)
    {
        var normalizedLabel = NormalizeCapacityLabel(label);
        if (!string.IsNullOrWhiteSpace(normalizedLabel)
            && !parts.Contains(normalizedLabel, StringComparer.OrdinalIgnoreCase))
        {
            parts.Add(normalizedLabel);
        }
    }

    private static string NormalizeCapacityLabel(string? label)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            return string.Empty;
        }

        return Regex.Replace(
            label.Trim(),
            "(\\d)\\s+(GB|TB|MB)\\b",
            "$1$2",
            RegexOptions.IgnoreCase);
    }

    private static string BuildVariantDetailUrl(Product product, ProductVariant variant)
    {
        return $"/product/{Uri.EscapeDataString(product.Slug)}?variant={Uri.EscapeDataString(GetVariantKey(variant))}";
    }

    private static string GetVariantKey(ProductVariant variant)
    {
        return string.IsNullOrWhiteSpace(variant.Code)
            ? variant.Id.ToString(CultureInfo.InvariantCulture)
            : variant.Code;
    }

    private static ProductVariantImage? GetPrimaryImage(ProductVariant variant)
    {
        return variant.ProductVariantImages
            .OrderBy(image => image.Position)
            .ThenBy(image => image.Id)
            .FirstOrDefault();
    }

    private static string BuildImageAlt(
        ProductVariantImage? image,
        string detailName,
        ProductVariant variant)
    {
        if (!string.IsNullOrWhiteSpace(image?.AltText))
        {
            return image.AltText;
        }

        return string.IsNullOrWhiteSpace(variant.ColorName)
            ? detailName
            : $"{detailName} {variant.ColorName}";
    }

    private static string NormalizeImageUrl(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return FallbackImageUrl;
        }

        if (imagePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || imagePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            || imagePath.StartsWith('/'))
        {
            return imagePath;
        }

        return "/" + imagePath.TrimStart('/');
    }

    private static string FormatSpecificationValue(string value, string? unit)
    {
        var trimmedValue = value.Trim();
        if (string.IsNullOrWhiteSpace(unit)
            || trimmedValue.Contains(unit, StringComparison.OrdinalIgnoreCase)
            || !Regex.IsMatch(trimmedValue, "^\\d+([\\.,]\\d+)?$"))
        {
            return trimmedValue;
        }

        return $"{trimmedValue} {unit}";
    }

    private static int GetColorOrder(ProductVariant variant)
    {
        var colorName = RemoveDiacritics(variant.ColorName ?? string.Empty).ToLowerInvariant();

        if (ContainsAny(colorName, "den", "black"))
        {
            return 0;
        }

        if (ContainsAny(colorName, "tim", "purple", "violet"))
        {
            return 1;
        }

        if (ContainsAny(colorName, "trang", "white"))
        {
            return 2;
        }

        if (ContainsAny(colorName, "xanh", "blue"))
        {
            return 3;
        }

        return 100;
    }

    private static int ParseCapacityToMb(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return 0;
        }

        var match = Regex.Match(
            value.Replace('_', ' ').Replace('-', ' '),
            "(?<number>\\d+(?:[\\.,]\\d+)?)\\s*(?<unit>TB|GB|MB)",
            RegexOptions.IgnoreCase);

        if (!match.Success
            || !decimal.TryParse(
                match.Groups["number"].Value.Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var number))
        {
            return 0;
        }

        var multiplier = match.Groups["unit"].Value.ToUpperInvariant() switch
        {
            "TB" => 1024 * 1024,
            "GB" => 1024,
            _ => 1
        };

        return (int)Math.Round(number * multiplier, MidpointRounding.AwayFromZero);
    }

    private static bool ContainsAny(string value, params string[] keywords)
    {
        return keywords.Any(keyword => value.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    private static string Slugify(string value)
    {
        var normalizedValue = RemoveDiacritics(value).ToLowerInvariant();
        var slug = Regex.Replace(normalizedValue, "[^a-z0-9]+", "-").Trim('-');

        return string.IsNullOrWhiteSpace(slug)
            ? "thong-so"
            : slug;
    }

    private static string RemoveDiacritics(string value)
    {
        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);
            if (category != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        return builder
            .ToString()
            .Normalize(NormalizationForm.FormC)
            .Replace('đ', 'd')
            .Replace('Đ', 'D');
    }

    private readonly record struct VersionKey(
        long ProductId,
        long RamOptionId,
        long StorageOptionId,
        string RamLabel,
        string StorageLabel,
        int RamSize,
        int StorageSize);

    private sealed record VersionGroup(
        VersionKey Key,
        Product Product,
        IReadOnlyList<ProductVariant> Variants);

    private sealed record VariantAttributeValue(
        long OptionId,
        string Code,
        string Name,
        string Value,
        string Label);

    private sealed record SpecRowDraft(
        string GroupName,
        int GroupSortOrder,
        ProductTechnicalSpecRowViewModel Row,
        int RowSortOrder);
}
