using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Application.Home;
using e_commerce_web_customer.Application.Navigation;
using e_commerce_web_customer.Application.Product;
using e_commerce_web_customer.Application.Services;
using e_commerce_web_customer.Infrastructure.Web;
using e_commerce_web_customer.Infrastructure.Services;
using e_commerce_web_customer.Infrastructure.MockData;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ISiteCategoryMenuProvider, MockSiteCategoryMenuProvider>();
builder.Services.AddSingleton<IProductDetailViewModelFactory, MockProductDetailViewModelFactory>();

// Session support — used to pass cart data from Cart → Checkout
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISessionStorage, WebSessionStorage>();
builder.Services.AddScoped<CartSessionService>();

var useMockData = builder.Configuration.GetValue<bool>("DatabaseSettings:UseMockData", true);
if (useMockData)
{
    builder.Services.AddSingleton<IHomePageViewModelFactory, MockHomePageViewModelFactory>();
    builder.Services.AddScoped<IAccountService, MockAccountService>();
    builder.Services.AddScoped<ICartItemValidator, MockCartItemValidator>();
}
else
{
    builder.Services.AddScoped<IHomePageViewModelFactory, DbHomePageViewModelFactory>();
    builder.Services.AddScoped<IAccountService, DbAccountService>();
    builder.Services.AddScoped<ICartItemValidator, DbCartItemValidator>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
