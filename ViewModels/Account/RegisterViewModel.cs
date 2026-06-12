using System.ComponentModel.DataAnnotations;

namespace e_commerce_web_customer.ViewModels.Account;

public sealed class RegisterViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Họ và tên cần từ 2 đến 80 ký tự.")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email chưa đúng định dạng.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Số điện thoại chưa đúng định dạng.")]
    [Display(Name = "Số điện thoại")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu cần ít nhất 8 ký tự.")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$", ErrorMessage = "Mật khẩu cần có cả chữ và số.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu.")]
    [Compare(nameof(Password), ErrorMessage = "Mật khẩu nhập lại chưa khớp.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nhập lại mật khẩu")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Display(Name = "Đồng ý với điều khoản")]
    public bool AgreeToTerms { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!AgreeToTerms)
        {
            yield return new ValidationResult(
                "Bạn cần đồng ý với điều khoản sử dụng và chính sách bảo mật.",
                [nameof(AgreeToTerms)]);
        }
    }
}
