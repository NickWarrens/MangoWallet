using BL;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


var url = Environment.GetEnvironmentVariable("MYSQL_URL");
builder.Services.AddDbContext<MangoWalletDbContext>(options =>
{
    options.UseMySql(url, ServerVersion.AutoDetect(url));
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