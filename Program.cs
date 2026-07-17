using LoadingQueue.Data;
using LoadingQueue.Models;
using LoadingQueue.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // به‌جز جاهایی که صراحتاً [AllowAnonymous] دارن (مثل صفحه‌ی Login)،
    // همه‌ی صفحات نیاز به لاگین دارن
    var policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
});


#region DB Context Connect to Sql server

// کانکشن استرینگ دیگه هاردکد نیست؛ از appsettings.json /
// appsettings.Development.json خونده می‌شه (کلید ConnectionStrings:Default).
// این‌طوری وقتی پروژه رو روی یه کامپیوتر دیگه اجرا کنی، فقط کافیه
// appsettings.Development.json رو با آدرس سرور خودت عوض کنی، لازم
// نیست کد رو دست بزنی.
builder.Services.AddDbContext<QueueDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
#endregion


#region Identity (احراز هویت و نقش‌ها)

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // فعلاً قوانین رمز عبور رو ساده نگه می‌داریم (محیط توسعه)
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<QueueDBContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

#endregion


#region 
// ----- ثبت Repositoryها -----
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IQueueRepository, QueueRepository>();

builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();

builder.Services.AddScoped<IShippingCompanyRepository, ShippingCompanyRepository>();
builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();


#endregion


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


#region ساخت خودکار نقش‌ها + کاربر مدیر کل + مدیر شرکت پیش‌فرض (فقط برای تست)

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "SystemAdmin", "CompanyAdmin", "User", "Driver" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // مدیر کل سیستم - به هیچ شرکتی وصل نیست، همه‌چیز رو می‌بینه
    var adminEmail = "admin@loadingqueue.local";

    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "مدیر کل سیستم",
            CompanyId = null,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, "Admin@123");

        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, "SystemAdmin");
    }

    // مدیر شرکت پیش‌فرض - فقط به شرکت Id=1 (همون شرکتی که رانندگان
    // و نوبت‌های فعلی بهش وصلن) دسترسی داره
    var companyAdminEmail = "companyadmin@loadingqueue.local";

    if (await userManager.FindByEmailAsync(companyAdminEmail) == null)
    {
        var companyAdmin = new ApplicationUser
        {
            UserName = companyAdminEmail,
            Email = companyAdminEmail,
            FullName = "مدیر شرکت پیش‌فرض",
            CompanyId = 1,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(companyAdmin, "Admin@123");

        if (result.Succeeded)
            await userManager.AddToRoleAsync(companyAdmin, "CompanyAdmin");
    }
}

#endregion


app.Run();