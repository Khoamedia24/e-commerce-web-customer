using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;

namespace e_commerce_web_customer.Controllers;

public sealed class AccountController(IAccountService accountService) : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        var success = await accountService.LoginAsync(model.Email, model.Password, model.RememberMe);
        if (success)
        {
            HttpContext.Session.SetString(e_commerce_web_customer.Application.Constants.SessionKeys.IsLoggedIn, "true");
            HttpContext.Session.SetString(e_commerce_web_customer.Application.Constants.SessionKeys.UserEmail, model.Email);
            
            TempData["AuthSuccess"] = "Đăng nhập thành công! Chào mừng bạn quay lại TechStore.";
            
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(
            string.Empty,
            "Email hoặc mật khẩu không chính xác, hoặc tài khoản chưa tồn tại.");

        ViewData["ReturnUrl"] = returnUrl;
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove(e_commerce_web_customer.Application.Constants.SessionKeys.IsLoggedIn);
        HttpContext.Session.Remove(e_commerce_web_customer.Application.Constants.SessionKeys.UserEmail);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var exists = await accountService.UserExistsAsync(model.Email);
        if (exists)
        {
            ModelState.AddModelError(nameof(model.Email), "Email này đã được đăng ký sử dụng.");
            return View(model);
        }

        var success = await accountService.RegisterAsync(model);
        if (success)
        {
            TempData["AuthSuccess"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction(nameof(Login));
        }

        ModelState.AddModelError(
            string.Empty,
            "Đăng ký thất bại. Đã xảy ra lỗi, vui lòng thử lại.");

        return View(model);
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        TempData["AuthNotice"] =
            "Khôi phục mật khẩu cần được kết nối với dịch vụ gửi email trước khi sử dụng.";

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult ExternalLogin(string provider, string? source = null)
    {
        var providerName = provider?.ToLowerInvariant() switch
        {
            "google" => "Google",
            "facebook" => "Facebook",
            _ => null
        };

        if (providerName is null)
        {
            return BadRequest();
        }

        TempData["AuthNotice"] =
            $"Kết nối {providerName} cần được cấu hình trước khi sử dụng đăng nhập mạng xã hội.";

        return string.Equals(source, nameof(Register), StringComparison.OrdinalIgnoreCase)
            ? RedirectToAction(nameof(Register))
            : RedirectToAction(nameof(Login));
    }
}
