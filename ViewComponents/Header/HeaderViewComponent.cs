using e_commerce_web_customer.Application.Navigation;
using e_commerce_web_customer.Application.Services;
using e_commerce_web_customer.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace e_commerce_web_customer.ViewComponents;

public class HeaderViewComponent : ViewComponent
{
    private readonly ISiteCategoryMenuProvider _categoryMenuProvider;
    private readonly CartSessionService _cartSessionService;

    public HeaderViewComponent(ISiteCategoryMenuProvider categoryMenuProvider, CartSessionService cartSessionService)
    {
        _categoryMenuProvider = categoryMenuProvider;
        _cartSessionService = cartSessionService;
    }

    public IViewComponentResult Invoke()
    {
        var items = _cartSessionService.Load();
        var count = items.Sum(i => Math.Max(1, i.Quantity));
        var isLoggedIn = HttpContext.Session.GetString(e_commerce_web_customer.Application.Constants.SessionKeys.IsLoggedIn) == "true";

        return View(new HeaderViewModel
        {
            CategoryMenu = _categoryMenuProvider.GetMenu(),
            CartItemCount = count,
            IsLoggedIn = isLoggedIn
        });
    }
}
