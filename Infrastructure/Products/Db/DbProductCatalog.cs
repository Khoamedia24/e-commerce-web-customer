using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Products;
using e_commerce_web_customer.Application.Search;
using e_commerce_web_customer.Data;
using e_commerce_web_customer.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace e_commerce_web_customer.Infrastructure.Products.Db;

public sealed class DbProductCatalog(EcommerceDbContext dbContext) : IProductCatalog
{
    private const string SearchCollation = "Vietnamese_CI_AI";
    private const int MaxSuggestionCandidates = 100;
    private const int MaxSearchTerms = 8;

    public async Task<IReadOnlyList<ProductReadModel>> SearchAsync(
        ProductCatalogSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = NormalizeQuery(request.Query);
        var terms = BuildSearchTerms(query);
        var candidateLimit = GetCandidateLimit(request.Limit);
        var products = BuildActiveProductQuery();

        foreach (var term in terms)
        {
            products = ApplyTermFilter(products, EscapeLikePattern(term));
        }

        IQueryable<Product> candidateQuery = products
            .OrderByDescending(product => product.IsFeatured)
            .ThenByDescending(product => product.TotalSoldCount)
            .ThenByDescending(product => product.ViewsCount)
            .ThenBy(product => product.Id);

        if (candidateLimit.HasValue)
        {
            candidateQuery = candidateQuery.Take(candidateLimit.Value);
        }

        var candidates = await candidateQuery
            .Include(product => product.Brand)
            .Include(product => product.Category)
            .Include(product => product.ProductSpecifications)
                .ThenInclude(specification => specification.Specification)
            .Include(product => product.ProductVariants.Where(variant => variant.IsActive))
                .ThenInclude(variant => variant.ProductVariantImages)
            .Include(product => product.ProductVariants.Where(variant => variant.IsActive))
                .ThenInclude(variant => variant.VariantAttributes)
                .ThenInclude(attribute => attribute.AttributeOption)
                    .ThenInclude(option => option!.Attribute)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        var normalizedQuery = SearchTextNormalizer.Normalize(query);
        var searchResults = request.Scope == ProductCatalogSearchScope.Variants
            ? BuildVariantResults(candidates, normalizedQuery)
            : BuildProductResults(candidates, normalizedQuery);

        if (request.Limit is > 0)
        {
            searchResults = searchResults.Take(Math.Clamp(
                request.Limit.Value,
                1,
                MaxSuggestionCandidates));
        }

        return searchResults.ToList();
    }

    public async Task<ProductReadModel?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var normalizedId = id.Trim();
        if (string.IsNullOrWhiteSpace(normalizedId))
        {
            return null;
        }

        var products = BuildActiveProductQuery();
        if (long.TryParse(normalizedId, out var numericId))
        {
            products = products.Where(product =>
                product.Id == numericId
                || product.ProductVariants.Any(variant => variant.Id == numericId));
        }
        else
        {
            products = products.Where(product =>
                product.Slug == normalizedId
                || product.ProductVariants.Any(variant => variant.Code == normalizedId));
        }

        var product = await products
            .Include(item => item.Brand)
            .Include(item => item.Category)
            .Include(item => item.ProductSpecifications)
                .ThenInclude(specification => specification.Specification)
            .Include(item => item.ProductVariants.Where(variant => variant.IsActive))
                .ThenInclude(variant => variant.ProductVariantImages)
            .Include(item => item.ProductVariants.Where(variant => variant.IsActive))
                .ThenInclude(variant => variant.VariantAttributes)
                .ThenInclude(attribute => attribute.AttributeOption)
                    .ThenInclude(option => option!.Attribute)
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken);

        return product is null ? null : DbProductSearchMapper.Map(product);
    }

    private IQueryable<Product> BuildActiveProductQuery()
    {
        return dbContext.Products
            .AsNoTracking()
            .Where(product => product.IsActive
                && product.Brand != null
                && product.Brand.IsActive
                && product.Category != null
                && product.Category.IsActive
                && product.ProductVariants.Any(variant => variant.IsActive));
    }

    private static IEnumerable<ProductReadModel> BuildProductResults(
        IEnumerable<Product> candidates,
        string normalizedQuery)
    {
        return candidates
            .Select(product => new
            {
                Result = DbProductSearchMapper.Map(product),
                Relevance = DbProductSearchRanker.CalculateRelevance(
                    product,
                    normalizedQuery)
            })
            .OrderByDescending(item => string.IsNullOrWhiteSpace(normalizedQuery)
                ? item.Result.PopularityScore
                : item.Relevance)
            .ThenByDescending(item => item.Result.PopularityScore)
            .ThenBy(item => item.Result.Name)
            .Select(item => item.Result);
    }

    private static IEnumerable<ProductReadModel> BuildVariantResults(
        IEnumerable<Product> candidates,
        string normalizedQuery)
    {
        return candidates
            .SelectMany(product => product.ProductVariants
                .Where(variant => variant.IsActive)
                .Select(variant => new
                {
                    Result = DbProductSearchMapper.MapVariant(product, variant),
                    Relevance = DbProductSearchRanker.CalculateRelevance(
                        product,
                        variant,
                        normalizedQuery)
                }))
            .OrderByDescending(item => string.IsNullOrWhiteSpace(normalizedQuery)
                ? item.Result.PopularityScore
                : item.Relevance)
            .ThenByDescending(item => item.Result.PopularityScore)
            .ThenBy(item => item.Result.Name)
            .Select(item => item.Result);
    }

    private static IQueryable<Product> ApplyTermFilter(
        IQueryable<Product> products,
        string escapedTerm)
    {
        var pattern = $"%{escapedTerm}%";
        return products.Where(product =>
            EF.Functions.Like(
                EF.Functions.Collate(product.Name, SearchCollation),
                pattern,
                "\\")
            || EF.Functions.Like(
                EF.Functions.Collate(product.Slug, SearchCollation),
                pattern,
                "\\")
            || EF.Functions.Like(
                EF.Functions.Collate(
                    product.Description ?? string.Empty,
                    SearchCollation),
                pattern,
                "\\")
            || EF.Functions.Like(
                EF.Functions.Collate(product.Brand!.Name, SearchCollation),
                pattern,
                "\\")
            || EF.Functions.Like(
                EF.Functions.Collate(product.Brand.Slug, SearchCollation),
                pattern,
                "\\")
            || EF.Functions.Like(
                EF.Functions.Collate(product.Category!.Name, SearchCollation),
                pattern,
                "\\")
            || EF.Functions.Like(
                EF.Functions.Collate(product.Category.Slug, SearchCollation),
                pattern,
                "\\")
            || product.ProductSpecifications.Any(specification =>
                EF.Functions.Like(
                    EF.Functions.Collate(specification.Value, SearchCollation),
                    pattern,
                    "\\")
                || EF.Functions.Like(
                    EF.Functions.Collate(
                        specification.Specification!.Name,
                        SearchCollation),
                    pattern,
                    "\\"))
            || product.ProductVariants.Any(variant =>
                variant.IsActive
                && (EF.Functions.Like(
                    EF.Functions.Collate(variant.Code, SearchCollation),
                    pattern,
                    "\\")
                || EF.Functions.Like(
                    EF.Functions.Collate(
                        variant.Code
                            .Replace("-", string.Empty)
                            .Replace("_", string.Empty),
                        SearchCollation),
                    pattern,
                    "\\")
                || EF.Functions.Like(
                    EF.Functions.Collate(
                        variant.ColorName ?? string.Empty,
                        SearchCollation),
                    pattern,
                    "\\")
                || variant.VariantAttributes.Any(attribute =>
                    EF.Functions.Like(
                        EF.Functions.Collate(
                            attribute.AttributeOption!.Label,
                            SearchCollation),
                        pattern,
                        "\\")
                    || EF.Functions.Like(
                        EF.Functions.Collate(
                            attribute.AttributeOption.Value,
                            SearchCollation),
                        pattern,
                        "\\")
                    || EF.Functions.Like(
                        EF.Functions.Collate(
                            attribute.AttributeOption.Label.Replace(" ", string.Empty),
                            SearchCollation),
                        pattern,
                        "\\")
                    || EF.Functions.Like(
                        EF.Functions.Collate(
                            attribute.AttributeOption.Value.Replace(" ", string.Empty),
                            SearchCollation),
                        pattern,
                        "\\")
                    || EF.Functions.Like(
                        EF.Functions.Collate(
                            attribute.AttributeOption.Attribute!.Name,
                            SearchCollation),
                        pattern,
                        "\\")))));
    }

    private static string NormalizeQuery(string? query)
    {
        return SearchTextNormalizer.CleanQuery(query);
    }

    private static IReadOnlyList<string> BuildSearchTerms(string query)
    {
        return DbProductSearchRanker.BuildTerms(query, MaxSearchTerms);
    }

    private static int? GetCandidateLimit(int? requestedLimit)
    {
        if (requestedLimit is not > 0)
        {
            return null;
        }

        var resultLimit = Math.Clamp(
            requestedLimit.Value,
            1,
            MaxSuggestionCandidates);
        return Math.Min(
            MaxSuggestionCandidates,
            Math.Max(resultLimit * 4, 24));
    }

    private static string EscapeLikePattern(string value)
    {
        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("%", "\\%", StringComparison.Ordinal)
            .Replace("_", "\\_", StringComparison.Ordinal)
            .Replace("[", "\\[", StringComparison.Ordinal);
    }

}
