using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace e_commerce_web_customer.ViewModels.Checkout;

public sealed class CheckoutViewModel
{
    private static readonly CultureInfo VietnameseCulture = CultureInfo.GetCultureInfo("vi-VN");

    // ── Shipping form ──────────────────────────────────────────────
    [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
    [Display(Name = "Họ và tên")]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [Display(Name = "Số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn tỉnh / thành phố.")]
    [Display(Name = "Tỉnh / Thành phố")]
    public string Province { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn quận / huyện.")]
    [Display(Name = "Quận / Huyện")]
    public string District { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn phường / xã.")]
    [Display(Name = "Phường / Xã")]
    public string Ward { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ cụ thể.")]
    [Display(Name = "Địa chỉ cụ thể")]
    [MaxLength(300)]
    public string AddressDetail { get; set; } = string.Empty;

    [Display(Name = "Ghi chú cho đơn hàng")]
    [MaxLength(500)]
    public string? Note { get; set; }

    // ── Payment ────────────────────────────────────────────────────
    [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán.")]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cod;

    // ── Order summary (read-only, passed in from session/cart) ─────
    public IReadOnlyList<CheckoutItemViewModel> Items { get; set; } = [];
    public decimal ShippingFee { get; set; } = 30_000m;
    public decimal Discount { get; set; }

    public decimal Subtotal => Items.Sum(i => i.UnitPrice * i.Quantity);
    public decimal Total    => Subtotal + ShippingFee - Discount;

    public static string FormatPrice(decimal value)
        => $"{value.ToString("N0", VietnameseCulture)}đ";
}

public sealed class CheckoutItemViewModel
{
    public string Name      { get; init; } = string.Empty;
    public string ImageUrl  { get; init; } = string.Empty;
    public string ImageAlt  { get; init; } = string.Empty;
    public string Variant   { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public int Quantity     { get; init; } = 1;
}

public enum PaymentMethod
{
    Cod,           // Cash on delivery
    BankTransfer,  // Bank transfer
    Momo,          // MoMo e-wallet
    VnPay,         // VNPay gateway
    ZaloPay        // ZaloPay
}
