using e_commerce_web_customer.Application.Services;
using e_commerce_web_customer.Application.Contracts;
using e_commerce_web_customer.Infrastructure.DependencyInjection;
using e_commerce_web_customer.Infrastructure.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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
    builder.Services.AddMockStorefrontServices();
}
else
{
    builder.Services.AddDatabaseStorefrontServices(builder.Configuration);
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
