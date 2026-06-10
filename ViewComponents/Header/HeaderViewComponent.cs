using Microsoft.AspNetCore.Mvc;

namespace e_commerce_web_customer.ViewComponents;

public class HeaderViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() => View();
}
