using e_commerce_web_customer.ViewModels.Shared;

namespace e_commerce_web_customer.Infrastructure.Home.Mock;

internal static class HomeProductCardFactory
{
    public static ProductCardViewModel Create(
        string id,
        string name,
        string imageUrl,
        string currentPrice,
        string? oldPrice,
        string? discountLabel,
        string memberOffer,
        string promotionNote,
        decimal? rating = 5.0m,
        string? studentOffer = null,
        string? imageAlt = null,
        string? installmentLabel = "Trả góp 0%")
    {
        return new ProductCardViewModel
        {
            Id = id,
            Name = name,
            Url = $"/product/{id}",
            ImageUrl = imageUrl,
            ImageAlt = imageAlt ?? name,
            CurrentPrice = currentPrice,
            OldPrice = oldPrice,
            DiscountLabel = discountLabel,
            InstallmentLabel = installmentLabel,
            MemberOffer = memberOffer,
            StudentOffer = studentOffer,
            PromotionNote = promotionNote,
            DeliveryLabel = "Giao 2 giờ",
            Location = "Hồ Chí Minh",
            Rating = rating
        };
    }

    public static IReadOnlyList<ProductCardViewModel> AddVariants(
        IReadOnlyList<ProductCardViewModel> products,
        IReadOnlyList<HomeProductVariant> variants)
    {
        if (variants.Count == 0)
        {
            return products;
        }

        var result = new List<ProductCardViewModel>(products.Count + variants.Count);
        result.AddRange(products);

        foreach (var variant in variants)
        {
            var source = products[variant.SourceIndex];

            result.Add(new ProductCardViewModel
            {
                Id = variant.Id,
                Name = variant.Name,
                Url = $"/product/{variant.Id}",
                ImageUrl = source.ImageUrl,
                ImageAlt = variant.Name,
                CurrentPrice = source.CurrentPrice,
                OldPrice = source.OldPrice,
                DiscountLabel = source.DiscountLabel,
                InstallmentLabel = source.InstallmentLabel,
                MemberOffer = source.MemberOffer,
                StudentOffer = source.StudentOffer,
                PromotionNote = source.PromotionNote,
                AvailabilityLabel = source.AvailabilityLabel,
                Specifications = source.Specifications,
                DeliveryLabel = source.DeliveryLabel,
                Location = source.Location,
                Rating = source.Rating,
                IsWishlisted = source.IsWishlisted,
                ShowWishlistAction = source.ShowWishlistAction
            });
        }

        return result;
    }
}

internal sealed record HomeProductVariant(int SourceIndex, string Id, string Name);
