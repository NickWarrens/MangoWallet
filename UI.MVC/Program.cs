using BL;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


var rawUrl = Environment.GetEnvironmentVariable("MYSQL_URL") ?? "";
Console.WriteLine($"[DEBUG] MYSQL_URL = '{rawUrl}'");

if (rawUrl.StartsWith("mysql://", StringComparison.OrdinalIgnoreCase))
{
    // Parse the URI
    var uri = new Uri(rawUrl);
    var userInfo = uri.UserInfo.Split(':');

    var host = uri.Host;
    var port = uri.Port;
    var user = userInfo[0];
    var pass = userInfo[1];
    var database = uri.AbsolutePath.TrimStart('/'); // e.g. "railway"

    // Build a normal MySQL connection string
    rawUrl = $"Server={host};Port={port};User={user};Password={pass};Database={database}";
}
Console.WriteLine($"[DEBUG] Generated Connection String: {rawUrl}");
builder.Services.AddDbContext<MangoWalletDbContext>(options =>
{
    options.UseMySql(rawUrl, ServerVersion.AutoDetect(rawUrl));
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<ICurrencyBalanceRepository, CurrencyBalanceRepository>();

builder.Services.AddScoped<IManager, Manager>();

builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromHours(1);
    option.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MangoWalletDbContext>();
    context.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();