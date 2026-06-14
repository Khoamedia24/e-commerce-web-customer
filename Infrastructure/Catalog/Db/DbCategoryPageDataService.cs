using e_commerce_web_customer.Application.Catalog;
using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Data;
using e_commerce_web_customer.ViewModels.Catalog;
using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Catalog.Db;

public sealed class DbCategoryPageDataService(EcommerceDbContext dbContext) : ICategoryPageDataService
{
    public Task<CategoryPageViewModel?> CreateCategoryPageAsync(
        CategoryPageRequest request,
        CancellationToken cancellationToken = default)
    {
        _ = dbContext;

        cancellationToken.ThrowIfCancellationRequested();

        var isAudio = request.Slug.Equals("audio", StringComparison.OrdinalIgnoreCase)
            || request.Slug.Equals("am-thanh", StringComparison.OrdinalIgnoreCase)
            || request.Slug.Equals("thiet-bi-am-thanh", StringComparison.OrdinalIgnoreCase);
        var title = request.Slug.Equals("phone", StringComparison.OrdinalIgnoreCase)
            ? "Dien thoai"
            : isAudio ? "Thiet bi am thanh" : request.Slug;

        CategoryPageViewModel model = new()
        {
            Slug = request.Slug,
            Title = title,
            MetaDescription = $"Danh muc {title} tai TechStore.",
            LayoutMode = isAudio ? CategoryPageLayoutMode.Sectioned : CategoryPageLayoutMode.FilterListing,
            Breadcrumbs =
            [
                new() { Label = "Trang chu", Url = "/" },
                new() { Label = title, Url = $"/catalog?cat={request.Slug}", IsCurrent = true }
            ],
            PromotionBanners = [],
            Brands = [],
            QuickLinks = [],
            HotSale = new CategoryHotSaleViewModel { Title = "San pham noi bat", Products = [] },
            Filter = new CategoryFilterViewModel
            {
                Title = "Chon theo tieu chi",
                PrimaryItems = [],
                SecondaryItems = [],
                SortOptions = []
            },
            Products = [],
            SectionTabs = isAudio
                ?
                [
                    new() { Id = "headphones", Label = "Tai nghe", Url = "#section-headphones", IsActive = true },
                    new() { Id = "speakers", Label = "Loa", Url = "#section-speakers" },
                    new() { Id = "audio-accessories", Label = "Phu kien am thanh", Url = "#section-audio-accessories" }
                ]
                : [],
            ProductSections = [],
            SeoContent = new CategorySeoContentViewModel
            {
                Title = $"Thong tin danh muc {title}",
                Paragraphs = []
            },
            QuestionAnswer = new QuestionAnswerSectionViewModel
            {
                Title = "Hoi va dap",
                FormTitle = "Ban can tu van?",
                Description = "Hay gui cau hoi de TechStore ho tro.",
                Placeholder = "Viet cau hoi cua ban tai day",
                SubmitLabel = "Gui cau hoi",
                Threads = []
            }
        };

        return Task.FromResult<CategoryPageViewModel?>(model);
    }
}
