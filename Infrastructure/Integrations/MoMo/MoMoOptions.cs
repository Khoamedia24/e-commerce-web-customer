namespace e_commerce_web_customer.Infrastructure.Integrations.MoMo;

public sealed class MoMoOptions
{
    public const string SectionName = "MoMo";

    /// <summary>MOMO / MOMOBKUN20180529</summary>
    public string PartnerCode { get; set; } = string.Empty;

    /// <summary>F8BBA842ECF85 (sandbox)</summary>
    public string AccessKey { get; set; } = string.Empty;

    /// <summary>K951B6PE1waDMi640xX08PD3vg6EkVlz (sandbox)</summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>https://test-payment.momo.vn</summary>
    public string BaseUrl { get; set; } = "https://test-payment.momo.vn";

    /// <summary>payWithMethod (same as Flutter sportset_customer)</summary>
    public string RequestType { get; set; } = "payWithMethod";
}
