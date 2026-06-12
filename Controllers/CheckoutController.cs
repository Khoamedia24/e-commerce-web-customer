using e_commerce_web_customer.Application.Services;
using e_commerce_web_customer.Infrastructure.MockData;
using e_commerce_web_customer.ViewModels.Cart;
using e_commerce_web_customer.ViewModels.Checkout;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace e_commerce_web_customer.Controllers;

public sealed class CheckoutController(CartSessionService cartSession) : Controller
{
    private const string SuccessSessionKey = "checkout_success_order";

    [HttpGet]
    public IActionResult Index(bool demo = false, string mode = "")
    {
        if (HttpContext.Session.GetString(e_commerce_web_customer.Application.Constants.SessionKeys.IsLoggedIn) != "true")
        {
            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Checkout", new { mode = mode }) });
        }

        var model = BuildModel(demo, mode);

        if (model.Items.Count == 0 && !demo)
        {
            return RedirectToAction("Index", "Cart");
        }

        ViewData["Mode"] = mode;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(CheckoutViewModel model, [FromQuery] string mode = "")
    {
        if (HttpContext.Session.GetString(e_commerce_web_customer.Application.Constants.SessionKeys.IsLoggedIn) != "true")
        {
            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Checkout", new { mode = mode }) });
        }

        if (!ModelState.IsValid)
        {
            // Re-populate read-only order summary before returning view
            var fresh = BuildModel(mode: mode);
            model.Items       = fresh.Items;
            model.ShippingFee = fresh.ShippingFee;
            model.Discount    = fresh.Discount;
            ViewData["Mode"] = mode;
            return View(model);
        }

        // TODO: persist order and process payment before clearing the cart session
        var orderSnapshot = BuildModel(mode: mode);
        if (orderSnapshot.Items.Count == 0)
        {
            return RedirectToAction("Index", "Cart");
        }

        var successModel = BuildSuccessModel(model, orderSnapshot);
        HttpContext.Session.SetString(SuccessSessionKey, JsonSerializer.Serialize(successModel));

        if (mode == "buynow")
        {
            cartSession.ClearBuyNow();
        }
        else
        {
            cartSession.Clear();
        }
        
        return RedirectToAction(nameof(Success));
    }

    [HttpGet]
    public IActionResult Success()
    {
        var successModel = ReadSuccessModel();
        if (successModel is null)
        {
            return RedirectToAction("Index", "Cart");
        }

        return View(successModel);
    }

    // Build model from session. Demo data is opt-in through ?demo=true.
    private CheckoutViewModel BuildModel(bool demo = false, string mode = "")
    {
        var sessionItems = mode == "buynow" ? cartSession.LoadBuyNow() : cartSession.Load();

        var items = sessionItems.Count > 0
            ? sessionItems.Select(i => new CheckoutItemViewModel
            {
                Name      = i.Name,
                ImageUrl  = i.ImageUrl,
                ImageAlt  = i.ImageAlt,
                Variant   = i.Variant,
                UnitPrice = i.UnitPrice,
                Quantity  = i.Quantity,
            }).ToList()
            : demo ? MockCartData.GetCheckoutDemoItems() : [];

        return new CheckoutViewModel
        {
            Items       = items,
            ShippingFee = 30_000m,
            Discount    = 0m,
        };
    }

    private CheckoutSuccessViewModel? ReadSuccessModel()
    {
        var json = HttpContext.Session.GetString(SuccessSessionKey);
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<CheckoutSuccessViewModel>(json);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static CheckoutSuccessViewModel BuildSuccessModel(CheckoutViewModel submittedModel, CheckoutViewModel order)
        => new()
        {
            OrderCode = BuildOrderCode(),
            CustomerName = string.IsNullOrWhiteSpace(submittedModel.FullName) ? "Quý khách" : submittedModel.FullName.Trim(),
            Phone = submittedModel.Phone?.Trim() ?? string.Empty,
            Email = submittedModel.Email?.Trim() ?? string.Empty,
            DeliveryAddress = BuildDeliveryAddress(submittedModel),
            ShippingMethodName = "Giao hàng nhanh",
            PaymentMethodName = GetPaymentMethodName(submittedModel.PaymentMethod),
            PlacedAt = DateTimeOffset.Now.ToString("HH:mm, dd/MM/yyyy"),
            EstimatedDeliveryDateText = DateTimeOffset.Now.AddDays(2).ToString("dd/MM/yyyy"),
            ItemCountText = FormatItemCount(order.Items),
            SubtotalText = CheckoutViewModel.FormatPrice(order.Subtotal),
            ShippingFeeText = CheckoutViewModel.FormatPrice(order.ShippingFee),
            DiscountText = FormatDiscount(order.Discount),
            TotalText = CheckoutViewModel.FormatPrice(order.Total),
            Items = BuildSuccessItems(order.Items),
        };

    private static List<CheckoutSuccessItemViewModel> BuildSuccessItems(IEnumerable<CheckoutItemViewModel> items)
        => items.Select(item => new CheckoutSuccessItemViewModel
        {
            Name = item.Name,
            ImageUrl = item.ImageUrl,
            ImageAlt = item.ImageAlt,
            Variant = item.Variant,
            Quantity = item.Quantity,
            LineTotalText = CheckoutViewModel.FormatPrice(item.UnitPrice * item.Quantity),
        }).ToList();

    private static string BuildOrderCode() => $"TS{DateTimeOffset.Now:yyyyMMddHHmm}";

    private static string BuildDeliveryAddress(CheckoutViewModel model)
    {
        var addressParts = new[]
        {
            model.AddressDetail,
            model.Ward,
            model.District,
            model.Province,
        };

        var address = string.Join(", ", addressParts.Where(part => !string.IsNullOrWhiteSpace(part)).Select(part => part.Trim()));
        return string.IsNullOrWhiteSpace(address) ? "Địa chỉ sẽ được xác nhận qua điện thoại." : address;
    }

    private static string FormatItemCount(IReadOnlyCollection<CheckoutItemViewModel> items)
    {
        var totalQuantity = items.Sum(item => item.Quantity);
        return totalQuantity > 0 ? $"{totalQuantity} sản phẩm" : "Đơn hàng đang cập nhật";
    }

    private static string FormatDiscount(decimal discount)
        => discount > 0 ? $"-{CheckoutViewModel.FormatPrice(discount)}" : CheckoutViewModel.FormatPrice(0);

    private static string GetPaymentMethodName(PaymentMethod method) => method switch
    {
        PaymentMethod.BankTransfer => "Chuyển khoản ngân hàng",
        PaymentMethod.Momo => "Ví MoMo",
        PaymentMethod.VnPay => "Cổng VNPay",
        PaymentMethod.ZaloPay => "Ví ZaloPay",
        _ => "Thanh toán khi nhận hàng",
    };
}
