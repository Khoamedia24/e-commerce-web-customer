using System.Text.Json;
using e_commerce_web_customer.Application.Constants;
using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Orders;
using e_commerce_web_customer.Application.Services;
using e_commerce_web_customer.ViewModels.Checkout;
using Microsoft.AspNetCore.Mvc;

namespace e_commerce_web_customer.Controllers;

public sealed class CheckoutController(
    CartSessionService cartSession,
    ICartItemValidator cartItemValidator,
    ICartPersistenceService cartPersistenceService,
    ICartDemoDataProvider demoDataProvider,
    ICheckoutPaymentMethodProvider paymentMethodProvider,
    IOrderService orderService,
    IMoMoIntegration momoIntegration) : Controller
{
    private const string SuccessSessionKey = "checkout_success_order";

    [HttpGet]
    public async Task<IActionResult> Index(
        bool demo = false,
        string mode = "",
        CancellationToken cancellationToken = default)
    {
        if (!IsLoggedIn())
        {
            return RedirectToLogin(mode);
        }

        var model = await BuildModelAsync(demo, mode, cancellationToken);
        if (model.Items.Count == 0 && !demo)
        {
            return RedirectToAction("Index", "Cart");
        }

        ViewData["Mode"] = mode;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(
        CheckoutViewModel model,
        [FromQuery] string mode = "",
        CancellationToken cancellationToken = default)
    {
        if (!IsLoggedIn())
        {
            return RedirectToLogin(mode);
        }

        var orderSnapshot = await BuildModelAsync(
            demo: false,
            mode,
            cancellationToken);

        if (orderSnapshot.Items.Count == 0)
        {
            return RedirectToAction("Index", "Cart");
        }

        if (!ModelState.IsValid)
        {
            RestoreOrderSummary(model, orderSnapshot);
            ViewData["Mode"] = mode;
            return View(model);
        }

        try
        {
            var placedOrder = await orderService.PlaceOrderAsync(
                BuildOrderRequest(model, orderSnapshot),
                cancellationToken);
            var successModel = BuildSuccessModel(
                model,
                orderSnapshot,
                placedOrder);

            HttpContext.Session.SetString(
                SuccessSessionKey,
                JsonSerializer.Serialize(successModel));
            await ClearCompletedCartAsync(mode, cancellationToken);

            // MoMo payment — mirrors Flutter: MoMoService.createMoMoPayment() → launchUrl(payUrl)
            if (IsMoMoPayment(model.PaymentMethodId))
            {
                var redirectUrl = Url.ActionLink("MoMoReturn", "Payment");
                var ipnUrl = Url.ActionLink("MoMoIpn", "Payment");
                var momoReq = new MoMoCreateRequest(
                    OrderId: placedOrder.OrderCode,
                    Amount: (long)orderSnapshot.Total,
                    OrderInfo: $"Thanh toan don hang {placedOrder.OrderCode}",
                    RedirectUrl: redirectUrl ?? string.Empty,
                    IpnUrl: ipnUrl ?? string.Empty);

                var momoResult = await momoIntegration.CreatePaymentAsync(momoReq, cancellationToken);

                if (momoResult.Success && !string.IsNullOrEmpty(momoResult.PayUrl))
                {
                    // Redirect sang MoMo sandbox — equivalent to launchUrl(uri) in Flutter
                    return Redirect(momoResult.PayUrl);
                }

                ModelState.AddModelError(string.Empty,
                    momoResult.ErrorMessage ?? "Không thể khởi tạo giao dịch MoMo.");
                RestoreOrderSummary(model, orderSnapshot);
                ViewData["Mode"] = mode;
                return View(model);
            }

            return RedirectToAction(nameof(Success));
        }
        catch (OrderPlacementException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            RestoreOrderSummary(model, orderSnapshot);
            ViewData["Mode"] = mode;
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Success()
    {
        var successModel = ReadSuccessModel();
        return successModel is null
            ? RedirectToAction("Index", "Cart")
            : View(successModel);
    }

    private async Task<CheckoutViewModel> BuildModelAsync(
        bool demo,
        string mode,
        CancellationToken cancellationToken)
    {
        var sessionItems = string.Equals(
            mode,
            "buynow",
            StringComparison.OrdinalIgnoreCase)
            ? cartSession.LoadBuyNow()
            : cartSession.Load();

        if (sessionItems.Count == 0
            && !string.Equals(mode, "buynow", StringComparison.OrdinalIgnoreCase)
            && GetLoggedInUserEmail() is { } storedCartEmail)
        {
            sessionItems = await cartPersistenceService.LoadAsync(
                storedCartEmail,
                cancellationToken);
            cartSession.Save(sessionItems);
        }

        IReadOnlyList<CheckoutItemViewModel> items;
        if (sessionItems.Count > 0)
        {
            var validatedItems = new List<CartSessionItem>();
            foreach (var sessionItem in sessionItems)
            {
                try
                {
                    validatedItems.Add(await cartItemValidator.ValidateAsync(
                        sessionItem,
                        cancellationToken));
                }
                catch (CartItemValidationException)
                {
                    // Products that became inactive or out of stock are removed.
                }
            }

            if (string.Equals(mode, "buynow", StringComparison.OrdinalIgnoreCase))
            {
                if (validatedItems.Count > 0)
                {
                    cartSession.SaveBuyNow(validatedItems[0]);
                }
                else
                {
                    cartSession.ClearBuyNow();
                }
            }
            else
            {
                cartSession.Save(validatedItems);
                if (GetLoggedInUserEmail() is { } userEmail)
                {
                    await cartPersistenceService.SaveAsync(
                        userEmail,
                        validatedItems,
                        cancellationToken);
                }
            }

            items = validatedItems.Select(item => new CheckoutItemViewModel
            {
                ProductId = item.Id,
                Name = item.Name,
                ImageUrl = item.ImageUrl,
                ImageAlt = item.ImageAlt,
                Variant = item.Variant,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity
            }).ToList();
        }
        else if (demo)
        {
            items = await demoDataProvider.GetCheckoutItemsAsync(
                cancellationToken);
        }
        else
        {
            items = [];
        }

        var paymentMethods = await paymentMethodProvider.GetActivePaymentMethodsAsync(
            cancellationToken);

        return new CheckoutViewModel
        {
            Items = items,
            PaymentMethods = paymentMethods,
            PaymentMethodId = paymentMethods.FirstOrDefault()?.Id ?? 0,
            ShippingFee = 30_000m,
            Discount = 0m
        };
    }

    private PlaceOrderRequest BuildOrderRequest(
        CheckoutViewModel submittedModel,
        CheckoutViewModel order)
    {
        return new PlaceOrderRequest(
            GetRequiredLoggedInUserEmail(),
            submittedModel.FullName.Trim(),
            submittedModel.Phone.Trim(),
            submittedModel.Email.Trim(),
            submittedModel.Province.Trim(),
            submittedModel.Ward.Trim(),
            BuildShippingDetail(submittedModel),
            submittedModel.PaymentMethodId,
            submittedModel.Note?.Trim(),
            order.ShippingFee,
            order.Discount,
            order.Items.Select(item => new PlaceOrderLine(
                item.ProductId,
                item.Name,
                item.Variant,
                item.UnitPrice,
                item.Quantity)).ToList());
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

    private static CheckoutSuccessViewModel BuildSuccessModel(
        CheckoutViewModel submittedModel,
        CheckoutViewModel order,
        PlacedOrder placedOrder)
    {
        return new CheckoutSuccessViewModel
        {
            OrderCode = placedOrder.OrderCode,
            CustomerName = string.IsNullOrWhiteSpace(submittedModel.FullName)
                ? "Quý khách"
                : submittedModel.FullName.Trim(),
            Phone = submittedModel.Phone?.Trim() ?? string.Empty,
            Email = submittedModel.Email?.Trim() ?? string.Empty,
            DeliveryAddress = BuildDeliveryAddress(submittedModel),
            ShippingMethodName = "Giao hàng nhanh",
            PaymentMethodName = GetPaymentMethodName(
                submittedModel.PaymentMethodId,
                order.PaymentMethods),
            PlacedAt = placedOrder.PlacedAt.ToString("HH:mm, dd/MM/yyyy"),
            EstimatedDeliveryDateText =
                placedOrder.EstimatedDeliveryAt.ToString("dd/MM/yyyy"),
            ItemCountText = FormatItemCount(order.Items),
            SubtotalText = CheckoutViewModel.FormatPrice(order.Subtotal),
            ShippingFeeText = CheckoutViewModel.FormatPrice(order.ShippingFee),
            DiscountText = FormatDiscount(order.Discount),
            TotalText = CheckoutViewModel.FormatPrice(order.Total),
            Items = BuildSuccessItems(order.Items)
        };
    }

    private static List<CheckoutSuccessItemViewModel> BuildSuccessItems(
        IEnumerable<CheckoutItemViewModel> items)
    {
        return items.Select(item => new CheckoutSuccessItemViewModel
        {
            Name = item.Name,
            ImageUrl = item.ImageUrl,
            ImageAlt = item.ImageAlt,
            Variant = item.Variant,
            Quantity = item.Quantity,
            LineTotalText = CheckoutViewModel.FormatPrice(
                item.UnitPrice * item.Quantity)
        }).ToList();
    }

    private static void RestoreOrderSummary(
        CheckoutViewModel target,
        CheckoutViewModel source)
    {
        target.Items = source.Items;
        target.PaymentMethods = source.PaymentMethods;
        target.ShippingFee = source.ShippingFee;
        target.Discount = source.Discount;
    }

    private async Task ClearCompletedCartAsync(
        string mode,
        CancellationToken cancellationToken)
    {
        if (string.Equals(mode, "buynow", StringComparison.OrdinalIgnoreCase))
        {
            cartSession.ClearBuyNow();
            return;
        }

        cartSession.Clear();
        if (GetLoggedInUserEmail() is { } userEmail)
        {
            await cartPersistenceService.ClearAsync(
                userEmail,
                cancellationToken);
        }
    }

    private bool IsLoggedIn()
    {
        return HttpContext.Session.GetString(SessionKeys.IsLoggedIn) == "true";
    }

    private string? GetLoggedInUserEmail()
    {
        if (!IsLoggedIn())
        {
            return null;
        }

        var email = HttpContext.Session.GetString(SessionKeys.UserEmail);
        return string.IsNullOrWhiteSpace(email) ? null : email.Trim();
    }

    private string GetRequiredLoggedInUserEmail()
    {
        return GetLoggedInUserEmail()
            ?? throw new OrderPlacementException("Không xác định được tài khoản đặt hàng.");
    }

    private IActionResult RedirectToLogin(string mode)
    {
        return RedirectToAction(
            "Login",
            "Account",
            new
            {
                returnUrl = Url.Action("Index", "Checkout", new { mode })
            });
    }

    private static string BuildDeliveryAddress(CheckoutViewModel model)
    {
        var addressParts = new[]
        {
            model.AddressDetail,
            model.Ward,
            model.District,
            model.Province
        };

        var address = string.Join(
            ", ",
            addressParts
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .Select(part => part.Trim()));

        return string.IsNullOrWhiteSpace(address)
            ? "Địa chỉ sẽ được xác nhận qua điện thoại."
            : address;
    }

    private static string BuildShippingDetail(CheckoutViewModel model)
    {
        var parts = new[]
        {
            model.AddressDetail,
            model.District
        };

        return string.Join(
            ", ",
            parts
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .Select(part => part.Trim()));
    }

    private static string FormatItemCount(
        IReadOnlyCollection<CheckoutItemViewModel> items)
    {
        var totalQuantity = items.Sum(item => item.Quantity);
        return totalQuantity > 0
            ? $"{totalQuantity} sản phẩm"
            : "Đơn hàng đang cập nhật";
    }

    private static string FormatDiscount(decimal discount)
    {
        return discount > 0
            ? $"-{CheckoutViewModel.FormatPrice(discount)}"
            : CheckoutViewModel.FormatPrice(0);
    }

    private static string GetPaymentMethodName(
        long paymentMethodId,
        IReadOnlyCollection<CheckoutPaymentMethodViewModel> paymentMethods)
    {
        return paymentMethods
            .FirstOrDefault(method => method.Id == paymentMethodId)
            ?.Name
            ?? "Phương thức thanh toán";
    }

    /// <summary>
    /// Returns true if the selected payment method is MoMo.
    /// DB: payment_methods — Id=1 COD, Id=2 Momo, Id=3 Vnpay, Id=4 ZaloPay
    /// </summary>
    private static bool IsMoMoPayment(long paymentMethodId)
    {
        return paymentMethodId == 2; // Id=2 → Momo (verified from DB)
    }
}
